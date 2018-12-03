using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSBHLib;
using SSBHLib.Formats;
using SSBHLib.Tools;
using CrossMod.Rendering;
using CrossMod.IO;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".numdlb")]
    public class NUMDL_Node : FileNode, IRenderableNode, IExportableModelNode
    {
        private MODL _model;

        public NUMDL_Node()
        {
            ImageKey = "model";
            SelectedImageKey = "model";
        }

        public IRenderable GetRenderableNode()
        {
            RNUMDL Model = new RNUMDL
            {
                MODL = _model
            };

            foreach (FileNode fileNode in Parent.Nodes)
            {
                if (fileNode is NUTEX_Node)
                {
                    Model.sfTextureByName.Add(((NUTEX_Node)fileNode).TexName.ToLower(), ((RTexture)((NUTEX_Node)fileNode).GetRenderableNode()).renderTexture);
                }
                if (fileNode.Text.Equals(_model.MeshString))
                {
                    Model.Model = (RModel)((NUMSHB_Node)fileNode).GetRenderableNode();
                }
                if (fileNode.Text.Equals(_model.SkeletonFileName))
                {
                    Model.Skeleton = (RSkeleton)((SKEL_Node)fileNode).GetRenderableNode();
                }
                if (fileNode.Text.Equals(_model.MaterialFileNames[0].MaterialFileName))
                {
                    Model.Material = ((MTAL_Node)fileNode)._material;
                }
            }
            if (Model.Material != null)
                Model.UpdateMaterial();
            if (Model.Skeleton != null)
                Model.UpdateBinds();
            return Model;
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
                    MeshFile = ((NUMSHB_Node)n)._mesh;
                }
                if (n.Text.Equals(_model.SkeletonFileName))
                {
                    OutModel.Skeleton = (RSkeleton)((SKEL_Node)n).GetRenderableNode();
                }
            }
            
            if(MeshFile != null)
            {
                using (SSBHVertexAccessor vertexAccessor = new SSBHVertexAccessor(MeshFile))
                {
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

                        outMesh.Vertices.AddRange(Vertices);
                        outMesh.Indicies.AddRange(vertexAccessor.ReadIndices(0, obj.IndexCount, obj));
                    }

                }
            }


            return OutModel;
        }
    }
}
