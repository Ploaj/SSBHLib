using CrossMod.Rendering;
using System;
using System.Collections.Generic;

namespace CrossMod.IO
{
    class IO_DAE
    {
        public static void ExportIOModelAsDAE(string FileName, IOModel m, bool Optimize, bool ExportMaterials)
        {
            using (DAEWriter writer = new DAEWriter(FileName, Optimize))
            {
                writer.WriteAsset();

                if (m.HasMaterials && ExportMaterials)
                {
                    List<string> TextureNames = new List<string>();
                    foreach (var mat in m.Materials)
                    {
                        if (mat.DiffuseTexture != null && !TextureNames.Contains(mat.DiffuseTexture.Name))
                            TextureNames.Add(mat.DiffuseTexture.Name);
                    }
                    writer.WriteLibraryImages(TextureNames.ToArray());

                    writer.StartMaterialSection();
                    foreach (var mat in m.Materials)
                    {
                        writer.WriteMaterial(mat.Name);
                    }
                    writer.EndMaterialSection();

                    writer.StartEffectSection();
                    foreach (var mat in m.Materials)
                    {
                        writer.WriteEffect(mat.Name, mat.DiffuseTexture == null ? "" : mat.DiffuseTexture.Name);
                    }
                    writer.EndEffectSection();
                }
                else
                {
                    writer.WriteLibraryImages();
                }
                
                if (m.HasSkeleton)
                {
                    foreach (var bone in m.Skeleton.Bones)
                    {
                        float[] Transform = new float[] { bone.Transform.M11, bone.Transform.M21, bone.Transform.M31, bone.Transform.M41,
                    bone.Transform.M12, bone.Transform.M22, bone.Transform.M32, bone.Transform.M42,
                    bone.Transform.M13, bone.Transform.M23, bone.Transform.M33, bone.Transform.M43,
                    bone.Transform.M14, bone.Transform.M24, bone.Transform.M34, bone.Transform.M44 };
                        float[] InvTransform = new float[] { bone.InvWorldTransform.M11, bone.InvWorldTransform.M21, bone.InvWorldTransform.M31, bone.InvWorldTransform.M41,
                    bone.InvWorldTransform.M12, bone.InvWorldTransform.M22, bone.InvWorldTransform.M32, bone.InvWorldTransform.M42,
                    bone.InvWorldTransform.M13, bone.InvWorldTransform.M23, bone.InvWorldTransform.M33, bone.InvWorldTransform.M43,
                    bone.InvWorldTransform.M14, bone.InvWorldTransform.M24, bone.InvWorldTransform.M34, bone.InvWorldTransform.M44 };
                        writer.AddJoint(bone.Name, bone.ParentID == -1 ? "" : m.Skeleton.Bones[bone.ParentID].Name, Transform, InvTransform);
                    }
                }

                writer.StartGeometrySection();
                foreach(var mesh in m.Meshes)
                {
                    writer.StartGeometryMesh(mesh.Name);

                    if(mesh.MaterialIndex != -1)
                    {
                        writer.CurrentMaterial = m.Materials[mesh.MaterialIndex].Name;
                    }

                    // collect sources
                    List<float> Position = new List<float>();
                    List<float> Normal = new List<float>();
                    List<float> UV0 = new List<float>();
                    List<float> UV1 = new List<float>();
                    List<float> UV2 = new List<float>();
                    List<float> UV3 = new List<float>();
                    List<float> Color = new List<float>();
                    List<int[]> BoneIndices = new List<int[]>();
                    List<float[]> BoneWeights = new List<float[]>();

                    foreach (var vertex in mesh.Vertices)
                    {
                        Position.Add(vertex.Position.X); Position.Add(vertex.Position.Y); Position.Add(vertex.Position.Z);
                        Normal.Add(vertex.Normal.X); Normal.Add(vertex.Normal.Y); Normal.Add(vertex.Normal.Z);
                        UV0.Add(vertex.UV0.X); UV0.Add(vertex.UV0.Y);
                        UV1.Add(vertex.UV1.X); UV1.Add(vertex.UV1.Y);
                        UV2.Add(vertex.UV2.X); UV2.Add(vertex.UV2.Y);
                        UV3.Add(vertex.UV3.X); UV3.Add(vertex.UV3.Y);
                        Color.AddRange(new float[] { vertex.Color.X, vertex.Color.Y, vertex.Color.Z, vertex.Color.W });

                        List<int> bIndices = new List<int>();
                        List<float> bWeights = new List<float>();
                        if (vertex.BoneWeights.X > 0)
                        {
                            bIndices.Add((int)vertex.BoneIndices.X);
                            bWeights.Add(vertex.BoneWeights.X);
                        }
                        if (vertex.BoneWeights.Y > 0)
                        {
                            bIndices.Add((int)vertex.BoneIndices.Y);
                            bWeights.Add(vertex.BoneWeights.Y);
                        }
                        if (vertex.BoneWeights.Z > 0)
                        {
                            bIndices.Add((int)vertex.BoneIndices.Z);
                            bWeights.Add(vertex.BoneWeights.Z);
                        }
                        if (vertex.BoneWeights.W > 0)
                        {
                            bIndices.Add((int)vertex.BoneIndices.W);
                            bWeights.Add(vertex.BoneWeights.W);
                        }
                        BoneIndices.Add(bIndices.ToArray());
                        BoneWeights.Add(bWeights.ToArray());
                    }

                    // write sources
                    if(mesh.HasPositions)
                        writer.WriteGeometrySource(mesh.Name, DAEWriter.VERTEX_SEMANTIC.POSITION, Position.ToArray(), mesh.Indices.ToArray());

                    if (mesh.HasNormals)
                        writer.WriteGeometrySource(mesh.Name, DAEWriter.VERTEX_SEMANTIC.NORMAL, Normal.ToArray(), mesh.Indices.ToArray());

                    if (mesh.HasColor)
                        writer.WriteGeometrySource(mesh.Name, DAEWriter.VERTEX_SEMANTIC.COLOR, Color.ToArray(), mesh.Indices.ToArray());

                    if (mesh.HasUV0)
                        writer.WriteGeometrySource(mesh.Name, DAEWriter.VERTEX_SEMANTIC.TEXCOORD, UV0.ToArray(), mesh.Indices.ToArray(), 0);

                    if (mesh.HasUV1)
                        writer.WriteGeometrySource(mesh.Name, DAEWriter.VERTEX_SEMANTIC.TEXCOORD, UV1.ToArray(), mesh.Indices.ToArray(), 1);

                    if (mesh.HasUV2)
                        writer.WriteGeometrySource(mesh.Name, DAEWriter.VERTEX_SEMANTIC.TEXCOORD, UV2.ToArray(), mesh.Indices.ToArray(), 2);

                    if (mesh.HasUV3)
                        writer.WriteGeometrySource(mesh.Name, DAEWriter.VERTEX_SEMANTIC.TEXCOORD, UV3.ToArray(), mesh.Indices.ToArray(), 3);


                    if (mesh.HasBoneWeights)
                    {
                        writer.AttachGeometryController(BoneIndices, BoneWeights);
                    }

                    writer.EndGeometryMesh();
                }
                writer.EndGeometrySection();
            }
            return;

            /*COLLADA colladaFile = new COLLADA();

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
                    List<double> list_uvs2 = new List<double>();
                    List<double> list_uvs3 = new List<double>();
                    List<double> list_uvs4 = new List<double>();
                    List<double> list_colors = new List<double>();
                    foreach (IOVertex v in iomesh.Vertices)
                    {
                        list_positions.Add(v.Position.X);
                        list_positions.Add(v.Position.Y);
                        list_positions.Add(v.Position.Z);
                        list_normals.Add(v.Normal.X);
                        list_normals.Add(v.Normal.Y);
                        list_normals.Add(v.Normal.Z);
                        list_uvs.Add(v.UV0.X); list_uvs.Add(v.UV0.Y);
                        list_uvs2.Add(v.UV1.X); list_uvs2.Add(v.UV1.Y);
                        list_uvs3.Add(v.UV2.X); list_uvs3.Add(v.UV2.Y);
                        list_uvs4.Add(v.UV3.X); list_uvs4.Add(v.UV3.Y);
                    }

                    // Generate Sources
                    List<source> Sources = new List<source>();

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
                    Sources.Add(source_position);

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
                    Sources.Add(source_normal);

                    List<InputLocalOffset> InputLocalOffsets = new List<InputLocalOffset>();
                    int LocalOffset = 0;
                    int UVSet = 0;

                    // vertices

                    vertices vertices = new vertices();
                    vertices.id = g.id + "_verts";
                    vertices.input = new InputLocal[]
                    {
                    new InputLocal() { source = "#" + source_position.id, semantic = "POSITION" },
                    new InputLocal() { source = "#" + source_normal.id, semantic = "NORMAL" }
                    };
                    InputLocalOffsets.Add(new InputLocalOffset() { offset = (ulong)LocalOffset++, semantic = "VERTEX", source = "#" + vertices.id });

                    // UV0
                    if (iomesh.HasUV0)
                    {
                        source source_uv0 = new source();
                        {
                            float_array floats = new float_array();
                            floats.count = (ulong)list_uvs.Count;
                            floats.id = g.id + $"_uv{UVSet}_arr";
                            floats.Values = list_uvs.ToArray();

                            source_uv0 = CreateSource(list_uvs.Count, 2, floats.id, floats, new param[] {
                            new param() { name="S", type="float"},
                            new param() { name="T", type="float"} });
                        }
                        Sources.Add(source_uv0);
                        InputLocalOffsets.Add(new InputLocalOffset() { source = "#" + source_uv0.id, semantic = "TEXCOORD", offset = (ulong)LocalOffset++, set = (ulong)UVSet++ });
                    }
                    if (iomesh.HasUV1)
                    {
                        source source_uv0 = new source();
                        {
                            float_array floats = new float_array();
                            floats.count = (ulong)list_uvs2.Count;
                            floats.id = g.id + $"_uv{UVSet}_arr";
                            floats.Values = list_uvs2.ToArray();

                            source_uv0 = CreateSource(list_uvs2.Count, 2, floats.id, floats, new param[] {
                            new param() { name="S", type="float"},
                            new param() { name="T", type="float"} });
                        }
                        Sources.Add(source_uv0);
                        InputLocalOffsets.Add(new InputLocalOffset() { source = "#" + source_uv0.id, semantic = "TEXCOORD", offset = (ulong)LocalOffset++, set = (ulong)UVSet++ });
                    }
                    if (iomesh.HasUV2)
                    {
                        source source_uv0 = new source();
                        {
                            float_array floats = new float_array();
                            floats.count = (ulong)list_uvs3.Count;
                            floats.id = g.id + $"_uv{UVSet}_arr";
                            floats.Values = list_uvs3.ToArray();

                            source_uv0 = CreateSource(list_uvs3.Count, 2, floats.id, floats, new param[] {
                            new param() { name="S", type="float"},
                            new param() { name="T", type="float"} });
                        }
                        Sources.Add(source_uv0);
                        InputLocalOffsets.Add(new InputLocalOffset() { source = "#" + source_uv0.id, semantic = "TEXCOORD", offset = (ulong)LocalOffset++, set = (ulong)UVSet++ });
                    }
                    if (iomesh.HasUV3)
                    {
                        source source_uv0 = new source();
                        {
                            float_array floats = new float_array();
                            floats.count = (ulong)list_uvs4.Count;
                            floats.id = g.id + $"_uv{UVSet}_arr";
                            floats.Values = list_uvs4.ToArray();

                            source_uv0 = CreateSource(list_uvs4.Count, 2, floats.id, floats, new param[] {
                            new param() { name="S", type="float"},
                            new param() { name="T", type="float"} });
                        }
                        Sources.Add(source_uv0);
                        InputLocalOffsets.Add(new InputLocalOffset() { source = "#" + source_uv0.id, semantic = "TEXCOORD", offset = (ulong)LocalOffset++, set = (ulong)UVSet++ });
                    }

                    // triangles
                    triangles triangles = new triangles();
                    triangles.count = ((ulong)iomesh.Indices.Count) / 3;
                    triangles.input = InputLocalOffsets.ToArray();
                    StringBuilder trianglebuilder = new StringBuilder();
                    foreach(var i in iomesh.Indices)
                    {
                        for(int j = 0; j< InputLocalOffsets.Count; j++)
                            trianglebuilder.Append($"{i} ");
                    }
                    triangles.p = trianglebuilder.ToString();

                    // creating mesh
                    mesh geomesh = new mesh();
                    geomesh.source = Sources.ToArray();
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

            colladaFile.Save(FileName);*/
        }

        /*private static source CreateSource(int count, int stride, string arraySource, object ArrayItem, param[] Params)
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
        }*/
    }
}
