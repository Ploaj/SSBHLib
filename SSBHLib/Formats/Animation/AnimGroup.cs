namespace SSBHLib.Formats.Animation
{
    public enum ANIM_TYPE
    {
        Transform = 1,
        Visibilty = 2,
        Material = 4
    }

    public class ANIM_Group : ISSBH_File
    {
        public ANIM_TYPE Type { get; set; }

        public ANIM_Node[] Nodes { get; set; }
    }

    public class ANIM_Node : ISSBH_File
    {
        public string Name { get; set; }

        public ANIM_Track[] Tracks { get; set; }
    }
}
