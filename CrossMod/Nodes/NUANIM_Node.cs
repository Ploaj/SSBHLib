using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossMod.Rendering;
using SSBHLib;
using SSBHLib.Formats;
using SSBHLib.Tools;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".nuanmb")]
    public class NUANIM_Node : FileNode, IRenderableNode
    {
        private ANIM animation;

        public NUANIM_Node()
        {
            ImageKey = "animation";
            SelectedImageKey = "animation";
        }
        
        public override void Open(string Path)
        {
            ISSBH_File SSBHFile;
            if (SSBH.TryParseSSBHFile(Path, out SSBHFile))
            {
                if (SSBHFile is ANIM anim)
                {
                    animation = anim;
                }
            }
        }

        public IRenderable GetRenderableNode()
        {
            RAnimation renderAnimation = new RAnimation() { FrameCount = (int)animation.FrameCount };

            SSBHAnimTrackDecoder decoder = new SSBHAnimTrackDecoder(animation);

            foreach (ANIM_Group animGroup in animation.Animations)
            {
                // Visibility Animations
                if (animGroup.Type == (int)ANIM_TYPE.Visibilty)
                {
                    foreach (ANIM_Node animNode in animGroup.Nodes)
                    {
                        RVisibilityAnimation visAnim = new RVisibilityAnimation()
                        {
                            MeshName = animNode.Name
                        };
                        foreach (ANIM_Track track in animNode.Tracks)
                        {
                            if (track.Name.Equals("Visibility"))
                            {
                                object[] Visibility = decoder.ReadTrack(track);
                                
                                for (int i = 0; i < Visibility.Length; i++)
                                {
                                    visAnim.Visibility.Keys.Add(new RKey()
                                    {
                                        Frame = i,
                                        Value = (((AnimTrackBool)Visibility[i]).Value ? 1 : 0)
                                    });
                                }
                            }
                        }
                        renderAnimation.VisibilityNodes.Add(visAnim);
                    }
                }
            }
            
                

            return renderAnimation;
        }
    }
}
