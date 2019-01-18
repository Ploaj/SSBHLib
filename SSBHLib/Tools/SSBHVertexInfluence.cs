namespace SSBHLib.Tools
{
    /// <summary>
    /// Represents a rigging influence for a vertex
    /// </summary>
    public struct SSBHVertexInfluence
    {
        /// <summary>
        /// Index of vertex in the MESH vertex array
        /// </summary>
        public ushort VertexIndex;
        /// <summary>
        /// Name of the bone that influences this vertex
        /// </summary>
        public string BoneName;
        /// <summary>
        /// Weight of the bone influence
        /// </summary>
        public float Weight;
    }
}
