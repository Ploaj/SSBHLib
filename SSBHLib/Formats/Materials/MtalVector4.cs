﻿namespace SSBHLib.Formats.Materials
{
    public partial class MtalAttribute
    {
        public class MtalVector4 : ISSBH_File
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
            public float W { get; set; }

            public override string ToString()
            {
                return $"({X}, {Y}, {Z}, {W})";
            }
        }
    }
}