﻿
using SSBHLib.IO;

namespace SSBHLib.Formats
{
    public enum SSBVertexAttribFormat
    {
        Float = 0,
        Byte = 2,
        HalfFloat = 5,
        HalfFloat2 = 8,
    }

    [SSBHFileAttribute("HSEM")]
    public class MESH : ISSBH_File
    {
        public uint Magic { get; set; }
        
        public ushort VersionMajor { get; set; } // 0x0001
        
        public ushort VersionMinor { get; set; } // 0x000A and 0x0008 - Most use 0x000A
        
        public string ModelName { get; set; } // unused

        public float BoundingSphereX { get; set; }
        public float BoundingSphereY { get; set; }
        public float BoundingSphereZ { get; set; }
        public float BoundingSphereRadius { get; set; }

        [ParseTag(InLine = true)]
        public float[] HeaderFloats { get; set; } = new float[22];

        [ParseTag("VersionMinor>8")]
        public MESH_Object[] Objects { get; set; }

        [ParseTag("VersionMinor>8")]
        public int[] BufferSizes { get; set; }

        [ParseTag("VersionMinor>8")]
        public long PolygonIndexCount { get; set; } // seems to match index count?

        [ParseTag("VersionMinor>8")]
        public MESH_Buffer[] VertexBuffers { get; set; }

        [ParseTag("VersionMinor>8")]
        public byte[] PolygonBuffer { get; set; }

        [ParseTag("VersionMinor>8")]
        public MESH_RiggingGroup[] RiggingBuffers { get; set; }

        [ParseTag("VersionMinor>8")]
        public long UnknownOffset { get; set; }

        [ParseTag("VersionMinor>8")]
        public long UnknownSize { get; set; }

        public System.Tuple<float, float, float, float> GetBoundingSphere()
        {
            // XYZ, Radius
            return new System.Tuple<float, float, float, float>(BoundingSphereX, BoundingSphereY, BoundingSphereZ, BoundingSphereRadius);
        }
    }

    public class MESH_RiggingGroup : ISSBH_File
    {
        public string Name { get; set; }
        
        public long SubMeshIndex { get; set; }
    
        public long Unk_Flags { get; set; }

        public MESH_BoneBuffer[] Buffers { get; set; }
    }

    public class MESH_BoneBuffer : ISSBH_File
    {
        public string BoneName { get; set; }

        public byte[] Data { get; set; }
    }

    public class MESH_Buffer : ISSBH_File
    {
        public byte[] Buffer { get; set; }
    }

    public class MESH_Object : ISSBH_File
    {
        public string Name { get; set; }
        
        public long SubMeshIndex { get; set; }
        
        public string ParentBoneName { get; set; }
        
        public int VertexCount { get; set; }
        
        public int IndexCount { get; set; }
        
        public uint Unk2 { get; set; } //usually 3? maybe means triangles?
        
        public int VertexOffset { get; set; }
        
        public int VertexOffset2 { get; set; }
        
        public int Unk4 { get; set; } // some offset inside of some unforseen buffer
        
        public int BufferIndex { get; set; }
        
        public int Stride { get; set; }
        
        public int Stride2 { get; set; }
        
        public int Unk6 { get; set; } // usually 0
        
        public int Unk7 { get; set; } // usually 0 long with above?

        public uint ElementOffset { get; set; }
        
        public int Unk8 { get; set; } // usually 4?
    
        public int DrawElementType { get; set; }
        
        public int HasRigging { get; set; } // 0 for single bind 1 otherwise?
        
        public int Unk11 { get; set; } // usually 0 long with above?

        [ParseTag(InLine = true)]
        public float[] Floats { get; set; } = new float[26];

        public MESH_Attribute[] Attributes { get; set; }

        public System.Tuple<float, float, float, float> GetBoundingSphere()
        {
            // XYZ, Radius
            return new System.Tuple<float, float, float, float>(Floats[1], Floats[2], Floats[3], Floats[4]);
        }
    }

    public class MESH_Attribute : ISSBH_File
    {
        public int Index { get; set; }
        
        public int DataType { get; set; }
        
        public int BufferIndex { get; set; }
        
        public int BufferOffset { get; set; }
        
        public int Unk4_0 { get; set; } // usually 0 padding?
        
        public int Unk5_0 { get; set; } // usually 0 padding?

        public string Name { get; set; }
        
        public MESH_AttributeString[] AttributeStrings { get; set; }
    }

    public class MESH_AttributeString : ISSBH_File
    {
        public string Name { get; set; }
    }
}
