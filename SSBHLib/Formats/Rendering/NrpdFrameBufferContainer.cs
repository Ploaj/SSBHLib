namespace SSBHLib.Formats.Rendering
{
    public class NrpdFrameBufferContainer : SsbhFile
    {
        public NrpdFrameBuffer FrameBuffer { get; set; }

        public ulong Type { get; set; }
    }
}
