using CrossMod.IO;
using CrossMod.Rendering;
using CrossMod.Rendering.Models;
using SSBHLib;
using SSBHLib.Formats;
using SSBHLib.Tools;
using System.Collections.Generic;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".numdlb")]
    public class NUMDL_Node : FileNode, IRenderableNode, IExportableModelNode
    {
        private MODL _model;
        private IRenderable renderableNode = null;

        public NUMDL_Node()
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
                    renderableNode.Material = ((MTAL_Node)fileNode).Material;
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

        public override void Open(string Path)
        {
            if (SSBH.TryParseSSBHFile(Path, out ISSBH_File ssbhFile))
            {
                if (ssbhFile is MODL)
                {
                    _model = (MODL)ssbhFile;
                }
            }
        }
        
        public IOModel GetIOModel()
        {
            IOModel OutModel = new IOModel();

            MESH MeshFile = null;

            foreach (FileNode n in Parent.Nodes)
            {
                if (n.Text.Equals(_model.MeshString))
                {
                    MeshFile = ((NUMSHB_Node)n).mesh;
                }
                if (n.Text.Equals(_model.SkeletonFileName))
                {
                    OutModel.Skeleton = (RSkeleton)((SKEL_Node)n).GetRenderableNode();
                }
            }

            Dictionary<string, int> indexByBoneName = new Dictionary<string, int>();
            if(OutModel.Skeleton != null)
            {
                for(int i = 0; i < OutModel.Skeleton.Bones.Count; i++)
                {
                    indexByBoneName.Add(OutModel.Skeleton.Bones[i].Name, i);
                }
            }
            
            if (MeshFile != null)
            {
                using (SSBHVertexAccessor vertexAccessor = new SSBHVertexAccessor(MeshFile))
                {
                    SSBHRiggingAccessor riggingAccessor = new SSBHRiggingAccessor(MeshFile);
                    foreach (MESH_Object obj in MeshFile.Objects)
                    {
                        IOMesh outMesh = new IOMesh()
                        {
                            Name = obj.Name,
                        };
                        OutModel.Meshes.Add(outMesh);

                        IOVertex[] Vertices = new IOVertex[obj.VertexCount];
                        for (int i = 0; i < Vertices.Length; i++)
                            Vertices[i] = new IOVertex();

                        foreach (MESH_Attribute attr in obj.Attributes)
                        {
                            SSBHVertexAttribute[] values = vertexAccessor.ReadAttribute(attr.AttributeStrings[0].Name, 0, obj.VertexCount, obj);

                            if (attr.AttributeStrings[0].Name.Equals("Position0"))
                            {
                                outMesh.HasPositions = true;
                                for (int i = 0; i < values.Length; i++)
                                    Vertices[i].Position = new OpenTK.Vector3(values[i].X, values[i].Y, values[i].Z);
                            }
                            if (attr.AttributeStrings[0].Name.Equals("Normal0"))
                            {
                                outMesh.HasNormals = true;
                                for (int i = 0; i < values.Length; i++)
                                    Vertices[i].Normal = new OpenTK.Vector3(values[i].X, values[i].Y, values[i].Z);
                            }
                            if (attr.AttributeStrings[0].Name.Equals("map1"))
                            {
                                outMesh.HasUV0 = true;
                                for (int i = 0; i < values.Length; i++)
                                    Vertices[i].UV0 = new OpenTK.Vector2(values[i].X, values[i].Y);
                            }
                            if (attr.AttributeStrings[0].Name.Equals("uvSet"))
                            {
                                outMesh.HasUV1 = true;
                                for (int i = 0; i < values.Length; i++)
                                    Vertices[i].UV1 = new OpenTK.Vector2(values[i].X, values[i].Y);
                            }
                            if (attr.AttributeStrings[0].Name.Equals("uvSet1"))
                            {
                                outMesh.HasUV2 = true;
                                for (int i = 0; i < values.Length; i++)
                                    Vertices[i].UV2 = new OpenTK.Vector2(values[i].X, values[i].Y);
                            }
                            if (attr.AttributeStrings[0].Name.Equals("uvSet2"))
                            {
                                outMesh.HasUV3 = true;
                                for (int i = 0; i < values.Length; i++)
                                    Vertices[i].UV3 = new OpenTK.Vector2(values[i].X, values[i].Y);
                            }
                            if (attr.AttributeStrings[0].Name.Equals("colorSet1"))
                            {
                                outMesh.HasColor = true;
                                for (int i = 0; i < values.Length; i++)
                                    Vertices[i].Color = new OpenTK.Vector4(values[i].X, values[i].Y, values[i].Z, values[i].W);
                            }
                        }

                        // Fix SingleBinds
                        if(OutModel.Skeleton != null && !obj.ParentBoneName.Equals(""))
                        {
                            int parentIndex = OutModel.Skeleton.GetBoneIndex(obj.ParentBoneName);
                            if(parentIndex != -1)
                                for(int i = 0; i < Vertices.Length; i++)
                                {
                                    Vertices[i].Position = OpenTK.Vector3.TransformPosition(Vertices[i].Position, OutModel.Skeleton.Bones[parentIndex].WorldTransform);
                                    Vertices[i].Normal = OpenTK.Vector3.TransformNormal(Vertices[i].Normal, OutModel.Skeleton.Bones[parentIndex].WorldTransform);
                                    Vertices[i].BoneIndices.X = indexByBoneName[obj.ParentBoneName];
                                    Vertices[i].BoneWeights.X = 1;
                                }
                        }

                        // Apply Rigging
                        SSBHVertexInfluence[] Influences = riggingAccessor.ReadRiggingBuffer(obj.Name, (int)obj.SubMeshIndex);
    
                        foreach(SSBHVertexInfluence influence in Influences)
                        {
                            // Some influences refer to bones that don't exist in the skeleton.
                            // _eff bones?
                            if (!indexByBoneName.ContainsKey(influence.BoneName))
                                continue;

                            if (Vertices[influence.VertexIndex].BoneWeights.X == 0)
                            {
                                Vertices[influence.VertexIndex].BoneIndices.X = indexByBoneName[influence.BoneName];
                                Vertices[influence.VertexIndex].BoneWeights.X = influence.Weight;
                            }
                            else if(Vertices[influence.VertexIndex].BoneWeights.Y == 0)
                            {
                                Vertices[influence.VertexIndex].BoneIndices.Y = indexByBoneName[influence.BoneName];
                                Vertices[influence.VertexIndex].BoneWeights.Y = influence.Weight;
                            }
                            else if(Vertices[influence.VertexIndex].BoneWeights.Z == 0)
                            {
                                Vertices[influence.VertexIndex].BoneIndices.Z = indexByBoneName[influence.BoneName];
                                Vertices[influence.VertexIndex].BoneWeights.Z = influence.Weight;
                            }
                            else if(Vertices[influence.VertexIndex].BoneWeights.W == 0)
                            {
                                Vertices[influence.VertexIndex].BoneIndices.W = indexByBoneName[influence.BoneName];
                                Vertices[influence.VertexIndex].BoneWeights.W = influence.Weight;
                            }
                        }

                        outMesh.Vertices.AddRange(Vertices);
                        outMesh.Indices.AddRange(vertexAccessor.ReadIndices(0, obj.IndexCount, obj));
                    }

                }
            }


            return OutModel;
        }
    }
}
