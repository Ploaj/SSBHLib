using System;
using CrossMod.Rendering;
using SSBHLib.Formats;
using SSBHLib;
using OpenTK;
using System.Collections.Generic;
using System.IO;
using SFGenericModel.VertexAttributes;
using OpenTK.Graphics.OpenGL;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".numshb")]
    public class NUMSHB_Node : FileNode, IRenderableNode
    {
        public MESH _mesh;

        public NUMSHB_Node()
        {
            ImageKey = "model";
            SelectedImageKey = "model";
        }

        public override void Open(string Path)
        {
            if (SSBH.TryParseSSBHFile(Path, out ISSBH_File ssbhFile))
            {
                if (ssbhFile is MESH)
                {
                    _mesh = (MESH)ssbhFile;
                }
            }
        }

        public IRenderable GetRenderableNode()
        {
            RModel Model = new RModel();

            // Merge buffers into one because Opengl can't do multiple array buffers.
            int[] bufferOffsets = new int[_mesh.VertexBuffers.Length];
            byte[] vertexBuffer = new byte[0];
            int bufferOffset = 0;
            int bufferIndex = 0;

            foreach (MESH_Buffer meshBuffer in _mesh.VertexBuffers)
            {
                byte[] nfinalBuffer = new byte[meshBuffer.Buffer.Length + vertexBuffer.Length];
                Array.Copy(vertexBuffer, 0, nfinalBuffer, 0, vertexBuffer.Length);
                Array.Copy(meshBuffer.Buffer, 0, nfinalBuffer, vertexBuffer.Length, meshBuffer.Buffer.Length);
                vertexBuffer = nfinalBuffer;
                bufferOffsets[bufferIndex++] = bufferOffset;
                bufferOffset += meshBuffer.Buffer.Length;
            }

            Model.indexBuffer = new SFGraphics.GLObjects.BufferObjects.BufferObject(BufferTarget.ElementArrayBuffer);
            Model.indexBuffer.SetData(_mesh.PolygonBuffer, BufferUsageHint.StaticDraw);

            Model.vertexBuffer = new SFGraphics.GLObjects.BufferObjects.BufferObject(BufferTarget.ArrayBuffer);
            Model.vertexBuffer.SetData(vertexBuffer, BufferUsageHint.StaticDraw);
            
            foreach (MESH_Object meshObject in _mesh.Objects)
            {
                RMesh Mesh = new RMesh();
                Model.subMeshes.Add(Mesh);
                Mesh.Name = meshObject.Name;
                Mesh.SingleBindName = meshObject.ParentBoneName;
                Mesh.IndexCount = meshObject.IndexCount;
                Mesh.IndexOffset = (int)meshObject.ElementOffset;

                if (meshObject.DrawElementType == 1)
                    Mesh.DrawElementType = DrawElementsType.UnsignedInt;

                // Vertex Attributes
                foreach (MESH_Attribute meshAttribute in meshObject.Attributes)
                {
                    CustomVertexAttribute customAttribute = new CustomVertexAttribute
                    {
                        Name = meshAttribute.AttributeStrings[0].Name,
                        Normalized = false,
                        Stride = meshAttribute.BufferIndex == 1 ? meshObject.Stride2 : meshObject.Stride,
                        Offset = bufferOffsets[meshAttribute.BufferIndex] + (meshAttribute.BufferIndex == 0 ? meshObject.VertexOffset : meshObject.VertexOffset2) + meshAttribute.BufferOffset,
                        Size = 3
                    };

                    if (customAttribute.Name.Equals("map1"))
                    {
                        customAttribute.Size = 2;
                    }

                    switch (meshAttribute.DataType)
                    {
                        case 0:
                            customAttribute.Type = VertexAttribPointerType.Float;
                            break;
                        case 2:
                            customAttribute.Type = VertexAttribPointerType.Byte; customAttribute.Normalized = true;
                            break; // color
                        case 5:
                            customAttribute.Type = VertexAttribPointerType.HalfFloat;
                            break;
                        case 8:
                            customAttribute.Type = VertexAttribPointerType.HalfFloat;
                            break;
                        default:
                            customAttribute.Type = VertexAttribPointerType.Float;
                            break;
                    }
                    Mesh.VertexAttributes.Add(customAttribute);

                    System.Diagnostics.Debug.WriteLine($"{customAttribute.Name} {customAttribute.Size}");
                }
            }


            return Model;
        }

    }
}
