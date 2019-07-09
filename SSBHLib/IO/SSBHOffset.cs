namespace SSBHLib.IO
{
    public struct SSBHOffset
    {
        public long Value { get ; }

        public SSBHOffset(long value)
        {
            Value = value;
        }

        public static implicit operator SSBHOffset(long s)
        {
            return new SSBHOffset(s);
        }

        public static implicit operator long(SSBHOffset p)
        {
            return p.Value;
        }
    }
}
