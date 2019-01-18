using System;
using System.Collections.Generic;
using SSBHLib.Formats.Animation;
using SSBHLib.IO;
using System.IO;
using System.Runtime.InteropServices;

namespace SSBHLib.Tools
{
    public struct AnimTrackTransform
    {
        public float X;
        public float Y;
        public float Z;

        public float RX;
        public float RY;
        public float RZ;
        public float RW;

        public float SX;
        public float SY;
        public float SZ;

        public float AbsoluteScale;

        public override string ToString()
        {
            return $"(Position: ({X}, {Y}, {Z}), Rotation: ({RX}, {RY}, {RZ}, {RW}), Scale: ({SX}, {SY}, {SZ}))";
        }
    }

    public struct AnimTrackCustomVector4
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public AnimTrackCustomVector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z}, {W})";
        }
    }

    public class SSBHAnimTrackDecoder
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SSBHAnimCompressedHeader
        {
            public ushort Unk_4; // always 4?
            public ushort Flags;
            public ushort DefaultDataOffset;
            public ushort BitsPerEntry;
            public int CompressedDataOffset;
            public int FrameCount;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SSBHAnimCompressedItem
        {
            public float Start;
            public float End;
            public ulong Count;
        }

        private ANIM AnimFile;

        public SSBHAnimTrackDecoder(ANIM AnimFile)
        {
            this.AnimFile = AnimFile;
        }

        private bool CheckFlag(uint flags, uint mask, ANIM_TRACKFLAGS expectedValue)
        {
            return (flags & mask) == (uint)expectedValue;
        }

        public object[] ReadTrack(AnimTrack Track)
        {
            //Console.WriteLine(Track.Name + " " + Track.Flags.ToString("X") + " " + Track.FrameCount + " " + Track.DataOffset.ToString("X"));
            List<object> output = new List<object>();
            using (SSBHParser parser = new SSBHParser(new MemoryStream(AnimFile.Buffer)))
            {
                parser.Seek(Track.DataOffset);

                if (CheckFlag(Track.Flags, 0xFF00, ANIM_TRACKFLAGS.Constant))
                {
                    output.Add(ReadDirect(parser, Track.Flags));
                }
                if (CheckFlag(Track.Flags, 0xFF00, ANIM_TRACKFLAGS.ConstTransform))
                {
                    // TODO: investigate more
                    output.Add(ReadDirect(parser, Track.Flags));
                }
                if (CheckFlag(Track.Flags, 0xFF00, ANIM_TRACKFLAGS.Direct))
                {
                    for(int i = 0; i < Track.FrameCount; i++)
                        output.Add(ReadDirect(parser, Track.Flags));
                }
                if (CheckFlag(Track.Flags, 0xFF00, ANIM_TRACKFLAGS.Compressed))
                {
                    output.AddRange(ReadCompressed(parser, Track.Flags));
                }
            }

            return output.ToArray();
        }

        private object[] ReadCompressed(SSBHParser reader, uint flags)
        {
            List<object> output = new List<object>();

            uint DataOffset = (uint)reader.BaseStream.Position;
            SSBHAnimCompressedHeader Header = reader.ByteToType<SSBHAnimCompressedHeader>();

            if (CheckFlag(flags, 0x00FF, ANIM_TRACKFLAGS.Boolean))
            {
                reader.Seek((int)DataOffset + Header.CompressedDataOffset);
                // note: there is a section for "default" and "compressed items" but they seem to always be 0-ed out

                for (int i = 0; i < Header.FrameCount; i++)
                {
                    output.Add(reader.ReadBits(Header.BitsPerEntry) == 1);
                }
            }
            if (CheckFlag(flags, 0x00FF, ANIM_TRACKFLAGS.Texture))
            {
                // TODO: What type is this
            }
            if (CheckFlag(flags, 0x00FF, ANIM_TRACKFLAGS.Float))
            {
                //TODO: What type is this
            }
            if (CheckFlag(flags, 0x00FF, ANIM_TRACKFLAGS.PatternIndex))
            {
                //TODO: What type is this
            }
            if (CheckFlag(flags, 0x00FF, ANIM_TRACKFLAGS.Vector4))
            {
                var decompressed = DecompressValues(reader, DataOffset, Header, 4);
                foreach(var decom in decompressed)
                    output.Add(new AnimTrackCustomVector4(decom[0], decom[1], decom[2], decom[3]));
            }
            if (CheckFlag(flags, 0x00FF, ANIM_TRACKFLAGS.Transform))
            {
                var decompressed = DecompressTransform(reader, DataOffset, Header);
                foreach (var v in decompressed)
                    output.Add(v);
            }

            return output.ToArray();
        }

        private List<float[]> DecompressValues(SSBHParser parser, uint dataOffset, SSBHAnimCompressedHeader header, int valueCount)
        {
            List<float[]> transforms = new List<float[]>(header.FrameCount);

            // PreProcess
            SSBHAnimCompressedItem[] items = parser.ByteToType<SSBHAnimCompressedItem>(valueCount);

            parser.Seek(dataOffset + header.DefaultDataOffset);

            float[] defaultValues = new float[valueCount];
            for(int i = 0; i < valueCount; i++)
            {
                defaultValues[i] = parser.ReadSingle();
            }

            parser.Seek(dataOffset + header.CompressedDataOffset);
            for (int frameIndex = 0; frameIndex < header.FrameCount; frameIndex++)
            {
                float[] values = new float[valueCount];
                for (int i = 0; i < valueCount; i++)
                    values[i] = defaultValues[i];

                for (int itemIndex = 0; itemIndex < items.Length; itemIndex++)
                {
                    var item = items[itemIndex];

                    // Decompress
                    int valueBitCount = (int)item.Count;
                    if (valueBitCount == 0)
                        continue;

                    int value = parser.ReadBits(valueBitCount);
                    int scale = 0;
                    for (int k = 0; k < valueBitCount; k++)
                        scale |= 0x1 << k;

                    float frameValue = Lerp(item.Start, item.End, 0, 1, value / (float)scale);
                    if (float.IsNaN(frameValue))
                        frameValue = 0;

                    values[itemIndex] = frameValue;
                }

                transforms.Add(values);
            }

            return transforms;
        }

        private AnimTrackTransform[] DecompressTransform(SSBHParser parser, uint dataOffset, SSBHAnimCompressedHeader header)
        {
            AnimTrackTransform[] transforms = new AnimTrackTransform[header.FrameCount];

            // PreProcess
            SSBHAnimCompressedItem[] Items = parser.ByteToType<SSBHAnimCompressedItem>(9);
            
            parser.Seek(dataOffset + header.DefaultDataOffset);

            float XSCA = parser.ReadSingle();
            float YSCA = parser.ReadSingle();
            float ZSCA = parser.ReadSingle();
            float XROT = parser.ReadSingle();
            float YROT = parser.ReadSingle();
            float ZROT = parser.ReadSingle();
            float WROT = parser.ReadSingle();
            float XPOS = parser.ReadSingle();
            float YPOS = parser.ReadSingle();
            float ZPOS = parser.ReadSingle();
            float WPOS = parser.ReadInt32(); // ????

            parser.Seek(dataOffset + header.CompressedDataOffset);
            for(int frame = 0; frame < header.FrameCount; frame++)
            {
                AnimTrackTransform transform = new AnimTrackTransform()
                {
                    X = XPOS,
                    Y = YPOS,
                    Z = ZPOS,
                    RX = XROT,
                    RY = YROT,
                    RZ = ZROT,
                    RW = WROT,
                    SX = XSCA,
                    SY = YSCA,
                    SZ = ZSCA,
                    AbsoluteScale = 1
                };
                for (int itemIndex = 0; itemIndex < Items.Length; itemIndex++)
                {
                    // First check if this track should be parsed
                    // TODO: Don't hard code these flags.
                    if (!((itemIndex == 0 && (header.Flags & 0x3) == 0x3) //isotropic scale
                                || (itemIndex >= 0 && itemIndex <= 2 && (header.Flags & 0x3) == 0x1) //normal scale
                                || (itemIndex > 2 && itemIndex <= 5 && (header.Flags & 0x4) > 0)
                                || (itemIndex > 5 && itemIndex <= 8 && (header.Flags & 0x8) > 0)))
                    {
                        continue;
                    }

                    var item = Items[itemIndex];

                    // Decompress
                    int valueBitCount = (int)item.Count;
                    if (valueBitCount == 0)
                        continue;

                    int value = parser.ReadBits(valueBitCount);
                    int scale = 0;
                    for (int k = 0; k < valueBitCount; k++)
                        scale |= 0x1 << k;

                    float frameValue = Lerp(item.Start, item.End, 0, 1, value / (float)scale);
                    if (float.IsNaN(frameValue))
                        frameValue = 0;

                    // the Transform type relies a lot on flags
                    
                    if ((header.Flags & 0x3) == 0x3)
                    {
                        //Scale Isotropic
                        if(itemIndex == 0)
                        {
                            transform.AbsoluteScale = frameValue;
                        }
                    }
                    if ((header.Flags & 0x3) == 0x1)
                    {
                        //Scale normal
                        switch (itemIndex)
                        {
                            case 0:
                                transform.SX = frameValue;
                                break;
                            case 1:
                                transform.SY = frameValue;
                                break;
                            case 2:
                                transform.SZ = frameValue;
                                break;
                        }
                    }
                    //Rotation and Position
                    switch (itemIndex)
                    {
                        case 3:
                            transform.RX = frameValue;
                            break;
                        case 4:
                            transform.RY = frameValue;
                            break;
                        case 5:
                            transform.RZ = frameValue;
                            break;
                        case 6:
                            transform.X = frameValue;
                            break;
                        case 7:
                            transform.Y = frameValue;
                            break;
                        case 8:
                            transform.Z = frameValue;
                            break;
                    }
                }

                // Rotations have an extra bit at the end
                if ((header.Flags & 0x4) > 0)
                {
                    bool wFlip = parser.ReadBits(1) == 1;

                    // W is calculated
                    transform.RW = (float)Math.Sqrt(Math.Abs(1 - (transform.RX * transform.RX + transform.RY * transform.RY + transform.RZ * transform.RZ)));

                    if (wFlip)
                        transform.RW = -transform.RW;
                }

                transforms[frame] = transform;
            }

            return transforms;
        }

        private object ReadDirect(SSBHParser reader, uint flags)
        {
            if (CheckFlag(flags, 0x00FF, ANIM_TRACKFLAGS.Transform))
            {
                var Transform = new AnimTrackTransform()
                {
                    SX = reader.ReadSingle(),
                    SY = reader.ReadSingle(),
                    SZ = reader.ReadSingle(),
                    RX = reader.ReadSingle(),
                    RY = reader.ReadSingle(),
                    RZ = reader.ReadSingle(),
                    RW = reader.ReadSingle(),
                    X = reader.ReadSingle(),
                    Y = reader.ReadSingle(),
                    Z = reader.ReadSingle(),
                    AbsoluteScale = 1
                };
                reader.ReadInt32(); // ????

                return Transform;
            }

            if (CheckFlag(flags, 0x00FF, ANIM_TRACKFLAGS.Texture))
            {
                // TODO: What type is this
            }

            if (CheckFlag(flags, 0x00FF, ANIM_TRACKFLAGS.Float))
                return reader.ReadSingle();

            if (CheckFlag(flags, 0x00FF, ANIM_TRACKFLAGS.PatternIndex))
            {
                //TODO: What type is this
            }

            if (CheckFlag(flags, 0x00FF, ANIM_TRACKFLAGS.Boolean))
                return reader.ReadByte() == 1;

            if (CheckFlag(flags, 0x00FF, ANIM_TRACKFLAGS.Vector4))
                return new AnimTrackCustomVector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            return null;
        }

        public static float Lerp(float av, float bv, float v0, float v1, float t)
        {
            if (v0 == v1) return av;

            if (t == v0) return av;
            if (t == v1) return bv;
            
            float mu = (t - v0) / (v1 - v0);
            return ((av * (1 - mu)) + (bv * mu));
        }
    }
}
