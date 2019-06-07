namespace SSBHLib.Formats
{
    public class NrpdRenderPass : ISSBH_File
    {
        public string Name { get; set; }
        public ulong Offset2 { get; set; } // TODO: 
        public ulong Type2 { get; set; }
        public ulong Offset3 { get; set; }
        public ulong Type3 { get; set; }
        public string UnkString { get; set; }
        public ulong Type4 { get; set; }
        public ulong Padding { get; set; }
    }
}
