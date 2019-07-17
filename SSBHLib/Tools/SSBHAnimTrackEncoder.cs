using SSBHLib.Formats.Animation;
using System;
using System.Collections.Generic;
using System.IO;

namespace SSBHLib.Tools
{
    /// <summary>
    /// Encoder for ANIM track data
    /// </summary>
    public class SSBHAnimTrackEncoder
    {
        public SSBHAnimTrackEncoder(float frameCount)
        {
            animFile.FrameCount = frameCount;
        }

        private List<AnimGroup> groups = new List<AnimGroup>();
        private Dictionary<AnimTrack, IList<object>> trackToValues = new Dictionary<AnimTrack, IList<object>>();

        private ANIM animFile = new ANIM();

        private static float DefaultEpsilon = 0.000002f; //0.0000012f;
        private float Epsilon = DefaultEpsilon;

        public bool CompressVector4 { get; set; } = false;

        /// <summary>
        /// Sets the margin of error for compression
        /// default is 0.000002 smash ultimate seems to be closer to 0.0000012
        /// smaller value means smaller filesize but less accuracy
        /// </summary>
        /// <param name="value"></param>
        public void SetCompressionLevel(float value)
        {
            Epsilon = value;
        }

        /// <summary>
        /// Adds a track to be encoded
        /// </summary>
        /// <param name="NodeName">target material/texture/mesh name</param>
        /// <param name="TrackName">Usually "Transform" or "Visibility" matches <see cref="ANIM_TYPE"/></param>
        /// <param name="Type"><see cref="ANIM_TYPE"/></param>
        /// <param name="Values">Supported types AnimTrackTransform, AnimTrackTexture, AnimTrackCustomVector4, bool, float, int</param>
        public void AddTrack(string nodeName, string trackName, ANIM_TYPE type, IList<object> values)
        {
            AnimNode node = GetNode(type, nodeName);

            AnimTrack track = new AnimTrack
            {
                FrameCount = (uint)values.Count,
                Name = trackName
            };

            var tracks = node.Tracks;
            Array.Resize(ref tracks, tracks.Length + 1);
            tracks[tracks.Length - 1] = track;
            node.Tracks = tracks;

            trackToValues.Add(track, values);
        }

        /// <summary>
        /// Generates and saves the new ANIM file
        /// </summary>
        /// <param name="fname"></param>
        public void Save(string fname)
        {
            // Prep Anim File
            animFile.Name = Path.GetFileName(fname);
            animFile.Animations = groups.ToArray();

            // Create Buffer
            MemoryStream buffer = new MemoryStream();
            
            using (BinaryWriter w = new BinaryWriter(buffer))
            {
                foreach (var animation in animFile.Animations)
                {
                    foreach (var node in animation.Nodes)
                    {
                        foreach(var track in node.Tracks) // TODO: these nodes need to be in ordinal order
                        {
                            var values = trackToValues[track];

                            track.DataOffset = (uint)buffer.Position;

                            track.FrameCount = (uint)values.Count;

                            if (values.Count > 0)
                                track.Flags |= (uint)GetTrackTypeFlag(values[0]);

                            if (values.Count == 1)
                            {
                                if (animation.Type == ANIM_TYPE.Transform)
                                    track.Flags |= (int)ANIM_TRACKFLAGS.ConstTransform;
                                else
                                    track.Flags |= (int)ANIM_TRACKFLAGS.Constant;

                                track.FrameCount = 1;

                                WriteValues(w, (int)track.Flags, values);
                            }
                            else
                            {
                                if (WriteValues(w, (int)track.Flags, values))
                                    track.Flags |= (int)ANIM_TRACKFLAGS.Compressed;
                                else
                                    track.Flags |= (int)ANIM_TRACKFLAGS.Direct;
                            }

                            track.DataSize = (uint)(buffer.Position - track.DataOffset);

                            int padding = (int)(0x40 - (w.BaseStream.Position % 0x40));
                            w.Write(new byte[padding]);

                            //Console.WriteLine(w.BaseStream.Position.ToString("X") + " " + values.Count + " " + track.Flags.ToString("X"));
                        }
                    }
                }
            }

            animFile.Buffer = buffer.ToArray();
            buffer.Close();
            buffer.Dispose();

            SSBH.TrySaveSSBHFile(fname, animFile);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private AnimGroup GetGroup(ANIM_TYPE type)
        {
            foreach (var g in groups)
            {
                if (g.Type == type)
                    return g;
            }
            AnimGroup group = new AnimGroup();
            group.Type = type;
            group.Nodes = new AnimNode[0];
            groups.Add(group);
            return group;
        }

        /// <summary>
        /// Returns the track flag for object type
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private int GetTrackTypeFlag(object o)
        {
            if (o is AnimTrackTransform transform)
            {
                return (int)ANIM_TRACKFLAGS.Transform;
            }
            else if (o is AnimTrackCustomVector4 vector)
            {
                return (int)ANIM_TRACKFLAGS.Vector4;
            }
            else if (o is AnimTrackTexture tex)
            {
                return (int)ANIM_TRACKFLAGS.Texture;
            }
            else if (o is bool b)
            {
                return (int)ANIM_TRACKFLAGS.Boolean;
            }
            else if (o is float f)
            {
                return (int)ANIM_TRACKFLAGS.Float;
            }
            else
            if (o is int)
            {
                return (int)ANIM_TRACKFLAGS.PatternIndex;
            }
            return 0;
            /*
        Transform = 0x0001,
        Texture = 0x0002,
        Float = 0x0003,
        PatternIndex = 0x0005,
        Boolean = 0x0008,
        Vector4 = 0x0009,
             * */
        }

        /// <summary>
        /// returns true if values were compressed
        /// </summary>
        /// <param name="w"></param>
        /// <param name="flags"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private bool WriteValues(BinaryWriter w, int flags, IList<object> values)
        {
            // empty track
            if (values.Count == 0)
                return false;

            // if values count is 1 then just write direct
            if (values.Count == 1)
            {
                WriteDirect(w, values[0]);
                return false;
            }

            // otherwise try to compress and compress if possible
            if (values[0] is AnimTrackTransform)
            {
                CompressTransformTracks(w, values);
                return true;
            }
            if (values[0] is AnimTrackCustomVector4 && CompressVector4)
            {
                CompressVectorTrack(w, values);
                return true;
            }
            if (values[0] is bool)
            {
                CompressBooleanTrack(w, values);
                return true;
            }

            // not compressed
            foreach (var v in values)
                WriteDirect(w, v);

            return false;
        }

        /// <summary>
        /// Special helper class for quantanizing the track values
        /// </summary>
        private class Quantanizer
        {
            public float Min { get; private set; } = float.MaxValue;
            public float Max { get; private set; } = float.MinValue;

            public bool Constant => Min == Max;

            public int QuantanizationValue
            {
                get
                {
                    int v = 0;
                    for (int i = 0; i < bitCount; i++)
                        v |= (1 << i);
                    return v;
                }
            }
            private List<float> values = new List<float>();

            // cache bitcount
            private float currentError = float.MaxValue;
            private int bitCount;

            public void Add(float v)
            {
                Max = Math.Max(v, Max);
                Min = Math.Min(v, Min);
                values.Add(v);
                currentError = float.MaxValue;
            }

            /// <summary>
            /// Returns the number of bits within given margin of error
            /// </summary>
            /// <param name="epsilon"></param>
            /// <returns></returns>
            public int GetBitCount(float epsilon)
            {
                if (Constant)
                    return 0x10;

                if (epsilon == currentError)
                    return bitCount;

                currentError = epsilon;

                // try to find an optimal bit length
                for (var i = 1; i < 31; i++)
                {
                    bitCount = i;
                    float error = ComputeError(i);
                    if (error < epsilon)
                    {
                        return i;
                    }
                }

                bitCount = 0;
                return -1;
            }


            /// <summary>
            /// Returns the maximum error for given bit length
            /// </summary>
            /// <param name="bits"></param>
            /// <returns></returns>
            private float ComputeError(int bits)
            {
                float epsilon = 0;

                if (Constant)
                    return epsilon;

                foreach (var v in values)
                {
                    epsilon = Math.Max(epsilon, v - DecompressedValue(v));
                }

                return epsilon;
            }

            /// <summary>
            /// Encodes value with bit compressed version
            /// </summary>
            /// <param name="v"></param>
            /// <returns></returns>
            public int GetQuantanizedValue(float v)
            {
                float quan = (v - Min) / (Max - Min);
                int quantanized = (int)(quan * QuantanizationValue);
                return quantanized;
            }

            /// <summary>
            /// Gets the encoded then decoded value
            /// </summary>
            /// <param name="v"></param>
            /// <returns></returns>
            public float DecompressedValue(float v)
            {
                var value = SSBHAnimTrackDecoder.Lerp(Min, Max, 0, 1, GetQuantanizedValue(v) / (float)QuantanizationValue);
                if (float.IsNaN(value))
                    return 0;
                return value;
            }
        }

        /// <summary>
        /// Special class for writing bits
        /// </summary>
        private class BitWriter
        {
            private int currentByte = 0;
            private int currentBit = 0;
            private List<byte> output = new List<byte>();

            public void WriteBits(int value, int bitCount)
            {
                for (int i = 0; i < bitCount; i++)
                {
                    int bit = (value >> i) & 0x1;
                    currentByte |= (bit << currentBit);
                    currentBit++;
                    if (currentBit > 7)
                    {
                        output.Add((byte)currentByte);
                        currentBit = 0;
                        currentByte = 0;
                    }
                }
            }

            public byte[] GetBytes()
            {
                byte[] o = new byte[output.Count + 1];
                for (int i = 0; i < output.Count; i++)
                    o[i] = output[i];
                o[o.Length - 1] = (byte)currentByte;
                return o;
            }
        }

        /// <summary>
        /// Compresses and writes booleans to writer
        /// </summary>
        /// <param name="w"></param>
        /// <param name="values"></param>
        private void CompressBooleanTrack(BinaryWriter w, IList<object> values)
        {
            // Compressed Header
            w.Write((short)0x04);
            w.Write((short)0);
            w.Write((short)0x20); // default values offset
            w.Write((short)1); // bits per entry
            w.Write(0x21); // compressed data start
            w.Write(values.Count); // frame count

            w.Write(0); // all 0s for booleans
            w.Write(0);
            w.Write(0);
            w.Write(0);

            BitWriter bitWriter = new BitWriter();
            foreach (bool b in values)
                bitWriter.WriteBits(b ? 1 : 0, 1);
            w.Write(bitWriter.GetBytes());
        }

        /// <summary>
        /// Compressed vector track and writes it to writer
        /// </summary>
        /// <param name="w"></param>
        /// <param name="values"></param>
        private void CompressVectorTrack(BinaryWriter w, IList<object> values)
        {
            // Process
            short Flags = 0;
            short BitsPerEntry = 0;

            Quantanizer v1 = new Quantanizer();
            Quantanizer v2 = new Quantanizer();
            Quantanizer v3 = new Quantanizer();
            Quantanizer v4 = new Quantanizer();

            foreach(AnimTrackCustomVector4 vec in values)
            {
                v1.Add(vec.X);
                v2.Add(vec.Y);
                v3.Add(vec.Z);
                v4.Add(vec.W);
            }

            BitsPerEntry = (short)(v1.GetBitCount(Epsilon) + v2.GetBitCount(Epsilon) + v3.GetBitCount(Epsilon) + v4.GetBitCount(Epsilon));

            //TODO: this is bugged
            // Write Compressed Header
            w.Write((short)0x04);
            w.Write(Flags);
            w.Write((short)(0x10 + 0x10 * 4)); // default values offset
            w.Write(BitsPerEntry);
            w.Write(0x10 + 0x10 * 4 + sizeof(float) * 4); // compressed data start
            w.Write(values.Count); // frame count

            // table
            w.Write(v1.Min); w.Write(v1.Max); w.Write(v1.GetBitCount(Epsilon));
            w.Write(v2.Min); w.Write(v2.Max); w.Write(v2.GetBitCount(Epsilon));
            w.Write(v3.Min); w.Write(v3.Max); w.Write(v3.GetBitCount(Epsilon));
            w.Write(v4.Min); w.Write(v4.Max); w.Write(v4.GetBitCount(Epsilon));

            // default values
            w.Write(((AnimTrackCustomVector4)values[0]).X);
            w.Write(((AnimTrackCustomVector4)values[0]).Y);
            w.Write(((AnimTrackCustomVector4)values[0]).Z);
            w.Write(((AnimTrackCustomVector4)values[0]).W);

            // compressed data
            BitWriter bitWriter = new BitWriter();
            foreach (AnimTrackCustomVector4 vec in values)
            {
                bitWriter.WriteBits(v1.GetQuantanizedValue(vec.X), v1.GetBitCount(Epsilon));
                bitWriter.WriteBits(v2.GetQuantanizedValue(vec.Y), v2.GetBitCount(Epsilon));
                bitWriter.WriteBits(v3.GetQuantanizedValue(vec.Z), v3.GetBitCount(Epsilon));
                bitWriter.WriteBits(v4.GetQuantanizedValue(vec.W), v4.GetBitCount(Epsilon));
            }
            w.Write(bitWriter.GetBytes());
        }

        /// <summary>
        /// Compresses <see cref="AnimTrackTransform"></see> track and writes to writer
        /// </summary>
        /// <param name="w"></param>
        /// <param name="values"></param>
        private void CompressTransformTracks(BinaryWriter w, IList<object> values)
        {
            Quantanizer SX = new Quantanizer();
            Quantanizer SY = new Quantanizer();
            Quantanizer SZ = new Quantanizer();
            Quantanizer RX = new Quantanizer();
            Quantanizer RY = new Quantanizer();
            Quantanizer RZ = new Quantanizer();
            Quantanizer X = new Quantanizer();
            Quantanizer Y = new Quantanizer();
            Quantanizer Z = new Quantanizer();

            // pre-process
            foreach (AnimTrackTransform transform in values)
            {
                SX.Add(transform.SX);
                SY.Add(transform.SY);
                SZ.Add(transform.SZ);
                RX.Add(transform.RX);
                RY.Add(transform.RY);
                RZ.Add(transform.RZ);
                X.Add(transform.X);
                Y.Add(transform.Y);
                Z.Add(transform.Z);
            }

            short Flags = 0;
            ushort BitsPerEntry = 0;

            bool hasScale = (!SX.Constant || !SY.Constant || !SZ.Constant);
            bool hasRotation = (!RX.Constant || !RY.Constant || !RZ.Constant);
            bool hasPosition = (!X.Constant || !Y.Constant || !Z.Constant);


            if (!hasScale)
                Flags |= 0x02;
            else
            {
                BitsPerEntry += (ushort)((SX.Constant ? 0 : SX.GetBitCount(Epsilon)) + (SY.Constant ? 0 : SY.GetBitCount(Epsilon)) + (SZ.Constant ? 0 : SZ.GetBitCount(Epsilon)));
                Flags |= 0x01;
            }
            if (hasRotation)
            {
                BitsPerEntry += (ushort)((RX.Constant ? 0 : RX.GetBitCount(Epsilon)) + (RY.Constant ? 0 : RY.GetBitCount(Epsilon)) + (RZ.Constant ? 0 : RZ.GetBitCount(Epsilon)) + 1); // the 1 is for extra w rotation bit
                Flags |= 0x04;
            }
            if (hasPosition)
            {
                BitsPerEntry += (ushort)((X.Constant ? 0 : X.GetBitCount(Epsilon)) + (Y.Constant ? 0 : Y.GetBitCount(Epsilon)) + (Z.Constant ? 0 : Z.GetBitCount(Epsilon)));
                Flags |= 0x08;
            }

            // Compressed Header
            w.Write((short)0x04);
            w.Write(Flags);
            w.Write((short)(0x10 + 0x10 * 9)); // default values offset
            w.Write(BitsPerEntry);
            w.Write(0x10 + 0x10 * 9 + sizeof(float) * 11); // compressed data start
            w.Write(values.Count); // frame count

            // write chunks
            w.Write(SX.Min); w.Write(SX.Max); w.Write((long)SX.GetBitCount(Epsilon));
            w.Write(SY.Min); w.Write(SY.Max); w.Write((long)SY.GetBitCount(Epsilon));
            w.Write(SZ.Min); w.Write(SZ.Max); w.Write((long)SZ.GetBitCount(Epsilon));
            w.Write(RX.Min); w.Write(RX.Max); w.Write((long)RX.GetBitCount(Epsilon));
            w.Write(RY.Min); w.Write(RY.Max); w.Write((long)RY.GetBitCount(Epsilon));
            w.Write(RZ.Min); w.Write(RZ.Max); w.Write((long)RZ.GetBitCount(Epsilon));
            w.Write(X.Min); w.Write(X.Max); w.Write((long)X.GetBitCount(Epsilon));
            w.Write(Y.Min); w.Write(Y.Max); w.Write((long)Y.GetBitCount(Epsilon));
            w.Write(Z.Min); w.Write(Z.Max); w.Write((long)Z.GetBitCount(Epsilon));

            // write default values
            AnimTrackTransform defaultValue = (AnimTrackTransform)values[0];
            w.Write(defaultValue.SX);
            w.Write(defaultValue.SY);
            w.Write(defaultValue.SZ);
            w.Write(defaultValue.RX);
            w.Write(defaultValue.RY);
            w.Write(defaultValue.RZ);
            w.Write(defaultValue.RW);
            w.Write(defaultValue.X);
            w.Write(defaultValue.Y);
            w.Write(defaultValue.Z);
            w.Write(0);

            // write compressed values
            BitWriter writer = new BitWriter();

            foreach (AnimTrackTransform transform in values)
            {
                if (hasScale)
                {
                    writer.WriteBits(SX.GetQuantanizedValue(transform.SX), SX.GetBitCount(Epsilon));
                    writer.WriteBits(SY.GetQuantanizedValue(transform.SY), SY.GetBitCount(Epsilon));
                    writer.WriteBits(SZ.GetQuantanizedValue(transform.SZ), SZ.GetBitCount(Epsilon));
                }
                if (hasRotation)
                {
                    writer.WriteBits(RX.GetQuantanizedValue(transform.RX), RX.GetBitCount(Epsilon));
                    writer.WriteBits(RY.GetQuantanizedValue(transform.RY), RY.GetBitCount(Epsilon));
                    writer.WriteBits(RZ.GetQuantanizedValue(transform.RZ), RZ.GetBitCount(Epsilon));
                }
                if (hasPosition)
                {
                    writer.WriteBits(X.GetQuantanizedValue(transform.X), X.GetBitCount(Epsilon));
                    writer.WriteBits(Y.GetQuantanizedValue(transform.Y), Y.GetBitCount(Epsilon));
                    writer.WriteBits(Z.GetQuantanizedValue(transform.Z), Z.GetBitCount(Epsilon));
                }
                if (hasRotation)
                {
                    // flip w bit

                    float calculateW = (float)Math.Sqrt(Math.Abs(1 - (
                        RX.DecompressedValue(transform.RX) * RX.DecompressedValue(transform.RX) +
                        RY.DecompressedValue(transform.RY) * RY.DecompressedValue(transform.RY) +
                        RZ.DecompressedValue(transform.RZ) * RZ.DecompressedValue(transform.RZ))));

                    writer.WriteBits(Math.Sign((int)transform.RW) != Math.Sign((int)calculateW) ? 1 : 0, 1);
                }
            }

            w.Write(writer.GetBytes());
        }

        /// <summary>
        /// Writes the object type directly with no compression
        /// </summary>
        /// <param name="w"></param>
        /// <param name="o"></param>
        private void WriteDirect(BinaryWriter w, object o)
        {
            if (o is AnimTrackTransform transform)
            {
                w.Write(transform.SX);
                w.Write(transform.SY);
                w.Write(transform.SZ);
                w.Write(transform.RX);
                w.Write(transform.RY);
                w.Write(transform.RZ);
                w.Write(transform.RW);
                w.Write(transform.X);
                w.Write(transform.Y);
                w.Write(transform.Z);
                w.Write((float)0);
            }
            else if (o is AnimTrackCustomVector4 vector)
            {
                w.Write(vector.X);
                w.Write(vector.Y);
                w.Write(vector.Z);
                w.Write(vector.W);
            }
            else if (o is AnimTrackTexture tex)
            {
                w.Write(tex.UnkFloat1);
                w.Write(tex.UnkFloat2);
                w.Write(tex.UnkFloat3);
                w.Write(tex.UnkFloat4);
                w.Write(tex.Unknown);
            }
            else if (o is bool b)
            {
                w.Write(b);
            }
            else if (o is float f)
            {
                w.Write(f);
            }
            else
            if (o is int i)
            {
                w.Write(i);
            }
            else
                throw new NotSupportedException($"{o.GetType()} is not a supported track object");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="NodeName"></param>
        /// <returns></returns>
        private AnimNode GetNode(ANIM_TYPE type, string nodeName)
        {
            AnimGroup group = GetGroup(type);

            foreach (var node in group.Nodes)
            {
                if (node.Name.Equals(nodeName))
                {
                    return node;
                }
            }

            var newnode = new AnimNode
            {
                Name = nodeName,
                Tracks = new AnimTrack[0]
            };

            var nodes = group.Nodes;
            Array.Resize(ref nodes, nodes.Length + 1);
            nodes[nodes.Length - 1] = newnode;
            group.Nodes = nodes;

            return newnode;
        }
    }
}
