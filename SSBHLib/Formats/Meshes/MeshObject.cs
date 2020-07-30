namespace SSBHLib.Formats.Meshes
{
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
        
        public int VertexOffset { get; set; }
        
        public int VertexOffset2 { get; set; }
        
        public int FinalBufferOffset { get; set; }
        
        // TODO: BufferIndex
        public int BufferIndex { get; set; } //??
        
        public int Stride { get; set; }
        
        public int Stride2 { get; set; }
        
        // TODO: Unk6
        public int Unk6 { get; set; } // usually 0
        
        // TODO: Unk7
        public int Unk7 { get; set; } // usually 0 long with above?

        /// <summary>
        /// The offset in bytes for the <see cref="Mesh.PolygonBuffer"/>.
        /// </summary>
        public uint ElementOffset { get; set; }

        // TODO: Unk8
        public int Unk8 { get; set; } = 4;// usually 4? maybe something to do with the final buffer offset?
    
        // TODO: DrawElementType can be an enum.
        public int DrawElementType { get; set; } // 1 for uint and 0 for ushort
        
        // TODO: HasRigging can be an enum.
        public int HasRigging { get; set; } // 0 for single bind 1 otherwise?
        
        public int Unk11 { get; set; } // usually 0 long with above?

        // TODO: UnkBounding0
        public float UnkBounding0 { get; set; }

        public Vector3 BoundingSphereCenter { get; set; }
        public float BoundingSphereRadius { get; set; }
        
        public Vector3 BoundingBoxMin { get; set; }

        public Vector3 BoundingBoxMax { get; set; }
        
        public Vector3 ObbCenter { get; set; }

        public Matrix3x3 Matrix { get; set; }

        public Vector3 ObbSize { get; set; }

        public MeshAttribute[] Attributes { get; set; }
    }
}
