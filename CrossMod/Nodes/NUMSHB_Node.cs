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
        private MESH _mesh;

        public NUMSHB_Node()
        {
            ImageKey = "model";
            SelectedImageKey = "model";
        }

        public override void Open(string Path)
        {
            ISSBH_File SSBHFile;
            if (SSBH.TryParseSSBHFile(Path, out SSBHFile))
            {
                if (SSBHFile is MESH)
                {
                    _mesh = (MESH)SSBHFile;
                }
            }
        }

        public IRenderable GetRenderableNode()
        {
            RModel Model = new RModel();

            RSkeleton Skeleton = null; // gonna need this eventually.....

            //Prepare Buffers
            // Merge them into 1
            // Opengl can't do multiple array buffers
            int[] BufferOffsets = new int[_mesh.VertexBuffers.Length];
            byte[] VertexBuffer = new byte[0];
            int bufferoffset = 0;
            int bufferindex = 0;
            foreach (MESH_Buffer b in _mesh.VertexBuffers)
            {
                byte[] nfinalBuffer = new byte[b.Buffer.Length + VertexBuffer.Length];
                Array.Copy(VertexBuffer, 0, nfinalBuffer, 0, VertexBuffer.Length);
                Array.Copy(b.Buffer, 0, nfinalBuffer, VertexBuffer.Length, b.Buffer.Length);
                VertexBuffer = nfinalBuffer;
                BufferOffsets[bufferindex++] = bufferoffset;
                bufferoffset += b.Buffer.Length;
            }

            Model.IndexBuffer = new SFGraphics.GLObjects.BufferObjects.BufferObject(BufferTarget.ElementArrayBuffer);
            Model.IndexBuffer.SetData(_mesh.PolygonBuffer, BufferUsageHint.StaticDraw);

            Model.VertexBuffer = new SFGraphics.GLObjects.BufferObjects.BufferObject(BufferTarget.ArrayBuffer);
            Model.VertexBuffer.SetData(VertexBuffer, BufferUsageHint.StaticDraw);
            
            foreach (MESH_Object o in _mesh.Objects)
            {
                RMesh Mesh = new RMesh();
                Model.Mesh.Add(Mesh);
                Mesh.Name = o.Name;
                Mesh.IndexCount = o.PolygonCount;
                Mesh.IndexOffset = (int)o.PolygonIndexOffset;

                // Vertex Attributes
                foreach (MESH_Attribute att in o.Attributes)
                {
                    CustomVertexAttribute a = new CustomVertexAttribute();
                    a.Name = att.AttributeStrings[0].Name;
                    a.Normalized = false;
                    a.Stride = att.BufferIndex == 1 ? o.Stride2 : o.Stride;
                    a.Offset = BufferOffsets[att.BufferIndex] + (att.BufferIndex == 0 ? o.VertexIndexOffset : o.VertexIndexOffset2) + att.BufferOffset;
                    a.Size = 3;
                    if (a.Name.Equals("map1"))
                    {
                        a.Size = 2;
                    }
                    switch (att.DataType)
                    {
                        case 0: a.Type = VertexAttribPointerType.Float; break;
                        case 2: a.Type = VertexAttribPointerType.Byte; a.Normalized = true; break; // color
                        case 5: a.Type = VertexAttribPointerType.HalfFloat; break;
                        case 8: a.Type = VertexAttribPointerType.HalfFloat; break;
                        default:
                            a.Type = VertexAttribPointerType.Float;
                            break;
                    }
                    Mesh.VertexAttributes.Add(a);
                }
            }


            return Model;
        }

    }
}
