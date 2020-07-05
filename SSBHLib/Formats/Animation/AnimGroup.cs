namespace SSBHLib.Formats.Animation
{
    public enum AnimType : ulong
    {
        Transform = 1Lu,
        Visibility = 2Lu,
        Material = 4Lu,
        Camera = 5Lu
    }

    public class AnimGroup : SsbhFile
    {
        public AnimType Type { get; set; }

        public AnimNode[] Nodes { get; set; }
    }

    public class AnimNode : SsbhFile
    {
        public string Name { get; set; }

        public AnimTrack[] Tracks { get; set; }
    }
}
