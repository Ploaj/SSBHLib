namespace SSBHLib.IO
{
    public struct SsbhOffset
    {
        public long Value { get ; }

        public SsbhOffset(long value)
        {
            Value = value;
        }

        public static implicit operator SsbhOffset(long s)
        {
            return new SsbhOffset(s);
        }

        public static implicit operator long(SsbhOffset p)
        {
            return p.Value;
        }
    }
}
