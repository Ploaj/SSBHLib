using CrossMod.Rendering;
using CrossMod.Rendering.Shapes;
using CrossMod.Tools;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using paracobNET;
using SFGraphics.Cameras;
using System;
using System.Collections.Generic;
using System.IO;

namespace CrossMod.Nodes
{
    public static class ParamNodeContainer
    {
        public static Dictionary<string, ParamFile> ParamDict { get; set; }
        public static Collision[] HitData { get; set; }
        public static CliffHangShape[] CliffHangData { get; set; }

        public static SkelNode SkelNode
        {
            set
            {
                Skel = value.GetRenderableNode() as RSkeleton;
                BoneIDs.Clear();
                // TODO: Adding "top" creates a duplicate key for some models.
                BoneIDs.Add(Hash.Hash40("top"), 0);
                for (int i = 0; i < Skel.Bones.Count; i++)
                    BoneIDs[Hash.Hash40(Skel.Bones[i].Name)] = i;
            }
        }
        private static RSkeleton Skel { get; set; }
        private static Dictionary<ulong, int> BoneIDs { get; set; }

        private static Capsule capsule = null;
        private static Sphere sphere = null;
        private static Polygon quad = null;

        static ParamNodeContainer()
        {
            ParamDict = new Dictionary<string, ParamFile>();
            BoneIDs = new Dictionary<ulong, int>();
            HitData = new Collision[0];
            CliffHangData = new CliffHangShape[0];
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

        public static void Unload()
        {
            ParamDict.Clear();
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
                ParamList list = file.Root.Nodes["hit_data"] as ParamList;
                HitData = new Collision[list.Nodes.Count];
                for (int i = 0; i < HitData.Length; i++)
                {
                    ParamStruct str = list.Nodes[i] as ParamStruct;
                    HitData[i] = new Collision((ulong)(str.Nodes["node_id"] as ParamValue).Value,
                        (float)(str.Nodes["size"] as ParamValue).Value,
                        new Vector3(
                            (float)(str.Nodes["offset1_x"] as ParamValue).Value,
                            (float)(str.Nodes["offset1_y"] as ParamValue).Value,
                            (float)(str.Nodes["offset1_z"] as ParamValue).Value),
                        new Vector3(
                            (float)(str.Nodes["offset2_x"] as ParamValue).Value,
                            (float)(str.Nodes["offset2_y"] as ParamValue).Value,
                            (float)(str.Nodes["offset2_z"] as ParamValue).Value));
                }

                list = file.Root.Nodes["cliff_hang_data"] as ParamList;
                CliffHangData = new CliffHangShape[list.Nodes.Count];
                for (int i = 0; i < CliffHangData.Length; i++)
                {
                    ParamStruct str = list.Nodes[i] as ParamStruct;
                    CliffHangData[i] = new CliffHangShape()
                    {
                        p1 = new Vector3(
                            (float)(str.Nodes["p1_x"] as ParamValue).Value,
                            (float)(str.Nodes["p1_y"] as ParamValue).Value,
                            0),
                        p2 = new Vector3(
                            (float)(str.Nodes["p2_x"] as ParamValue).Value,
                            (float)(str.Nodes["p2_y"] as ParamValue).Value,
                            0),
                    };
                }
            }
        }

        public static void Render(Camera camera)
        {
            if (Skel == null)
                return;

            if (capsule == null)
                capsule = new Capsule();

            if (sphere == null)
                sphere = new Sphere();

            var capsuleShader = ShaderContainer.GetShader("Capsule");
            var sphereShader = ShaderContainer.GetShader("Sphere");
            var polygonShader = ShaderContainer.GetShader("Polygon");


            if (RenderSettings.Instance.RenderHitCollisions)
            {
                GL.Disable(EnableCap.DepthTest);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                foreach (var hit in HitData)
                {
                    //some param files refer to bone hashes that don't exist on the skeleton
                    if (!BoneIDs.TryGetValue(hit.Bone, out int boneIndex))
                        continue;

                    Matrix4 bone = Skel.GetAnimationSingleBindsTransform(boneIndex);
                    Vector4 color = new Vector4(hit.Color, 0.3f);
                    //if (BoneIDs[hit.Bone] == 0)//special purpose HitData attached to trans or top
                    //    color = new Vector4(1, 0.3f, 0.3f, 0.3f);
                    if (hit.Pos != hit.Pos2)
                        capsule.Render(capsuleShader, hit.Size, hit.Pos, hit.Pos2, bone, camera.MvpMatrix, color);
                    else
                        sphere.Render(sphereShader, hit.Size, hit.Pos, bone, camera.MvpMatrix, color);
                }
                GL.Enable(EnableCap.DepthTest);
                GL.Disable(EnableCap.Blend);
            }

            Matrix4 transN = Skel.GetAnimationSingleBindsTransform(0).ClearRotation().ClearScale();
            int cliffHangID = RenderSettings.Instance.CliffHangID;
            if (cliffHangID >= 0 && cliffHangID < CliffHangData.Length)
            {
                GL.Disable(EnableCap.DepthTest);

                CliffHangShape shape = CliffHangData[cliffHangID];
                Vector3 v1;
                Vector3 v2;
                Vector3 v3;
                Vector3 v4;
                //this makes sure the rectangle is always front facing
                //I had trouble using OpenGL's culling mode, I don't know why
                if ((shape.p1.X - shape.p2.X) * (shape.p1.Y - shape.p2.Y) > 0)
                {
                    v1 = new Vector3(0, shape.p1.Y, shape.p1.X);
                    v2 = new Vector3(0, shape.p1.Y, shape.p2.X);
                    v3 = new Vector3(0, shape.p2.Y, shape.p2.X);
                    v4 = new Vector3(0, shape.p2.Y, shape.p1.X);
                }
                else
                {
                    v1 = new Vector3(0, shape.p1.Y, shape.p1.X);
                    v2 = new Vector3(0, shape.p2.Y, shape.p1.X);
                    v3 = new Vector3(0, shape.p2.Y, shape.p2.X);
                    v4 = new Vector3(0, shape.p1.Y, shape.p2.X);
                }
                quad = new Polygon(new List<Vector3>() { v1, v2, v3, v4 });
                quad.Render(polygonShader, transN, camera.MvpMatrix, new Vector4(Collision.IDColors[cliffHangID % 9], 1f));

                GL.Enable(EnableCap.DepthTest);
            }
        }

        public struct CliffHangShape
        {
            public Vector3 p1;
            public Vector3 p2;
        }
    }
}
