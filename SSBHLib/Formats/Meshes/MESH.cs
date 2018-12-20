using SSBHLib.IO;

namespace SSBHLib.Formats.Meshes
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
        public MeshObject[] Objects { get; set; }

        [ParseTag("VersionMinor>8")]
        public int[] BufferSizes { get; set; }

        [ParseTag("VersionMinor>8")]
        public long PolygonIndexCount { get; set; } // seems to match index count?

        [ParseTag("VersionMinor>8")]
        public MeshBuffer[] VertexBuffers { get; set; }

        [ParseTag("VersionMinor>8")]
        public byte[] PolygonBuffer { get; set; }

        [ParseTag("VersionMinor>8")]
        public MeshRiggingGroup[] RiggingBuffers { get; set; }

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
}
