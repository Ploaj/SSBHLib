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
        public uint Magic { get; set; } = 0x4D455348;

        public ushort VersionMajor { get; set; } = 0x1;// 0x0001

        public ushort VersionMinor { get; set; } = 0xA;// 0x000A and 0x0008 - Most use 0x000A

        public string ModelName { get; set; } = "";// unused

        public float BoundingSphereX { get; set; }
        public float BoundingSphereY { get; set; }
        public float BoundingSphereZ { get; set; }
        public float BoundingSphereRadius { get; set; }

        public float MinBoundingBoxX { get; set; }
        public float MinBoundingBoxY { get; set; }
        public float MinBoundingBoxZ { get; set; }

        public float MaxBoundingBoxX { get; set; }
        public float MaxBoundingBoxY { get; set; }
        public float MaxBoundingBoxZ { get; set; }

        [ParseTag(InLine = true)]
        public float[] UnknownFloats { get; set; } = new float[16]; // Possibly a matrix

        [ParseTag("VersionMinor>8")]
        public MeshObject[] Objects { get; set; }

        [ParseTag("VersionMinor>8")]
        public int[] BufferSizes { get; set; }

        [ParseTag("VersionMinor>8")]
        public long PolygonIndexSize { get; set; } // seems to match index count?

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
    }
}
