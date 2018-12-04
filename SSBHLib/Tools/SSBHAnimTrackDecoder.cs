using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSBHLib.Formats;
using SSBHLib.IO;
using System.IO;

namespace SSBHLib.Tools
{
    public class AnimTrackBool
    {
        public bool Value { get; set; } = false;
        public AnimTrackBool(bool Value)
        {
            this.Value = Value;
        }
    }

    public class SSBHAnimTrackDecoder
    {
        private ANIM AnimFile;

        public SSBHAnimTrackDecoder(ANIM AnimFile)
        {
            this.AnimFile = AnimFile;
        }

        public object[] ReadTrack(ANIM_Track Track)
        {
            List<object> Output = new List<object>();
            using (SSBHParser parser = new SSBHParser(new MemoryStream(AnimFile.Buffer)))
            {
                parser.Seek(Track.DataOffset);

                if (Track.Flags.HasFlag(ANIM_TRACKFLAGS.Visibilty))
                {
                    if ((int)Track.Flags == 0x0408)
                    {
                        int Unk_4 = parser.ReadInt16();
                        int TrackFlag = parser.ReadInt16();
                        int Unk1 = parser.ReadInt16();
                        int Unk2 = parser.ReadInt16();
                        int DataStart = parser.ReadInt32();
                        int FrameCount = parser.ReadInt32();

                        parser.Seek((int)Track.DataOffset + DataStart);
                        for (int i = 0; i < FrameCount; i++)
                        {
                            Output.Add(new AnimTrackBool(parser.ReadBits(1) == 1));
                        }
                    }
                    else
                    if ((int)Track.Flags == 0x0508)
                    {
                        Output.Add(new AnimTrackBool(parser.ReadBits(1) == 1));
                    }
                }
            }
            return Output.ToArray();
        }
    }
}
