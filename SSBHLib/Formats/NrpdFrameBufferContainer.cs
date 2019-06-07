namespace SSBHLib.Formats
{
    public class NrpdFrameBufferContainer : ISSBH_File
    {
        public NrpdFrameBuffer FrameBuffer { get; set; }

        public ulong Type { get; set; }
    }
}
