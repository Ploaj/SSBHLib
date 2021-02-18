using SSBHLib.Formats.Animation;
using System;
using System.Collections.Generic;
using System.IO;

namespace SSBHLib.Tools
{
    /// <summary>
    /// Encoder for ANIM track data
    /// </summary>
    public class SsbhAnimTrackEncoder
    {
        public SsbhAnimTrackEncoder(float finalFrameIndex)
        {
            animFile.FinalFrameIndex = finalFrameIndex;
        }

        private readonly List<AnimGroup> groups = new List<AnimGroup>();
        private readonly Dictionary<AnimTrack, IList<object>> trackToValues = new Dictionary<AnimTrack, IList<object>>();

        private readonly Anim animFile = new Anim();

        private static readonly float defaultEpsilon = 0.000002f; //0.0000012f;
        private float epsilon = defaultEpsilon;

        public bool CompressVector4 { get; set; } = false;

        /// <summary>
        /// Sets the margin of error for compression
        /// default is 0.000002 smash ultimate seems to be closer to 0.0000012
        /// smaller value means smaller filesize but less accuracy
        /// </summary>
        /// <param name="value"></param>
        public void SetCompressionLevel(float value)
        {
            epsilon = value;
        }

        /// <summary>
        /// Adds a track to be encoded
        /// </summary>
        /// <param name="NodeName">target material/texture/mesh name</param>
        /// <param name="TrackName">Usually "Transform" or "Visibility" matches <see cref="AnimType"/></param>
        /// <param name="Type"><see cref="AnimType"/></param>
        /// <param name="Values">Supported types AnimTrackTransform, AnimTrackTexture, AnimTrackCustomVector4, bool, float, int</param>
        public void AddTrack(string nodeName, string trackName, AnimType type, IList<object> values)
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
                        foreach (var track in node.Tracks) // TODO: these nodes need to be in ordinal order
                        {
                            var values = trackToValues[track];

                            track.DataOffset = (uint)buffer.Position;

                            track.FrameCount = (uint)values.Count;

                            if (values.Count > 0)
                                track.Flags |= (uint)GetTrackTypeFlag(values[0]);

                            if (values.Count == 1)
                            {
                                if (animation.Type == AnimType.Transform)
                                    track.Flags |= (int)AnimTrackFlags.ConstTransform;
                                else
                                    track.Flags |= (int)AnimTrackFlags.Constant;

                                track.FrameCount = 1;

                                WriteValues(w, (int)track.Flags, values);
                            }
                            else
                            {
                                if (WriteValues(w, (int)track.Flags, values))
                                    track.Flags |= (int)AnimTrackFlags.Compressed;
                                else
                                    track.Flags |= (int)AnimTrackFlags.Direct;
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

            Ssbh.TrySaveSsbhFile(fname, animFile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private AnimGroup GetGroup(AnimType type)
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
            if (o is AnimTrackTransform)
            {
                return (int)AnimTrackFlags.Transform;
            }
            else if (o is AnimTrackCustomVector4)
            {
                return (int)AnimTrackFlags.Vector4;
            }
            else if (o is AnimTrackTexture)
            {
                return (int)AnimTrackFlags.Texture;
            }
            else if (o is bool)
            {
                return (int)AnimTrackFlags.Boolean;
            }
            else if (o is float)
            {
                return (int)AnimTrackFlags.Float;
            }
            else if (o is int)
            {
                return (int)AnimTrackFlags.PatternIndex;
            }
            return 0;
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
            private readonly List<float> values = new List<float>();

            // cache bitcount
            private float currentError = float.MaxValue;
            private int bitCount;

            public void Add(float v)
            {
                if (float.IsNaN(v))
                    v = 0.0f;

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
                if (Constant || values.Count == 0)
                    return 0;
                if (epsilon == (double)currentError)
                    return bitCount;
                currentError = epsilon;
                while (FindOptimalBitCount(epsilon) == -1)
                {
                    epsilon *= 2f;
                    if (epsilon > 1.0)
                        break;
                }
                return bitCount;
            }

            private int FindOptimalBitCount(float epsilon)
            {
                for (int bits = 1; bits < 31; ++bits)
                {
                    bitCount = bits;
                    if (ComputeError(bits) < (double)epsilon)
                        return bits;
                }
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
                if ((double)v <= Min)
                    return 0;
                return v >= (double)Max ? QuantanizationValue : (int)((v - (double)Min) / (Max - (double)Min) * QuantanizationValue);
            }

            /// <summary>
            /// Gets the encoded then decoded value
            /// </summary>
            /// <param name="v"></param>
            /// <returns></returns>
            public float DecompressedValue(float v)
            {
                var value = SsbhAnimTrackDecoder.Lerp(Min, Max, 0, 1, GetQuantanizedValue(v) / (float)QuantanizationValue);
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
            private readonly List<byte> output = new List<byte>();

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
            w.Write((byte)0);

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
            short flags = 0;
            short bitsPerEntry = 0;

            Quantanizer v1 = new Quantanizer();
            Quantanizer v2 = new Quantanizer();
            Quantanizer v3 = new Quantanizer();
            Quantanizer v4 = new Quantanizer();

            foreach (AnimTrackCustomVector4 vec in values)
            {
                v1.Add(vec.X);
                v2.Add(vec.Y);
                v3.Add(vec.Z);
                v4.Add(vec.W);
            }

            bitsPerEntry = (short)(v1.GetBitCount(epsilon) + v2.GetBitCount(epsilon) + v3.GetBitCount(epsilon) + v4.GetBitCount(epsilon));


            // Write Compressed Header
            w.Write((short)0x04);
            w.Write(flags);
            w.Write((short)(0x10 + 0x10 * 4)); // default values offset
            w.Write(bitsPerEntry);
            w.Write(0x10 + 0x10 * 4 + sizeof(float) * 4); // compressed data start
            w.Write(values.Count); // frame count

            // table
            w.Write(v1.Min); w.Write(v1.Max); w.Write(v1.GetBitCount(epsilon)); w.Write(0);
            w.Write(v2.Min); w.Write(v2.Max); w.Write(v2.GetBitCount(epsilon)); w.Write(0);
            w.Write(v3.Min); w.Write(v3.Max); w.Write(v3.GetBitCount(epsilon)); w.Write(0);
            w.Write(v4.Min); w.Write(v4.Max); w.Write(v4.GetBitCount(epsilon)); w.Write(0);

            // default values
            w.Write(((AnimTrackCustomVector4)values[0]).X);
            w.Write(((AnimTrackCustomVector4)values[0]).Y);
            w.Write(((AnimTrackCustomVector4)values[0]).Z);
            w.Write(((AnimTrackCustomVector4)values[0]).W);

            // compressed data
            BitWriter bitWriter = new BitWriter();
            foreach (AnimTrackCustomVector4 vec in values)
            {
                bitWriter.WriteBits(v1.GetQuantanizedValue(vec.X), v1.GetBitCount(epsilon));
                bitWriter.WriteBits(v2.GetQuantanizedValue(vec.Y), v2.GetBitCount(epsilon));
                bitWriter.WriteBits(v3.GetQuantanizedValue(vec.Z), v3.GetBitCount(epsilon));
                bitWriter.WriteBits(v4.GetQuantanizedValue(vec.W), v4.GetBitCount(epsilon));
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
            Quantanizer sx = new Quantanizer();
            Quantanizer sy = new Quantanizer();
            Quantanizer sz = new Quantanizer();
            Quantanizer rx = new Quantanizer();
            Quantanizer ry = new Quantanizer();
            Quantanizer rz = new Quantanizer();
            Quantanizer x = new Quantanizer();
            Quantanizer y = new Quantanizer();
            Quantanizer z = new Quantanizer();

            // pre-process
            foreach (AnimTrackTransform transform in values)
            {
                sx.Add(transform.Sx);
                sy.Add(transform.Sy);
                sz.Add(transform.Sz);
                rx.Add(transform.Rx);
                ry.Add(transform.Ry);
                rz.Add(transform.Rz);
                x.Add(transform.X);
                y.Add(transform.Y);
                z.Add(transform.Z);
            }

            short flags = 0;
            ushort bitsPerEntry = 0;

            bool hasScale = (!sx.Constant || !sy.Constant || !sz.Constant);
            bool hasRotation = (!rx.Constant || !ry.Constant || !rz.Constant);
            bool hasPosition = (!x.Constant || !y.Constant || !z.Constant);

            if (sx.GetBitCount(epsilon) == -1 || sy.GetBitCount(epsilon) == -1 || (sz.GetBitCount(epsilon) == -1 || rx.GetBitCount(epsilon) == -1) || (ry.GetBitCount(epsilon) == -1 || rz.GetBitCount(epsilon) == -1 || (x.GetBitCount(epsilon) == -1 || y.GetBitCount(epsilon) == -1)) || z.GetBitCount(epsilon) == -1)
                throw new Exception("Compression Level is too small to compress!");

            if (!hasScale)
            {
                flags |= 0x02;
            }
            else
            {
                bitsPerEntry += (ushort)((sx.Constant ? 0 : sx.GetBitCount(epsilon)) + (sy.Constant ? 0 : sy.GetBitCount(epsilon)) + (sz.Constant ? 0 : sz.GetBitCount(epsilon)));
                flags |= 0x01;
            }
            if (hasRotation)
            {
                bitsPerEntry += (ushort)((rx.Constant ? 0 : rx.GetBitCount(epsilon)) + (ry.Constant ? 0 : ry.GetBitCount(epsilon)) + (rz.Constant ? 0 : rz.GetBitCount(epsilon)) + 1); // the 1 is for extra w rotation bit
                flags |= 0x04;
            }
            if (hasPosition)
            {
                bitsPerEntry += (ushort)((x.Constant ? 0 : x.GetBitCount(epsilon)) + (y.Constant ? 0 : y.GetBitCount(epsilon)) + (z.Constant ? 0 : z.GetBitCount(epsilon)));
                flags |= 0x08;
            }

            // Compressed Header
            w.Write((short)0x04);
            w.Write(flags);
            w.Write((short)160);
            w.Write(bitsPerEntry);
            w.Write(204);
            w.Write(values.Count);
            w.Write(sx.Min);
            w.Write(sx.Max);
            w.Write(hasScale ? sx.GetBitCount(epsilon) : 16L);
            w.Write(sy.Min);
            w.Write(sy.Max);
            w.Write(hasScale ? sy.GetBitCount(epsilon) : 16L);
            w.Write(sz.Min);
            w.Write(sz.Max);
            w.Write(hasScale ? sz.GetBitCount(epsilon) : 16L);
            w.Write(rx.Min);
            w.Write(rx.Max);
            w.Write(hasRotation ? rx.GetBitCount(epsilon) : 16L);
            w.Write(ry.Min);
            w.Write(ry.Max);
            w.Write(hasRotation ? ry.GetBitCount(epsilon) : 16L);
            w.Write(rz.Min);
            w.Write(rz.Max);
            w.Write(hasRotation ? rz.GetBitCount(epsilon) : 16L);
            w.Write(x.Min);
            w.Write(x.Max);
            w.Write(hasPosition ? x.GetBitCount(epsilon) : 16L);
            w.Write(y.Min);
            w.Write(y.Max);
            w.Write(hasPosition ? y.GetBitCount(epsilon) : 16L);
            w.Write(z.Min);
            w.Write(z.Max);
            w.Write(hasPosition ? z.GetBitCount(epsilon) : 16L);
            AnimTrackTransform defaultValue = (AnimTrackTransform)values[0];
            w.Write(defaultValue.Sx);
            w.Write(defaultValue.Sy);
            w.Write(defaultValue.Sz);
            w.Write(defaultValue.Rx);
            w.Write(defaultValue.Ry);
            w.Write(defaultValue.Rz);
            w.Write(defaultValue.Rw);
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
                    writer.WriteBits(sx.GetQuantanizedValue(transform.Sx), sx.GetBitCount(epsilon));
                    writer.WriteBits(sy.GetQuantanizedValue(transform.Sy), sy.GetBitCount(epsilon));
                    writer.WriteBits(sz.GetQuantanizedValue(transform.Sz), sz.GetBitCount(epsilon));
                }
                if (hasRotation)
                {
                    writer.WriteBits(rx.GetQuantanizedValue(transform.Rx), rx.GetBitCount(epsilon));
                    writer.WriteBits(ry.GetQuantanizedValue(transform.Ry), ry.GetBitCount(epsilon));
                    writer.WriteBits(rz.GetQuantanizedValue(transform.Rz), rz.GetBitCount(epsilon));
                }
                if (hasPosition)
                {
                    writer.WriteBits(x.GetQuantanizedValue(transform.X), x.GetBitCount(epsilon));
                    writer.WriteBits(y.GetQuantanizedValue(transform.Y), y.GetBitCount(epsilon));
                    writer.WriteBits(z.GetQuantanizedValue(transform.Z), z.GetBitCount(epsilon));
                }
                if (hasRotation)
                {
                    // flip w bit

                    float calculateW = (float)Math.Sqrt(Math.Abs(1 - (
                        rx.DecompressedValue(transform.Rx) * rx.DecompressedValue(transform.Rx) +
                        ry.DecompressedValue(transform.Ry) * ry.DecompressedValue(transform.Ry) +
                        rz.DecompressedValue(transform.Rz) * rz.DecompressedValue(transform.Rz))));

                    writer.WriteBits(Math.Sign(transform.Rw) != Math.Sign(calculateW) ? 1 : 0, 1);
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
                w.Write(transform.Sx);
                w.Write(transform.Sy);
                w.Write(transform.Sz);
                w.Write(transform.Rx);
                w.Write(transform.Ry);
                w.Write(transform.Rz);
                w.Write(transform.Rw);
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
            else if (o is int i)
            {
                w.Write(i);
            }
            else
                throw new NotSupportedException($"{o.GetType()} is not a supported track object");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        private AnimNode GetNode(AnimType type, string nodeName)
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
