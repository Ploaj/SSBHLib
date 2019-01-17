using CrossMod.IO;
using CrossMod.Rendering;
using CrossMod.Rendering.Models;
using SSBHLib;
using SSBHLib.Formats.Meshes;
using SSBHLib.Formats;
using SSBHLib.Formats.Materials;
using SSBHLib.Tools;
using System.Collections.Generic;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".numdlb")]
    public class NUMDL_Node : FileNode, IRenderableNode, IExportableModelNode
    {
        private MODL _model;
        private IRenderable renderableNode = null;

        public NUMDL_Node(string path): base(path)
        {
            ImageKey = "model";
            SelectedImageKey = "model";
        }

        public IRenderable GetRenderableNode()
        {
            // Don't initialize more than once.
            // We'll assume the context isn't destroyed.
            if (renderableNode == null)
                renderableNode = CreateRenderableModel();

            if(renderableNode is RNUMDL MDL)
            {
                if(MDL.Skeleton != null)
                {
                    MDL.Skeleton.Reset();
                }
            }

            return renderableNode;
        }

        private IRenderable CreateRenderableModel()
        {
            RNUMDL renderableNode = new RNUMDL
            {
                MODL = _model
            };

            NUMSHB_Node modelNode = null;
            NUHLPB_Node helperNode = null;
            foreach (FileNode fileNode in Parent.Nodes)
            {
                if (fileNode is NUHLPB_Node)
                {
                    helperNode = (NUHLPB_Node)fileNode;
                }
                if (fileNode is NUTEX_Node nutexNode)
                {
                    var texture = (RTexture)nutexNode.GetRenderableNode();
                    // TODO: Why are there empty streams?
                    if (nutexNode.TexName != null)
                        renderableNode.sfTextureByName.Add(nutexNode.TexName.ToLower(), texture.renderTexture);
                }
                if (fileNode.Text.Equals(_model.MeshString))
                {
                    modelNode = (NUMSHB_Node)fileNode;
                }
                if (fileNode.Text.Equals(_model.SkeletonFileName))
                {
                    renderableNode.Skeleton = (RSkeleton)((SKEL_Node)fileNode).GetRenderableNode();
                }
                if (fileNode.Text.Equals(_model.MaterialFileNames[0].MaterialFileName))
                {
                    renderableNode.Material = ((MATL_Node)fileNode).Material;
                }
            }

            if(modelNode != null)
                renderableNode.Model = modelNode.GetRenderModel(renderableNode.Skeleton);
            if (renderableNode.Material != null)
                renderableNode.UpdateMaterial();
            if (renderableNode.Skeleton != null)
            {
                if (helperNode != null)
                    helperNode.AddToRenderSkeleton(renderableNode.Skeleton);
                renderableNode.UpdateBinds();
            }

            return renderableNode;
        }

        public override void Open()
        {
            if (SSBH.TryParseSSBHFile(AbsolutePath, out ISSBH_File ssbhFile))
            {
                if (ssbhFile is MODL)
                {
                    _model = (MODL)ssbhFile;
                }
            }
        }
        
        public IOModel GetIOModel()
        {
            IOModel outModel = new IOModel();

            MESH meshFile = null;
            MATL materialFile = null;

            foreach (FileNode n in Parent.Nodes)
            {
                if (n.Text.Equals(_model.MeshString))
                {
                    meshFile = ((NUMSHB_Node)n).mesh;
                }
                if (n.Text.Equals(_model.SkeletonFileName))
                {
                    outModel.Skeleton = (RSkeleton)((SKEL_Node)n).GetRenderableNode();
                }
                if (n.Text.Equals(_model.MaterialFileNames[0].MaterialFileName))
                {
                    materialFile = ((MATL_Node)n).Material;
                }
            }

            Dictionary<string, int> indexByBoneName = new Dictionary<string, int>();
            if(outModel.Skeleton != null)
            {
                for(int i = 0; i < outModel.Skeleton.Bones.Count; i++)
                {
                    indexByBoneName.Add(outModel.Skeleton.Bones[i].Name, i);
                }
            }

            Dictionary<string, int> materialNameToIndex = new Dictionary<string, int>();
            if(materialFile != null)
            {
                int materialIndex = 0;
                foreach(var entry in materialFile.Entries)
                {
                    materialNameToIndex.Add(entry.MaterialLabel, materialIndex++);
                    IOMaterial material = new IOMaterial();
                    material.Name = entry.MaterialLabel;
                    outModel.Materials.Add(material);

                    foreach (var attr in entry.Attributes)
                    {
                        if(attr.ParamID == MatlEnums.ParamId.Texture0)
                        {
                            IOTexture dif = new IOTexture();
                            dif.Name = attr.DataObject.ToString();
                            material.DiffuseTexture = dif;
                        }
                    }
                }
            }
            
            if (meshFile != null)
            {
                using (SSBHVertexAccessor vertexAccessor = new SSBHVertexAccessor(meshFile))
                {
                    SSBHRiggingAccessor riggingAccessor = new SSBHRiggingAccessor(meshFile);
                    foreach (MeshObject obj in meshFile.Objects)
                    {
                        IOMesh outMesh = new IOMesh()
                        {
                            Name = obj.Name,
                        };
                        outModel.Meshes.Add(outMesh);
                        
                        // get material
                        if(materialFile != null)
                        {
                            foreach(var entry in _model.ModelEntries)
                            {
                                if(entry.MeshName.Equals(obj.Name) && entry.SubIndex == obj.SubMeshIndex)
                                {
                                    outMesh.MaterialIndex = materialNameToIndex[entry.MaterialName];
                                    break;
                                }
                            }
                        }

                        IOVertex[] vertices = new IOVertex[obj.VertexCount];
                        for (int i = 0; i < vertices.Length; i++)
                            vertices[i] = new IOVertex();

                        foreach (MeshAttribute attr in obj.Attributes)
                        {
                            SSBHVertexAttribute[] values = vertexAccessor.ReadAttribute(attr.AttributeStrings[0].Name, 0, obj.VertexCount, obj);

                            if (attr.AttributeStrings[0].Name.Equals("Position0"))
                            {
                                outMesh.HasPositions = true;
                                for (int i = 0; i < values.Length; i++)
                                    vertices[i].Position = new OpenTK.Vector3(values[i].X, values[i].Y, values[i].Z);
                            }
                            if (attr.AttributeStrings[0].Name.Equals("Normal0"))
                            {
                                outMesh.HasNormals = true;
                                for (int i = 0; i < values.Length; i++)
                                    vertices[i].Normal = new OpenTK.Vector3(values[i].X, values[i].Y, values[i].Z);
                            }

                            // Flip UVs vertically for export.
                            if (attr.AttributeStrings[0].Name.Equals("map1"))
                            {
                                outMesh.HasUV0 = true;
                                for (int i = 0; i < values.Length; i++)
                                    vertices[i].UV0 = new OpenTK.Vector2(values[i].X, 1 - values[i].Y);
                            }
                            if (attr.AttributeStrings[0].Name.Equals("uvSet"))
                            {
                                outMesh.HasUV1 = true;
                                for (int i = 0; i < values.Length; i++)
                                    vertices[i].UV1 = new OpenTK.Vector2(values[i].X, 1 - values[i].Y);
                            }
                            if (attr.AttributeStrings[0].Name.Equals("uvSet1"))
                            {
                                outMesh.HasUV2 = true;
                                for (int i = 0; i < values.Length; i++)
                                    vertices[i].UV2 = new OpenTK.Vector2(values[i].X, 1 - values[i].Y);
                            }
                            if (attr.AttributeStrings[0].Name.Equals("uvSet2"))
                            {
                                outMesh.HasUV3 = true;
                                for (int i = 0; i < values.Length; i++)
                                    vertices[i].UV3 = new OpenTK.Vector2(values[i].X, 1 - values[i].Y);
                            }
                            if (attr.AttributeStrings[0].Name.Equals("colorSet1"))
                            {
                                outMesh.HasColor = true;
                                for (int i = 0; i < values.Length; i++)
                                    vertices[i].Color = new OpenTK.Vector4(values[i].X, values[i].Y, values[i].Z, values[i].W) / 127f;
                            }
                        }

                        // Fix SingleBinds
                        if(outModel.Skeleton != null && !obj.ParentBoneName.Equals(""))
                        {
                            int parentIndex = outModel.Skeleton.GetBoneIndex(obj.ParentBoneName);
                            if(parentIndex != -1)
                                for(int i = 0; i < vertices.Length; i++)
                                {
                                    vertices[i].Position = OpenTK.Vector3.TransformPosition(vertices[i].Position, outModel.Skeleton.Bones[parentIndex].WorldTransform);
                                    vertices[i].Normal = OpenTK.Vector3.TransformNormal(vertices[i].Normal, outModel.Skeleton.Bones[parentIndex].WorldTransform);
                                    vertices[i].BoneIndices.X = indexByBoneName[obj.ParentBoneName];
                                    vertices[i].BoneWeights.X = 1;
                                    outMesh.HasBoneWeights = true;
                                }
                        }

                        // Apply Rigging
                        SSBHVertexInfluence[] influences = riggingAccessor.ReadRiggingBuffer(obj.Name, (int)obj.SubMeshIndex);
    
                        foreach(SSBHVertexInfluence influence in influences)
                        {
                            outMesh.HasBoneWeights = true;

                            // Some influences refer to bones that don't exist in the skeleton.
                            // _eff bones?
                            if (!indexByBoneName.ContainsKey(influence.BoneName))
                                continue;

                            if (vertices[influence.VertexIndex].BoneWeights.X == 0)
                            {
                                vertices[influence.VertexIndex].BoneIndices.X = indexByBoneName[influence.BoneName];
                                vertices[influence.VertexIndex].BoneWeights.X = influence.Weight;
                            }
                            else if(vertices[influence.VertexIndex].BoneWeights.Y == 0)
                            {
                                vertices[influence.VertexIndex].BoneIndices.Y = indexByBoneName[influence.BoneName];
                                vertices[influence.VertexIndex].BoneWeights.Y = influence.Weight;
                            }
                            else if(vertices[influence.VertexIndex].BoneWeights.Z == 0)
                            {
                                vertices[influence.VertexIndex].BoneIndices.Z = indexByBoneName[influence.BoneName];
                                vertices[influence.VertexIndex].BoneWeights.Z = influence.Weight;
                            }
                            else if(vertices[influence.VertexIndex].BoneWeights.W == 0)
                            {
                                vertices[influence.VertexIndex].BoneIndices.W = indexByBoneName[influence.BoneName];
                                vertices[influence.VertexIndex].BoneWeights.W = influence.Weight;
                            }
                        }

                        outMesh.Vertices.AddRange(vertices);
                        outMesh.Indices.AddRange(vertexAccessor.ReadIndices(0, obj.IndexCount, obj));
                    }

                }
            }


            return outModel;
        }
    }
}
