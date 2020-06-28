using System;
using System.Collections.Generic;
using SSBHLib.Formats.Animation;
using SSBHLib.IO;
using System.IO;
using System.Runtime.InteropServices;

namespace SSBHLib.Tools
{
    /// <summary>
    /// Decoder for the ANIM track data
    /// </summary>
    public class SsbhAnimTrackDecoder
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SsbhAnimCompressedHeader
        {
            public ushort Unk_4; // always 4?
            public ushort Flags;
            public ushort DefaultDataOffset;
            public ushort BitsPerEntry;
            public int CompressedDataOffset;
            public int FrameCount;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SsbhAnimCompressedItem
        {
            public float Start;
            public float End;
            public ulong Count;
        }

        private readonly Anim animFile;

        /// <summary>
        /// Creates decoder for given ANIM file
        /// </summary>
        /// <param name="animFile"></param>
        public SsbhAnimTrackDecoder(Anim animFile)
        {
            this.animFile = animFile;
        }

        /// <summary>
        /// return true if given flag is set
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="mask"></param>
        /// <param name="expectedValue"></param>
        /// <returns></returns>
        private bool CheckFlag(uint flags, uint mask, AnimTrackFlags expectedValue)
        {
            return (flags & mask) == (uint)expectedValue;
        }

        /// <summary>
        /// Reads the data out of the given track.
        /// </summary>
        /// <param name="track"></param>
        /// <returns></returns>
        public object[] ReadTrack(AnimTrack track)
        {
            //Console.WriteLine(Track.Name + " " + Track.Flags.ToString("X") + " " + Track.FrameCount + " " + Track.DataOffset.ToString("X"));
            List<object> output = new List<object>();
            using (SsbhParser parser = new SsbhParser(new MemoryStream(animFile.Buffer)))
            {
                parser.Seek(track.DataOffset);

                if (CheckFlag(track.Flags, 0xFF00, AnimTrackFlags.Constant))
                {
                    output.Add(ReadDirect(parser, track.Flags));
                }
                if (CheckFlag(track.Flags, 0xFF00, AnimTrackFlags.ConstTransform))
                {
                    // TODO: investigate more
                    output.Add(ReadDirect(parser, track.Flags));
                }
                if (CheckFlag(track.Flags, 0xFF00, AnimTrackFlags.Direct))
                {
                    for(int i = 0; i < track.FrameCount; i++)
                        output.Add(ReadDirect(parser, track.Flags));
                }
                if (CheckFlag(track.Flags, 0xFF00, AnimTrackFlags.Compressed))
                {
                    output.AddRange(ReadCompressed(parser, track.Flags));
                }
            }

            return output.ToArray();
        }

        /// <summary>
        /// Reads the data from a compressed track
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        private object[] ReadCompressed(SsbhParser reader, uint flags)
        {
            List<object> output = new List<object>();

            uint dataOffset = (uint)reader.BaseStream.Position;
            SsbhAnimCompressedHeader header = reader.ReadStruct<SsbhAnimCompressedHeader>();

            if (CheckFlag(flags, 0x00FF, AnimTrackFlags.Boolean))
            {
                ReadBooleans(reader, output, dataOffset, header);
            }
            if (CheckFlag(flags, 0x00FF, AnimTrackFlags.Texture))
            {
                // TODO: What type is this
            }
            if (CheckFlag(flags, 0x00FF, AnimTrackFlags.Float))
            {
                //TODO: What type is this
            }
            if (CheckFlag(flags, 0x00FF, AnimTrackFlags.PatternIndex))
            {
                //TODO: What type is this
            }
            if (CheckFlag(flags, 0x00FF, AnimTrackFlags.Vector4))
            {
                ReadVector4(reader, output, dataOffset, header);
            }
            if (CheckFlag(flags, 0x00FF, AnimTrackFlags.Transform))
            {
                ReadTransform(reader, output, dataOffset, header);
            }

            return output.ToArray();
        }

        private void ReadTransform(SsbhParser reader, List<object> output, uint dataOffset, SsbhAnimCompressedHeader header)
        {
            var decompressed = DecompressTransform(reader, dataOffset, header);
            foreach (var v in decompressed)
                output.Add(v);
        }

        private void ReadVector4(SsbhParser reader, List<object> output, uint dataOffset, SsbhAnimCompressedHeader header)
        {
            var decompressed = DecompressValues(reader, dataOffset, header, 4);
            foreach (var decom in decompressed)
                output.Add(new AnimTrackCustomVector4(decom[0], decom[1], decom[2], decom[3]));
        }

        private static void ReadBooleans(SsbhParser reader, List<object> output, uint dataOffset, SsbhAnimCompressedHeader header)
        {
            reader.Seek((int)dataOffset + header.CompressedDataOffset);
            // note: there is a section for "default" and "compressed items" but they seem to always be 0-ed out

            for (int i = 0; i < header.FrameCount; i++)
            {
                output.Add(reader.ReadBits(header.BitsPerEntry) == 1);
            }
        }

        /// <summary>
        /// Decompresses values in a track
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="dataOffset"></param>
        /// <param name="header"></param>
        /// <param name="valueCount"></param>
        /// <returns></returns>
        private List<float[]> DecompressValues(SsbhParser parser, uint dataOffset, SsbhAnimCompressedHeader header, int valueCount)
        {
            List<float[]> transforms = new List<float[]>(header.FrameCount);

            // PreProcess
            SsbhAnimCompressedItem[] items = parser.ReadStructs<SsbhAnimCompressedItem>(valueCount);

            parser.Seek(dataOffset + header.DefaultDataOffset);

            float[] defaultValues = GetDefaultValues(parser, valueCount);

            parser.Seek(dataOffset + header.CompressedDataOffset);
            for (int frameIndex = 0; frameIndex < header.FrameCount; frameIndex++)
            {
                // Copy default values.
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

        private static float[] GetDefaultValues(SsbhParser parser, int valueCount)
        {
            float[] defaultValues = new float[valueCount];
            for (int i = 0; i < valueCount; i++)
            {
                defaultValues[i] = parser.ReadSingle();
            }

            return defaultValues;
        }

        /// <summary>
        /// decompresses transform values from a track
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="dataOffset"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        private AnimTrackTransform[] DecompressTransform(SsbhParser parser, uint dataOffset, SsbhAnimCompressedHeader header)
        {
            AnimTrackTransform[] transforms = new AnimTrackTransform[header.FrameCount];

            // PreProcess
            SsbhAnimCompressedItem[] items = parser.ReadStructs<SsbhAnimCompressedItem>(9);
            
            parser.Seek(dataOffset + header.DefaultDataOffset);

            float xsca = parser.ReadSingle();
            float ysca = parser.ReadSingle();
            float zsca = parser.ReadSingle();
            float xrot = parser.ReadSingle();
            float yrot = parser.ReadSingle();
            float zrot = parser.ReadSingle();
            float wrot = parser.ReadSingle();
            float xpos = parser.ReadSingle();
            float ypos = parser.ReadSingle();
            float zpos = parser.ReadSingle();
            float csca = parser.ReadSingle();

            parser.Seek(dataOffset + header.CompressedDataOffset);
            for(int frame = 0; frame < header.FrameCount; frame++)
            {
                AnimTrackTransform transform = new AnimTrackTransform()
                {
                    X = xpos,
                    Y = ypos,
                    Z = zpos,
                    Rx = xrot,
                    Ry = yrot,
                    Rz = zrot,
                    Rw = wrot,
                    Sx = xsca,
                    Sy = ysca,
                    Sz = zsca,
                    CompensateScale = csca
                };
                for (int itemIndex = 0; itemIndex < items.Length; itemIndex++)
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

                    // the Transform type relies a lot on flags
                    
                    if ((header.Flags & 0x3) == 0x3)
                    {
                        //Scale Compensate
                        if(itemIndex == 0)
                        {
                            transform.CompensateScale = frameValue;
                        }
                    }
                    if ((header.Flags & 0x3) == 0x1)
                    {
                        //Scale normal
                        switch (itemIndex)
                        {
                            case 0:
                                transform.Sx = frameValue;
                                break;
                            case 1:
                                transform.Sy = frameValue;
                                break;
                            case 2:
                                transform.Sz = frameValue;
                                break;
                        }
                    }
                    //Rotation and Position
                    switch (itemIndex)
                    {
                        case 3:
                            transform.Rx = frameValue;
                            break;
                        case 4:
                            transform.Ry = frameValue;
                            break;
                        case 5:
                            transform.Rz = frameValue;
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
                    transform.Rw = (float)Math.Sqrt(Math.Abs(1 - (transform.Rx * transform.Rx + transform.Ry * transform.Ry + transform.Rz * transform.Rz)));

                    if (wFlip)
                        transform.Rw = -transform.Rw;
                }

                transforms[frame] = transform;
            }

            return transforms;
        }

        /// <summary>
        /// Reads direct information from track
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        private object ReadDirect(SsbhParser reader, uint flags)
        {
            if (CheckFlag(flags, 0x00FF, AnimTrackFlags.Transform))
            {
                var transform = new AnimTrackTransform()
                {
                    Sx = reader.ReadSingle(),
                    Sy = reader.ReadSingle(),
                    Sz = reader.ReadSingle(),
                    Rx = reader.ReadSingle(),
                    Ry = reader.ReadSingle(),
                    Rz = reader.ReadSingle(),
                    Rw = reader.ReadSingle(),
                    X = reader.ReadSingle(),
                    Y = reader.ReadSingle(),
                    Z = reader.ReadSingle(),
                    CompensateScale = reader.ReadInt32()
                };

                return transform;
            }

            if (CheckFlag(flags, 0x00FF, AnimTrackFlags.Texture))
            {
                return new AnimTrackTexture()
                {
                    UnkFloat1 = reader.ReadSingle(),
                    UnkFloat2 = reader.ReadSingle(),
                    UnkFloat3 = reader.ReadSingle(),
                    UnkFloat4 = reader.ReadSingle(),
                    Unknown = reader.ReadInt32()
                };
            }

            if (CheckFlag(flags, 0x00FF, AnimTrackFlags.Float))
                return reader.ReadSingle();

            if (CheckFlag(flags, 0x00FF, AnimTrackFlags.PatternIndex))
            {
                return reader.ReadInt32();
            }

            if (CheckFlag(flags, 0x00FF, AnimTrackFlags.Boolean))
                return reader.ReadByte() == 1;

            if (CheckFlag(flags, 0x00FF, AnimTrackFlags.Vector4))
                return new AnimTrackCustomVector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            return null;
        }

        /// <summary>
        /// A standard linear interpolation function
        /// </summary>
        /// <param name="av"></param>
        /// <param name="bv"></param>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float Lerp(float av, float bv, float v0, float v1, float t)
        {
            if (v0 == v1) return av;

            if (t == v0) return av;
            if (t == v1) return bv;
            
            float mu = (t - v0) / (v1 - v0);
            float value = ((av * (1 - mu)) + (bv * mu));
            if (float.IsNaN(value))
                return 0;
            return value;
        }
    }
}
