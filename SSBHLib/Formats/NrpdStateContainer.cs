namespace SSBHLib.Formats
{
    public class NrpdStateContainer : ISSBH_File
    {
        public NrpdSampler StateObject { get; set; } // TODO: not always sampler

        public ulong Type { get; set; } // TODO: 0 - sampler 3 - state 2 - ZKeepAlways?
    }
}
