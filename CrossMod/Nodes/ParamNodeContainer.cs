using CrossMod.Rendering;
using CrossMod.Rendering.Shapes;
using CrossMod.Tools;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using paracobNET;
using SFGraphics.Cameras;
using System.Collections.Generic;
using System.IO;

namespace CrossMod.Nodes
{
    public static class ParamNodeContainer
    {
        public static Dictionary<string, ParamFile> ParamDict { get; set; }
        public static Collision[] HitData { get; set; }

        public static SKEL_Node SkelNode
        {
            set
            {
                Skel = value.GetRenderableNode() as RSkeleton;
                BoneIDs.Clear();
                BoneIDs.Add(Hash.Hash40("top"), 0);
                for (int i = 0; i < Skel.Bones.Count; i++)
                    BoneIDs.Add(Hash.Hash40(Skel.Bones[i].Name), i);
            }
        }
        private static RSkeleton Skel { get; set; }
        private static Dictionary<ulong, int> BoneIDs { get; set; }

        private static Capsule capsule;
        private static Sphere sphere;

        static ParamNodeContainer()
        {
            ParamDict = new Dictionary<string, ParamFile>();
            BoneIDs = new Dictionary<ulong, int>();
            HitData = new Collision[0];
            capsule = new Capsule();
            sphere = new Sphere();
        }

        public static void AddFile(ParamNode node)
        {
            string name = Path.GetFileNameWithoutExtension(node.AbsolutePath);
            if (!ParamDict.ContainsKey(name))
            {
                
                ParamDict.Add(name, node.Param);
                HandleParam(name, node.Param);
            }
        }

        public static ParamFile GetFile(string name)
        {
            if (!string.IsNullOrEmpty(name) && ParamDict.ContainsKey(name))
                return ParamDict[name];
            return null;
        }

        private static void HandleParam(string name, ParamFile file)
        {
            if (name == "vl")
            {
                ParamArray array = file.Root.Nodes["hit_data"] as ParamArray;
                HitData = new Collision[array.Nodes.Length];
                for (int i = 0; i < HitData.Length; i++)
                {
                    ParamStruct str = array.Nodes[i] as ParamStruct;
                    HitData[i] = new Collision((ulong)(str.Nodes["node_id"] as ParamValue).Value,
                        (float)(str.Nodes["size"] as ParamValue).Value,
                        new OpenTK.Vector3(
                            (float)(str.Nodes["offset1_x"] as ParamValue).Value,
                            (float)(str.Nodes["offset1_y"] as ParamValue).Value,
                            (float)(str.Nodes["offset1_z"] as ParamValue).Value),
                        new OpenTK.Vector3(
                            (float)(str.Nodes["offset2_x"] as ParamValue).Value,
                            (float)(str.Nodes["offset2_y"] as ParamValue).Value,
                            (float)(str.Nodes["offset2_z"] as ParamValue).Value));
                }
            }
        }

        public static void Render(Camera camera)
        {
            if (Skel == null)
                return;
            if (RenderSettings.Instance.RenderHitCollisions)
            {
                GL.Disable(EnableCap.DepthTest);
                foreach (var hit in HitData)
                {
                    Matrix4 bone = Skel.GetAnimationSingleBindsTransform(BoneIDs[hit.Bone]);
                    Vector4 color = new Vector4(1, 1, 1, 0.3f);
                    if (hit.Pos != hit.Pos2)
                        capsule.Render(hit.Size, hit.Pos, hit.Pos2, bone, camera.MvpMatrix, color);
                    else
                        sphere.Render(hit.Size, hit.Pos, bone, camera.MvpMatrix, color);
                }
            }
        }
    }
}
