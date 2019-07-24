namespace SSBHLib.Formats.Animation
{
    public enum AnimType
    {
        Transform = 1,
        Visibilty = 2,
        Material = 4,
        Camera = 5
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
