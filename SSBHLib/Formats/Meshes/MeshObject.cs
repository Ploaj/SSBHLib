
using SSBHLib.IO;

namespace SSBHLib.Formats.Meshes
{
    public class MeshObject : ISSBH_File
    {
        public string Name { get; set; }
        
        public long SubMeshIndex { get; set; }
        
        public string ParentBoneName { get; set; }
        
        public int VertexCount { get; set; }
        
        public int IndexCount { get; set; }
        
        public uint Unk2 { get; set; } //usually 3? maybe means triangles?
        
        public int VertexOffset { get; set; }
        
        public int VertexOffset2 { get; set; }
        
        public int FinalBufferOffset { get; set; }
        
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

        public float BoundingSphereX { get; set; }
        public float BoundingSphereY { get; set; }
        public float BoundingSphereZ { get; set; }
        public float BoundingSphereRadius { get; set; }

        [ParseTag(InLine = true)]
        public float[] HeaderFloats { get; set; } = new float[22];

        public MeshAttribute[] Attributes { get; set; }
    }
}
