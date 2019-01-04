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

        public AnimTrackCustomVector4(float X, float Y, float Z, float W)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.W = W;
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

        private bool CheckFlag(uint Flags, uint Mask, ANIM_TRACKFLAGS ToCheck)
        {
            return (Flags & Mask) == (uint)ToCheck;
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

        private object[] ReadCompressed(SSBHParser reader, uint Flags)
        {
            List<object> output = new List<object>();

            uint DataOffset = (uint)reader.BaseStream.Position;
            SSBHAnimCompressedHeader Header = reader.ByteToType<SSBHAnimCompressedHeader>();

            if (CheckFlag(Flags, 0x00FF, ANIM_TRACKFLAGS.Boolean))
            {
                reader.Seek((int)DataOffset + Header.CompressedDataOffset);
                // note: there is a section for "default" and "compressed items" but they seem to always be 0-ed out

                for (int i = 0; i < Header.FrameCount; i++)
                {
                    output.Add(reader.ReadBits(Header.BitsPerEntry) == 1);
                }
            }
            if (CheckFlag(Flags, 0x00FF, ANIM_TRACKFLAGS.Texture))
            {
                // TODO: What type is this
            }
            if (CheckFlag(Flags, 0x00FF, ANIM_TRACKFLAGS.Float))
            {
                //TODO: What type is this
            }
            if (CheckFlag(Flags, 0x00FF, ANIM_TRACKFLAGS.PatternIndex))
            {
                //TODO: What type is this
            }
            if (CheckFlag(Flags, 0x00FF, ANIM_TRACKFLAGS.Vector4))
            {
                var decompressed = DecompressValues(reader, DataOffset, Header, 4);
                foreach(var decom in decompressed)
                    output.Add(new AnimTrackCustomVector4(decom[0], decom[1], decom[2], decom[3]));
            }
            if (CheckFlag(Flags, 0x00FF, ANIM_TRACKFLAGS.Transform))
            {
                var decompressed = DecompressTransform(reader, DataOffset, Header);
                foreach (var v in decompressed)
                    output.Add(v);
            }

            return output.ToArray();
        }

        private List<float[]> DecompressValues(SSBHParser parser, uint DataOffset, SSBHAnimCompressedHeader Header, int ValueCount)
        {
            List<float[]> Transforms = new List<float[]>(Header.FrameCount);

            // PreProcess
            SSBHAnimCompressedItem[] Items = parser.ByteToType<SSBHAnimCompressedItem>(ValueCount);

            parser.Seek(DataOffset + Header.DefaultDataOffset);

            float[] DefaultValues = new float[ValueCount];
            for(int i = 0; i < ValueCount; i++)
            {
                DefaultValues[i] = parser.ReadSingle();
            }

            parser.Seek(DataOffset + Header.CompressedDataOffset);
            for (int frame = 0; frame < Header.FrameCount; frame++)
            {
                float[] Values = new float[ValueCount];
                for (int i = 0; i < ValueCount; i++)
                    Values[i] = DefaultValues[i];

                for (int itemIndex = 0; itemIndex < Items.Length; itemIndex++)
                {
                    var item = Items[itemIndex];

                    // Decompress
                    int ValueBitCount = (int)item.Count;
                    if (ValueBitCount == 0) continue;

                    int Value = parser.ReadBits(ValueBitCount);
                    int scale = 0;
                    for (int k = 0; k < ValueBitCount; k++)
                        scale |= 0x1 << k;

                    float FrameValue = Lerp(item.Start, item.End, 0, 1, Value / (float)scale);
                    if (float.IsNaN(FrameValue))
                        FrameValue = 0;

                    Values[itemIndex] = FrameValue;
                }

                Transforms[frame] = Values;
            }

            return Transforms;
        }

        private AnimTrackTransform[] DecompressTransform(SSBHParser parser, uint DataOffset, SSBHAnimCompressedHeader Header)
        {
            AnimTrackTransform[] Transforms = new AnimTrackTransform[Header.FrameCount];

            // PreProcess
            SSBHAnimCompressedItem[] Items = parser.ByteToType<SSBHAnimCompressedItem>(9);
            
            parser.Seek(DataOffset + Header.DefaultDataOffset);

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

            parser.Seek(DataOffset + Header.CompressedDataOffset);
            for(int frame = 0; frame < Header.FrameCount; frame++)
            {
                AnimTrackTransform Transform = new AnimTrackTransform()
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
                    SZ = ZSCA
                };
                for (int itemIndex = 0; itemIndex < Items.Length; itemIndex++)
                {
                    // First check if this track should be parsed
                    if (!((itemIndex >= 0 && itemIndex <= 0 && (Header.Flags & 0x3) == 0x3) //isotrophic scale
                                || (itemIndex >= 0 && itemIndex <= 2 && (Header.Flags & 0x3) == 0x1) //normal scale
                                || (itemIndex > 2 && itemIndex <= 5 && (Header.Flags & 0x4) > 0)
                                || (itemIndex > 5 && itemIndex <= 8 && (Header.Flags & 0x8) > 0)))
                    {
                        continue;
                    }

                    var item = Items[itemIndex];

                    // Decompress
                    int ValueBitCount = (int)item.Count;
                    if (ValueBitCount == 0) continue;

                    int Value = parser.ReadBits(ValueBitCount);
                    int scale = 0;
                    for (int k = 0; k < ValueBitCount; k++)
                        scale |= 0x1 << k;

                    float FrameValue = Lerp(item.Start, item.End, 0, 1, Value / (float)scale);
                    if (float.IsNaN(FrameValue))
                        FrameValue = 0;

                    // the Transform type relies a lot on flags

                    if ((Header.Flags & 0x3) == 0x3)
                    {
                        //Scale Isotropic
                        if(itemIndex == 0)
                        {
                            Transform.SX = FrameValue;
                            Transform.SY = FrameValue;
                            Transform.SZ = FrameValue;
                        }
                    }
                    if ((Header.Flags & 0x3) == 0x1)
                    {
                        //Scale normal
                        switch (itemIndex)
                        {
                            case 0:
                                Transform.SX = FrameValue;
                                break;
                            case 1:
                                Transform.SY = FrameValue;
                                break;
                            case 2:
                                Transform.SZ = FrameValue;
                                break;
                        }
                    }
                    //Rotation and Position
                    switch (itemIndex)
                    {
                        case 3:
                            Transform.RX = FrameValue;
                            break;
                        case 4:
                            Transform.RY = FrameValue;
                            break;
                        case 5:
                            Transform.RZ = FrameValue;
                            break;
                        case 6:
                            Transform.X = FrameValue;
                            break;
                        case 7:
                            Transform.Y = FrameValue;
                            break;
                        case 8:
                            Transform.Z = FrameValue;
                            break;
                    }
                }

                // Rotations have an extra bit at the end
                if ((Header.Flags & 0x4) > 0)
                {
                    bool Wflip = parser.ReadBits(1) == 1;

                    // W is calculated
                    Transform.RW = (float)Math.Sqrt(Math.Abs(1 - (Transform.RX * Transform.RX + Transform.RY * Transform.RY + Transform.RZ * Transform.RZ)));

                    if (Wflip)
                        Transform.RW = -Transform.RW;
                }

                Transforms[frame] = Transform;
            }

            return Transforms;
        }

        private object ReadDirect(SSBHParser reader, uint Flags)
        {
            if (CheckFlag(Flags, 0x00FF, ANIM_TRACKFLAGS.Transform))
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
                };
                reader.ReadInt32(); // ????

                return Transform;
            }
            if (CheckFlag(Flags, 0x00FF, ANIM_TRACKFLAGS.Texture))
            {
                // TODO: What type is this
            }
            if (CheckFlag(Flags, 0x00FF, ANIM_TRACKFLAGS.Float))
                return reader.ReadSingle();

            if (CheckFlag(Flags, 0x00FF, ANIM_TRACKFLAGS.PatternIndex))
            {
                //TODO: What type is this
            }

            if (CheckFlag(Flags, 0x00FF, ANIM_TRACKFLAGS.Boolean))
                return reader.ReadByte() == 1;

            if (CheckFlag(Flags, 0x00FF, ANIM_TRACKFLAGS.Vector4))
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
