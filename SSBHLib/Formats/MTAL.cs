
using SSBHLib.IO;

namespace SSBHLib.Formats
{
    [SSBHFileAttribute("LTAM")]
    public class MTAL : ISSBH_File
    {
        public uint Magic { get; set; }

        public short MajorVersion { get; set; }

        public short MinorVersion { get; set; }

        public MTAL_Entry[] MaterialEntries { get; set; }
    }

    public class MTAL_Entry : ISSBH_File
    {
        public string MaterialLabel { get; set; }

        public MTAL_Attribute[] MaterialData { get; set; }

        public string MaterialName { get; set; }

    }

    public class MTAL_Attribute : ISSBH_File
    {
        public long ParamID { get; set; }

        public SSBHOffset OffsetToData { get; set; }

        public long DataType { get; set; }

        // not part of the entry
        public object DataObject;

        public override void PostProcess(SSBHParser R)
        {
            R.Seek((int)OffsetToData);
            if (DataType == 0x01)
            {
                //TODO:
            }
            if (DataType == 0x02)
            {
                DataObject = R.ReadUInt64();
            }
            if (DataType == 0x05)
            {
                DataObject = R.Parse<MTAL_Vector4>();
            }
            if (DataType == 0x0B)
            {
                DataObject = R.Parse<MTAL_String>();
            }
            if (DataType == 0x0E)
            {
                DataObject = R.Parse<MTAL_Unk_0E>();
            }
            if (DataType == 0x10)
            {
                // TODO:
            }
            if (DataType == 0x11)
            {
                DataObject = R.Parse<MTAL_Unk_11>();
            }
            if (DataType == 0x12)
            {
                DataObject = R.Parse<MTAL_Unk_12>();
            }
        }

        public class MTAL_Vector4 : ISSBH_File
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
            public float W { get; set; }
        }

        public class MTAL_String : ISSBH_File
        {
            public string Text { get; set; }
        }

        public class MTAL_Unk_0E : ISSBH_File
        {
            public int Unk1 { get; set; }
            public int Unk2 { get; set; }
            public int Unk3 { get; set; }
            public int Unk4 { get; set; }
            public int Unk5 { get; set; }
            public int Unk6 { get; set; }
            public int Unk7 { get; set; }
            public int Unk8 { get; set; }
            public int Unk9 { get; set; }
            public int Unk10 { get; set; }
            public int Unk11 { get; set; }
            public float Unk12 { get; set; }
            public float Unk13 { get; set; }
            public int Unk14 { get; set; }
            public int Unk15 { get; set; }
        }


        public class MTAL_Unk_11 : ISSBH_File
        {
            public int Unk1 { get; set; }

            public int Unk2 { get; set; }

            public int Unk3 { get; set; }

            public int Unk4 { get; set; }

            public int Unk5 { get; set; }

            public int Unk6 { get; set; }

            public int Unk7 { get; set; }

            public int Unk8 { get; set; }

            public int Unk9 { get; set; }

            public int Unk10 { get; set; }

            public int Unk11 { get; set; }

            public int Unk12 { get; set; }
        }

        public class MTAL_Unk_12 : ISSBH_File
        {

            public int Unk1 { get; set; }

            public int Unk2 { get; set; }

            public int Unk3 { get; set; }

            public int Unk4 { get; set; }

            public int Unk5 { get; set; }

            public int Unk6 { get; set; }

            public int Unk7 { get; set; }

            public int Unk8 { get; set; }
        }
    }
}