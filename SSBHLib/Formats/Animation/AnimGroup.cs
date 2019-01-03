namespace SSBHLib.Formats.Animation
{
    public enum ANIM_TYPE
    {
        Transform = 1,
        Visibilty = 2,
        Material = 4,
        Camera = 5
    }

    public class AnimGroup : ISSBH_File
    {
        public ANIM_TYPE Type { get; set; }

        public AnimNode[] Nodes { get; set; }
    }

    public class AnimNode : ISSBH_File
    {
        public string Name { get; set; }

        public AnimTrack[] Tracks { get; set; }
    }
}
