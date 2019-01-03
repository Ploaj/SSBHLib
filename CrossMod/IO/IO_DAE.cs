using CrossMod.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossMod.IO
{
    class IO_DAE
    {
        public static void ExportIOModelAsDAE(string FileName, IOModel m)
        {
            COLLADA colladaFile = new COLLADA();

            List<geometry> list_geometries = new List<geometry>(m.Meshes.Count);

            if (m.HasMeshes)
                foreach (IOMesh iomesh in m.Meshes)
                {
                    geometry g = new geometry();
                    g.name = iomesh.Name;
                    g.id = iomesh.Name + $"_{m.Meshes.IndexOf(iomesh)}";

                    List<double> list_positions = new List<double>();
                    List<double> list_normals = new List<double>();
                    List<double> list_uvs = new List<double>();
                    List<double> list_colors = new List<double>();
                    foreach (IOVertex v in iomesh.Vertices)
                    {
                        list_positions.Add(v.Position.X);
                        list_positions.Add(v.Position.Y);
                        list_positions.Add(v.Position.Z);
                        list_normals.Add(v.Normal.X);
                        list_normals.Add(v.Normal.Y);
                        list_normals.Add(v.Normal.Z);
                        list_uvs.Add(v.UV0.X);
                        list_uvs.Add(v.UV0.Y);
                    }

                    // Position
                    source source_position = new source();
                    {
                        float_array floats = new float_array();
                        floats.count = (ulong)list_positions.Count;
                        floats.id = g.id + "_pos_arr";
                        floats.Values = list_positions.ToArray();

                        source_position = CreateSource(list_positions.Count, 3, floats.id, floats, new param[] {
                        new param() { name="X", type="float"},
                        new param() { name="Y", type="float"},
                        new param() { name="Z", type="float"} });
                    }

                    // Normal
                    source source_normal = new source();
                    {
                        float_array floats = new float_array();
                        floats.count = (ulong)list_normals.Count;
                        floats.id = g.id + "_nrm_arr";
                        floats.Values = list_normals.ToArray();

                        source_normal = CreateSource(list_normals.Count, 3, floats.id, floats, new param[] {
                        new param() { name="X", type="float"},
                        new param() { name="Y", type="float"},
                        new param() { name="Z", type="float"} });
                    }

                    // UV0
                    source source_uv0 = new source();
                    {
                        float_array floats = new float_array();
                        floats.count = (ulong)list_uvs.Count;
                        floats.id = g.id + "_uv0_arr";
                        floats.Values = list_uvs.ToArray();

                        source_uv0 = CreateSource(list_uvs.Count, 2, floats.id, floats, new param[] {
                        new param() { name="S", type="float"},
                        new param() { name="T", type="float"} });
                    }

                    // vertices

                    vertices vertices = new vertices();
                    vertices.id = g.id + "_verts";
                    vertices.input = new InputLocal[]
                    {
                    new InputLocal() { source = "#" + source_position.id, semantic = "POSITION" },
                    new InputLocal() { source = "#" + source_normal.id, semantic = "NORMAL" },
                    new InputLocal() { source = "#" + source_uv0.id, semantic = "TEXCOORD" }
                    };

                    // triangles
                    triangles triangles = new triangles();
                    triangles.count = ((ulong)iomesh.Indices.Count) / 3;
                    triangles.input = new InputLocalOffset[] {
                        new InputLocalOffset(){offset = 0, semantic = "VERTEX", source = "#" + vertices.id }
                    };
                    triangles.p = string.Join(" ", iomesh.Indices);

                    // creating mesh
                    mesh geomesh = new mesh();
                    geomesh.source = new source[] { source_position, source_normal, source_uv0 };
                    geomesh.Items = new object[] { triangles };
                    geomesh.vertices = vertices;

                    g.Item = geomesh;

                    list_geometries.Add(g);
                }
            library_geometries lib_geometry = new library_geometries();
            lib_geometry.geometry = list_geometries.ToArray();


            // controllers

            List<controller> list_controller = new List<controller>();
            if(m.HasMeshes && m.HasSkeleton)
            {
                // create lists
                List<source> skinSources = new List<source>();
                List<string> boneNames = new List<string>();
                List<double> InverseBinds = new List<double>();
                foreach(RBone b in m.Skeleton.Bones)
                {
                    boneNames.Add(b.Name);
                    InverseBinds.AddRange(new double[] { b.InvWorldTransform.M11, b.InvWorldTransform.M21, b.InvWorldTransform.M31, b.InvWorldTransform.M41,
                    b.InvWorldTransform.M12, b.InvWorldTransform.M22, b.InvWorldTransform.M32, b.InvWorldTransform.M42,
                    b.InvWorldTransform.M13, b.InvWorldTransform.M23, b.InvWorldTransform.M33, b.InvWorldTransform.M43,
                    b.InvWorldTransform.M14, b.InvWorldTransform.M24, b.InvWorldTransform.M34, b.InvWorldTransform.M44,});
                }

                


                // setup controllers
                foreach (IOMesh iomesh in m.Meshes)
                {
                    controller controller = new controller()
                    {
                        id = iomesh.Name + "_" + m.Meshes.IndexOf(iomesh) + "_controller"
                    };
                    list_controller.Add(controller);

                    // create source for weights
                    List<double> weights = new List<double>();
                    List<int> bones = new List<int>();
                    List<int> boneCount = new List<int>();
                    StringBuilder build_v = new StringBuilder();
                    foreach(IOVertex v in iomesh.Vertices)
                    {
                        int bcount = 0;
                        if (v.BoneWeights.X > 0)
                        {
                            if (!weights.Contains(v.BoneWeights.X)) weights.Add(v.BoneWeights.X);
                            build_v.Append($"{(int)v.BoneIndices.X} {weights.IndexOf(v.BoneWeights.X)} ");
                            bcount++;
                        }
                        if (v.BoneWeights.Y > 0)
                        {
                            if (!weights.Contains(v.BoneWeights.Y)) weights.Add(v.BoneWeights.Y);
                            build_v.Append($"{(int)v.BoneIndices.Y} {weights.IndexOf(v.BoneWeights.Y)} ");
                            bcount++;
                        }
                        if (v.BoneWeights.Z > 0)
                        {
                            if (!weights.Contains(v.BoneWeights.Z)) weights.Add(v.BoneWeights.Z);
                            build_v.Append($"{(int)v.BoneIndices.Z} {weights.IndexOf(v.BoneWeights.Z)} ");
                            bcount++;
                        }
                        if (v.BoneWeights.W > 0)
                        {
                            if (!weights.Contains(v.BoneWeights.W)) weights.Add(v.BoneWeights.W);
                            build_v.Append($"{(int)v.BoneIndices.W} {weights.IndexOf(v.BoneWeights.W)} ");
                            bcount++;
                        }
                        boneCount.Add(bcount);
                    }


                    // skin

                    Name_array arr_name = new Name_array();
                    arr_name.count = (ulong)boneNames.Count;
                    arr_name.id = controller.id + "joints";
                    arr_name.Values = boneNames.ToArray();

                    source source_skin = CreateSource(boneNames.Count, 1, arr_name.id, arr_name, new param[] {
                        new param() { name="JOINT", type="name"} });

                    // bind

                    float_array arr_bind = new float_array();
                    arr_bind.count = (ulong)InverseBinds.Count;
                    arr_bind.id = controller.id + "binds";
                    arr_bind.Values = InverseBinds.ToArray();

                    source source_binds = CreateSource(InverseBinds.Count, 16, arr_bind.id, arr_bind, new param[] {
                        new param() { name="TRANSFORM", type="float4x4"} });

                    // weight
                    
                    source source_weight = new source();
                    {
                        float_array floats = new float_array();
                        floats.count = (ulong)weights.Count;
                        floats.id = controller.id + "_weights";
                        floats.Values = weights.ToArray();

                        source_weight = CreateSource(weights.Count, 1, floats.id, floats, new param[] {
                        new param() { name="WEIGHT", type="float"},});
                    }

                    skin skin = new skin();
                    skin.source1 = "#" + iomesh.Name + $"_{m.Meshes.IndexOf(iomesh)}";
                    skin.source = new source[] { source_skin, source_binds, source_weight};

                    skin.joints = new skinJoints()
                    {
                        input = new InputLocal[]
                        {
                            new InputLocal()
                            {
                                semantic = "JOINT",
                                source = "#" + source_skin.id
                            },
                            new InputLocal()
                            {
                                semantic = "INV_BIND_MATRIX",
                                source = "#" + source_binds.id
                            }
                        }
                    };


                    //skin weights
                    skin.vertex_weights = new skinVertex_weights();
                    skin.vertex_weights.count = (ulong)iomesh.Vertices.Count;
                    skin.vertex_weights.input = new InputLocalOffset[]
                    {
                        new InputLocalOffset()
                        {
                            semantic = "JOINT",
                            source = "#" + source_skin.id,
                            offset = 0
                        },
                        new InputLocalOffset()
                        {
                            semantic = "WEIGHT",
                            source = "#" + source_weight.id,
                            offset = 1
                        }
                    };
                    skin.vertex_weights.vcount = string.Join(" ", boneCount);
                    skin.vertex_weights.v = build_v.ToString();

                    controller.Item = skin;
                }
            }
            library_controllers lib_controllers = new library_controllers();
            lib_controllers.controller = list_controller.ToArray();


            // scene nodes

            List<node> scene_nodes = new List<node>();
            int visual_index = 0;
            if (m.HasSkeleton)
            {
                Dictionary<RBone, node> boneToNode = new Dictionary<RBone, node>();
                foreach (RBone b in m.Skeleton.Bones)
                {
                    // create bone node
                    node node = new node();
                    node.name = b.Name;
                    node.id = "bone" + visual_index++;
                    node.sid = b.Name;
                    node.type = NodeType.JOINT;

                    // add transform
                    matrix mat = new matrix()
                    {
                        Values = new double[] { b.Transform.M11, b.Transform.M21 , b.Transform.M31 , b.Transform.M41,
                        b.Transform.M12, b.Transform.M22 , b.Transform.M32 , b.Transform.M42,
                        b.Transform.M13, b.Transform.M23 , b.Transform.M33 , b.Transform.M43,
                        b.Transform.M14, b.Transform.M24 , b.Transform.M34 , b.Transform.M44,}
                    };
                    node.ItemsElementName = new ItemsChoiceType2[] { ItemsChoiceType2.matrix };
                    node.Items = new object[] { mat };

                    boneToNode.Add(b, node);
                }
                // deal with parenting
                foreach (var b in m.Skeleton.Bones)
                {
                    if (b.ParentID == -1)
                    {
                        scene_nodes.Add(boneToNode[b]);
                    }
                    else
                    {
                        if (boneToNode[m.Skeleton.Bones[b.ParentID]].node1 == null)
                            boneToNode[m.Skeleton.Bones[b.ParentID]].node1 = new node[0];
                        node[] parentnode = boneToNode[m.Skeleton.Bones[b.ParentID]].node1;
                        Array.Resize<node>(ref parentnode, parentnode.Length + 1);
                        parentnode[parentnode.Length - 1] = boneToNode[b];
                        boneToNode[m.Skeleton.Bones[b.ParentID]].node1 = parentnode;
                    }
                }
            }
            if (m.HasMeshes)
            {
                foreach (IOMesh iomesh in m.Meshes)
                {
                    node node = new node() {
                        id = "mesh" + visual_index++,
                        name = iomesh.Name,
                        type = NodeType.NODE
                    };

                    if (m.HasSkeleton)
                    {
                        instance_controller controller = new instance_controller()
                        {
                            url = "#" + iomesh.Name + "_" + m.Meshes.IndexOf(iomesh) + "_controller"
                        };
                        controller.skeleton = new string[] { "#bone0" };
                        node.instance_controller = new instance_controller[] { controller };
                    }
                    scene_nodes.Add(node);
                }
            }

            // visual scene root
            library_visual_scenes scenes = new library_visual_scenes();
            scenes.visual_scene = new visual_scene[] {
                new visual_scene(){
                    id = "visualscene0",
                    name = "rdmscene"
            } };
            scenes.visual_scene[0].node = scene_nodes.ToArray();


            // scene
            COLLADAScene scene = new COLLADAScene();
            scene.instance_visual_scene = new InstanceWithExtra()
            {
                url = "#visualscene0"
            };

            // putting it all together
            colladaFile.Items = new object[] { lib_geometry, lib_controllers, scenes };
            colladaFile.scene = scene;

            colladaFile.Save(FileName);
        }

        private static source CreateSource(int count, int stride, string arraySource, object ArrayItem, param[] Params)
        {
            source source = new source();

            source.id = arraySource + "_base";
            source.Item = ArrayItem;

            source.technique_common = new sourceTechnique_common();
            source.technique_common.accessor = new accessor();
            source.technique_common.accessor.source = "#" + arraySource;
            source.technique_common.accessor.count = (ulong)(count / stride);
            source.technique_common.accessor.stride = (ulong)stride;
                source.technique_common.accessor.param = Params;
            return source;
        }
    }
}
