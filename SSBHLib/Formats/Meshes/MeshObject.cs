namespace SSBHLib.Formats.Meshes
{
    public enum DrawElementType : int
    {
        UnsignedShort = 0,
        UnsignedInt = 1
    }

    public enum RiggingType : int
    {
        SingleBound = 0,
        Regular = 1
    }

    public class MeshObject : SsbhFile
    {
        /// <summary>
        /// The name that identifies this object, which may be shared with other <see cref="MeshObject"/>.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Differentiates objects with duplicate names.
        /// Ex: gamemodel0, gamemodel1.
        /// </summary>
        public long SubIndex { get; set; }

        /// <summary>
        /// The name of the bone used for single binding.
        /// </summary>
        public string ParentBoneName { get; set; } = "";
        
        /// <summary>
        /// The number of elements in each buffer in <see cref="Mesh.VertexBuffers"/>.
        /// </summary>
        public int VertexCount { get; set; }
        
        /// <summary>
        /// The number of elements in each buffer in <see cref="Mesh.PolygonBuffer"/>.
        /// </summary>
        public int IndexCount { get; set; }

        // TODO: Unk2
        public uint Unk2 { get; set; } = 3; //usually 3? maybe means triangles?
        
        /// <summary>
        /// The offset in bytes for the start of the data in the first buffer for <see cref="Mesh.VertexBuffers"/>.
        /// </summary>
        public int VertexOffset { get; set; }

        /// <summary>
        /// The offset in bytes for the start of the data in the second buffer for <see cref="Mesh.VertexBuffers"/>.
        /// </summary>
        public int VertexOffset2 { get; set; }
        
        // TODO: FinalBufferOffset?
        public int FinalBufferOffset { get; set; }
        
        // TODO: BufferIndex
        public int BufferIndex { get; set; } //??

        /// <summary>
        /// The stride in bytes for each element in the first buffer for <see cref="Mesh.VertexBuffers"/>.
        /// </summary>
        public int Stride { get; set; }

        /// <summary>
        /// The stride in bytes for each element in the second buffer for <see cref="Mesh.VertexBuffers"/>.
        /// </summary>
        public int Stride2 { get; set; }
        
        // TODO: Unk6
        public int Unk6 { get; set; } // usually 0
        
        // TODO: Unk7
        public int Unk7 { get; set; } // usually 0 long with above?

        /// <summary>
        /// The offset in bytes for the start of the data in <see cref="Mesh.PolygonBuffer"/>.
        /// </summary>
        public uint ElementOffset { get; set; }

        // TODO: Unk8
        public int Unk8 { get; set; } = 4;// usually 4? maybe something to do with the final buffer offset?
    
        /// <summary>
        /// The data type of each element in <see cref="Mesh.PolygonBuffer"/>.
        /// </summary>
        public DrawElementType DrawElementType { get; set; }
        
        public RiggingType RiggingType { get; set; }
        
        // TODO: Unk11?
        public int Unk11 { get; set; } // usually 0 long with above?

        // TODO: Some sort of flags. Values are 0, 1, 256, 257
        public int Unk12 { get; set; }

        public Vector3 BoundingSphereCenter { get; set; }
        public float BoundingSphereRadius { get; set; }
        
        public Vector3 BoundingBoxMin { get; set; }

        public Vector3 BoundingBoxMax { get; set; }
        
        public Vector3 OrientedBoundingBoxCenter { get; set; }

        // TODO: What does this matrix do?
        public Matrix3x3 OrientedBoundingBoxTransform { get; set; }

        public Vector3 OrientedBoundingBoxSize { get; set; }

        public MeshAttribute[] Attributes { get; set; }
    }
}
