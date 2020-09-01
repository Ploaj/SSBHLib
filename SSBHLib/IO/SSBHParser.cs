using SSBHLib.Formats;
using SSBHLib.Formats.Animation;
using SSBHLib.Formats.Materials;
using SSBHLib.Formats.Meshes;
using System;
using System.Collections.Generic;
using System.IO;

namespace SSBHLib.IO
{
    public class SsbhParser : BinaryReader
    {
        public long Position => BaseStream.Position;
        public long FileSize => BaseStream.Length;

        private int bitPosition;

        // Avoid reflection invoke overhead for known file magics.
        private static readonly Dictionary<string, Func<SsbhParser, SsbhFile>> parseMethodByMagic = new Dictionary<string, Func<SsbhParser, SsbhFile>>()
        {
            { "BPLH", (parser) => parser.ParseHlpb() },
            { "DPRN", (parser) => parser.ParseNrpd() },
            { "RDHS", (parser) => parser.ParseShdr() },
            { "LEKS", (parser) => parser.ParseSkel() },
            { "LDOM", (parser) => parser.ParseModl() },
            { "HSEM", (parser) => parser.ParseMesh() },
            { "LTAM", (parser) => parser.ParseMatl() },
            { "MINA", (parser) => parser.ParseAnim() }
        };

        // Avoid reflection invoke overhead for known types.
        private static readonly Dictionary<Type, Func<SsbhParser, SsbhFile>> parseMethodByType = new Dictionary<Type, Func<SsbhParser, SsbhFile>>()
        {
            { typeof(AnimGroup), (parser) => parser.ParseAnimGroup() },
            { typeof(AnimNode), (parser) => parser.ParseAnimNode() },
            { typeof(AnimTrack), (parser) => parser.ParseAnimTrack() },
            { typeof(MatlEntry), (parser) => parser.ParseMatlEntry() },
            { typeof(MatlAttribute), (parser) => parser.ParseMatlAttribute() },
            { typeof(ModlMaterialName), (parser) => parser.ParseModlMaterialName() },
            { typeof(ModlEntry), (parser) => parser.ParseModlEntry() },
            { typeof(MeshObject), (parser) => parser.ParseMeshObject() },
            { typeof(MeshAttribute), (parser) => parser.ParseMeshAttribute() },
            { typeof(MeshAttributeString), (parser) => parser.ParseMeshAttributeString() },
            { typeof(MeshBuffer), (parser) => parser.ParseMeshBuffer() },
            { typeof(MeshRiggingGroup), (parser) => parser.ParseMeshRiggingGroup() },
            { typeof(MeshBoneBuffer), (parser) => parser.ParseMeshBoneBuffer() },
            { typeof(SkelBoneEntry), (parser) => parser.ParseSkelBoneEntry() },
        };

        public SsbhParser(Stream stream) : base(stream)
        {
            
        }

        public void Seek(long position)
        {
            BaseStream.Position = position;
        }

        public byte Peek()
        {
            byte b = ReadByte();
            BaseStream.Seek(-1, SeekOrigin.Current);
            return b;
        }

        public bool TryParse<T>(out T file) where T : SsbhFile
        {
            file = null;
            if (FileSize < 4)
                return false;
            
            string fileMagic = new string(ReadChars(4));
            Seek(Position - 4);
            if (fileMagic.Equals("HBSS"))
            {
                Seek(0x10);
                fileMagic = new string(ReadChars(4));
                Seek(0x10);
            }

            if (parseMethodByMagic.ContainsKey(fileMagic))
            {
                file = (T)parseMethodByMagic[fileMagic](this);
                return true;
            }

            return false;
        }

        public string ReadOffsetReadString()
        {
            long stringOffset = Position + ReadInt64();
            return ReadString(stringOffset);
        }

        public long ReadRelativeGetAbsoluteOffset()
        {
            return Position + ReadInt64();
        }

        public string ReadString(long offset)
        {
            long previousPosition = Position;

            var stringValue = new System.Text.StringBuilder();

            Seek(offset);
            if (Position >= FileSize)
            {
                Seek(previousPosition);
                return "";
            }

            byte b = ReadByte();
            while (b != 0)
            {
                stringValue.Append((char)b);
                b = ReadByte();
            }
            
            Seek(previousPosition);

            return stringValue.ToString();
        }

        public Vector3 ReadVector3()
        {
            return new Vector3
            {
                X = ReadSingle(),
                Y = ReadSingle(),
                Z = ReadSingle()
            };
        }

        public Vector4 ReadVector4()
        {
            return new Vector4
            {
                X = ReadSingle(),
                Y = ReadSingle(),
                Z = ReadSingle(),
                W = ReadSingle()
            };
        }
        public Matrix3x3 ReadMatrix3x3()
        {
            return new Matrix3x3
            {
                Row1 = ReadVector3(),
                Row2 = ReadVector3(),
                Row3 = ReadVector3(),
            };
        }

        public Matrix4x4 ReadMatrix4x4()
        {
            return new Matrix4x4
            {
                Row1 = ReadVector4(),
                Row2 = ReadVector4(),
                Row3 = ReadVector4(),
                Row4 = ReadVector4()
            };
        }

        public int ReadBits(int bitCount)
        {
            byte b = Peek();
            int value = 0;
            int le = 0;
            int bitIndex = 0;
            for (int i = 0; i < bitCount; i++)
            {
                byte bit = (byte)((b & (0x1 << (bitPosition))) >> (bitPosition));
                value |= (bit << (le + bitIndex));
                bitPosition++;
                bitIndex++;
                if (bitPosition >= 8)
                {
                    bitPosition = 0;
                    b = ReadByte();
                    b = Peek();
                }
                if (bitIndex >= 8)
                {
                    bitIndex = 0;
                    if (le + 8 > bitCount)
                    {
                        le = bitCount - 1;
                    }
                    else
                        le += 8;
                }
            }
            
            return value;
        }
    }
}
