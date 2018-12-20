using System;
using System.Collections.Generic;
using SSBHLib.Formats;
using SSBHLib.IO;
using System.IO;

namespace SSBHLib.Tools
{
    public class AnimTrackBool
    {
        public bool Value { get; set; } = false;
        public AnimTrackBool(bool Value)
        {
            this.Value = Value;
        }
    }

    public class AnimTrackTransform
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
    }

    public class AnimTrackCustomVector4
    {
        public float X;
        public float Y;
        public float Z;
        public float W;
    }

    public class SSBHAnimTrackDecoder
    {
        private ANIM AnimFile;

        public SSBHAnimTrackDecoder(ANIM AnimFile)
        {
            this.AnimFile = AnimFile;
        }

        public object[] ReadTrack(ANIM_Track Track)
        {
            Console.WriteLine(Track.Name + " " + Track.Flags.ToString("X") + " " + Track.FrameCount + " " + Track.DataOffset.ToString("X"));
            List<object> output = new List<object>();
            using (SSBHParser parser = new SSBHParser(new MemoryStream(AnimFile.Buffer)))
            {
                parser.Seek(Track.DataOffset);

                if (Track.Flags.HasFlag(ANIM_TRACKFLAGS.Visibilty))
                {
                    if ((int)Track.Flags == 0x0408)
                    {
                        int Unk_4 = parser.ReadInt16();
                        int TrackFlag = parser.ReadInt16();
                        int Unk1 = parser.ReadInt16();
                        int Unk2 = parser.ReadInt16();
                        int DataStart = parser.ReadInt32();
                        int FrameCount = parser.ReadInt32();

                        parser.Seek((int)Track.DataOffset + DataStart);
                        for (int i = 0; i < FrameCount; i++)
                        {
                            output.Add(new AnimTrackBool(parser.ReadBits(1) == 1));
                        }
                    }
                    else
                    if ((int)Track.Flags == 0x0508)
                    {
                        output.Add(new AnimTrackBool(parser.ReadBits(1) == 1));
                    }
                }
                if (Track.Flags.HasFlag(ANIM_TRACKFLAGS.Transform))
                {
                    if ((int)Track.Flags == 0x0401)
                    {
                        int Unk_4 = parser.ReadInt16();
                        int TrackFlag = parser.ReadInt16();
                        int Unk1 = parser.ReadInt16();
                        int Unk2 = parser.ReadInt16();
                        int DataStart = parser.ReadInt32();
                        int FrameCount = parser.ReadInt32();

                        int[] ByteCounts = new int[9];
                        int[] BitCounts = new int[9];
                        float[] Start = new float[9];
                        float[] End = new float[9];
                        for (int i = 0; i < 9; i++)
                        {
                            Start[i] = parser.ReadSingle();
                            End[i] = parser.ReadSingle();
                            long Count = parser.ReadInt64();
                            long bytes = (Count >> 3);
                            int bits = ((int)Count & 0x7);

                            if ((i >= 0 && i <= 0 && (TrackFlag & 0x3) == 0x3) //isotrophic scale
                                || (i >= 0 && i <= 2 && (TrackFlag & 0x3) == 0x1) //normal scale
                                || (i > 2 && i <= 5 && (TrackFlag & 0x4) > 0)
                                || (i > 5 && i <= 8 && (TrackFlag & 0x8) > 0))
                            {
                                //reads
                                {
                                    BitCounts[i] = bits;
                                    ByteCounts[i] = (int)bytes;
                                }
                            }
                        }

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

                        parser.ReadInt32(); // ????

                        parser.Seek((int)Track.DataOffset + DataStart);
                        for (int i = 0; i < FrameCount; i++)
                        {
                            AnimTrackTransform Transform = new AnimTrackTransform();
                            for (int j = 0; j < 9; j++)
                            {
                                int ValueBitCount = ByteCounts[j] * 8 + BitCounts[j];
                                int Value = parser.ReadBits(ValueBitCount);
                                int scale = 0;
                                for (int k = 0; k < ValueBitCount; k++)
                                    scale |= 0x1 << k;

                                float FrameValue = Lerp(Start[j], End[j], 0, 1, Value / (float)scale);
                                if (float.IsNaN(FrameValue))
                                    FrameValue = 0;
                                
                                if ((TrackFlag & 0x3) == 0x3)
                                {
                                    //Scale Isotropic
                                    if (j == 0)
                                    {
                                        Transform.SX = FrameValue;
                                        Transform.SY = FrameValue;
                                        Transform.SZ = FrameValue;
                                    }
                                }
                                else if ((TrackFlag & 0x3) == 0x1)
                                {
                                    //Scale normal
                                    switch (j)
                                    {
                                        case 0: 
                                            if (ValueBitCount > 0)
                                                Transform.SX = FrameValue;
                                            else
                                                Transform.SX = XSCA;
                                            break;
                                        case 1: 
                                            if (ValueBitCount > 0)
                                                Transform.SY = FrameValue;
                                            else
                                                Transform.SY = YSCA;
                                            break;
                                        case 2:
                                            if (ValueBitCount > 0)
                                                Transform.SZ = FrameValue;
                                            else
                                                Transform.SZ = ZSCA;
                                            break;
                                    }
                                }
                                else
                                {
                                    Transform.SX = XSCA;
                                    Transform.SY = YSCA;
                                    Transform.SZ = ZSCA;
                                }

                                //Rotation
                                if ((TrackFlag & 0x4) > 0)
                                {
                                    switch (j)
                                    {
                                        case 3:
                                            if (ValueBitCount > 0)
                                                Transform.RX = FrameValue;
                                            else
                                                Transform.RX = XROT;
                                            break;
                                        case 4:
                                            if (ValueBitCount > 0)
                                                Transform.RY = FrameValue;
                                            else
                                                Transform.RY = YROT;
                                            break;
                                        case 5:
                                            if (ValueBitCount > 0)
                                                Transform.RZ = FrameValue;
                                            else
                                                Transform.RZ = ZROT;
                                            break;
                                    }
                                }
                                else
                                {
                                    Transform.RX = XROT;
                                    Transform.RY = YROT;
                                    Transform.RZ = ZROT;
                                    Transform.RW = WROT;
                                }

                                // Position
                                if ((TrackFlag & 0x8) > 0)
                                {
                                    switch (j)
                                    {
                                        case 6:
                                            if (ValueBitCount > 0)
                                                Transform.X = FrameValue;
                                            else
                                                Transform.X = XPOS;
                                            break;
                                        case 7:
                                            if (ValueBitCount > 0)
                                                Transform.Y = FrameValue;
                                            else
                                                Transform.Y = YPOS;
                                            break;
                                        case 8:
                                            if (ValueBitCount > 0)
                                                Transform.Z = FrameValue;
                                            else
                                                Transform.Z = ZPOS;
                                            break;
                                    }
                                }
                                else
                                {
                                    Transform.X = XPOS;
                                    Transform.Y = YPOS;
                                    Transform.Z = ZPOS;
                                }
                            }
                            if ((TrackFlag & 0x4) > 0)
                            {
                                // Rotation w
                                bool Wflip = parser.ReadBits(1) == 1;// (TrackFlag & 0x1) == 0 ? parser.ReadBits(1) == 1 : true;

                                Transform.RW = (float)Math.Sqrt(Math.Abs(1 - (Transform.RX * Transform.RX + Transform.RY * Transform.RY + Transform.RZ * Transform.RZ)));

                                if (Wflip)
                                    Transform.RW = -Transform.RW;
                            }
                            
                            output.Add(Transform);
                        }

                    }
                    else if ((int)Track.Flags == 0x0201)
                    {
                        output.Add(new AnimTrackTransform()
                        {
                            SX = parser.ReadSingle(),
                            SY = parser.ReadSingle(),
                            SZ = parser.ReadSingle(),
                            RX = parser.ReadSingle(),
                            RY = parser.ReadSingle(),
                            RZ = parser.ReadSingle(),
                            RW = parser.ReadSingle(),
                            X = parser.ReadSingle(),
                            Y = parser.ReadSingle(),
                            Z = parser.ReadSingle(),
                        });

                        parser.ReadInt32(); // ????
                    }
                }
            }

            return output.ToArray();
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
