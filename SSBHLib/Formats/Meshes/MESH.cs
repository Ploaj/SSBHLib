using SSBHLib.IO;

namespace SSBHLib.Formats.Meshes
{
    [SsbhFile("HSEM")]
    public class Mesh : SsbhFile
    {
        public uint Magic { get; set; } = 0x4D455348;

        public ushort VersionMajor { get; set; } = 0x1;// 0x0001

        public ushort VersionMinor { get; set; } = 0xA;// 0x000A and 0x0008 - Most use 0x000A

        public string ModelName { get; set; } = "";// unused

        public Vector3 BoundingSphereCenter { get; set; }
        public float BoundingSphereRadius { get; set; }

        public Vector3 BoundingBoxMin { get; set; }
        public Vector3 BoundingBoxMax { get; set; }
        public Vector3 OrientedBoundingBoxCenter { get; set; }

        // TODO: What does this matrix do?
        public Matrix3x3 OrientedBoundingBoxTransform { get; set; }
        public Vector3 OrientedBoundingBoxSize { get; set; }

        public float Unk1 { get; set; }
        
        public MeshObject[] Objects { get; set; }
        
        public int[] BufferSizes { get; set; }
        
        public long PolygonIndexSize { get; set; } // TODO: seems to match index count?
        
        public MeshBuffer[] VertexBuffers { get; set; }
        
        public byte[] PolygonBuffer { get; set; }
        
        public MeshRiggingGroup[] RiggingBuffers { get; set; }
        
        public long UnknownOffset { get; set; }
        
        public long UnknownSize { get; set; }
    }
}
