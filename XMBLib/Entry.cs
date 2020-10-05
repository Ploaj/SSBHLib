namespace XMBLib
{
    public struct EntryData
    {
        public uint NameOffset { get; set; }
        public ushort PropertyCount { get; set; }
        public ushort ChildCount { get; set; }
        public ushort PropertyStartIndex { get; set; }
        public ushort Unk1 { get; set; }
        public short ParentIndex { get; set; }
        public ushort Unk2 { get; set; }
    }
}
