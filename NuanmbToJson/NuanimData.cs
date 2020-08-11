using SSBHLib;
using SSBHLib.Formats.Animation;
using SSBHLib.Tools;
using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NuanmbToJson
{
    class AnimTrackData
    {
        public string Name { get; set; }

        public object[] Values { get; set; }

        public AnimTrackData(AnimTrack track, SsbhAnimTrackDecoder decoder)
        {
            Name = track.Name;
            Values = decoder.ReadTrack(track);
        }
    }

    class AnimNodeData
    {
        public string Name { get; set; }

        public AnimTrackData[] Tracks { get; set; }

        public AnimNodeData(AnimNode node, SsbhAnimTrackDecoder decoder)
        {
            Name = node.Name;
            Tracks = node.Tracks.Select(track => new AnimTrackData(track, decoder)).ToArray();
        }
    }

    class AnimGroupData
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public AnimType Type { get; set; }

        public AnimNodeData[] Nodes { get; set; }

        public AnimGroupData(AnimGroup group, SsbhAnimTrackDecoder decoder)
        {
            Type = group.Type;
            Nodes = group.Nodes.Select(node => new AnimNodeData(node, decoder)).ToArray();
        }
    }

    class NuanimData
    {
        private readonly SsbhAnimTrackDecoder decoder;

        private readonly Anim animation;

        public AnimGroupData[] Animations { get; set; }

        public NuanimData(string path)
        {
            if (Ssbh.TryParseSsbhFile(path, out Anim animation))
            {
                if (animation == null)
                    throw new NullReferenceException("Could not parse file.");

                decoder = new SsbhAnimTrackDecoder(animation);

                Animations = animation.Animations.Select(group => new AnimGroupData(group, decoder)).ToArray();
            }
        }
    }
}
