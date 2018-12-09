using System.Text;
using System.Collections.Generic;
using System.IO;

namespace CrossMod.IO
{
    public class IO_OBJ
    {
        public static void ExportIOModelAsOBJ(string FileName, IOModel Model)
        {
            StringBuilder o = new StringBuilder();

            // keep track of used names so we don't have name overlap
            Dictionary<string, int> UsedNames = new Dictionary<string, int>();


            // uuuggh
            int VertexCount = 1;
            foreach (IOMesh mesh in Model.Meshes)
            {
                // append the index number for this mesh name
                string Meshname = mesh.Name;
                if (UsedNames.ContainsKey(mesh.Name))
                {
                    UsedNames[mesh.Name] += 1;
                    o.AppendLine($"g _{UsedNames[mesh.Name]}");
                }
                else
                {
                    UsedNames.Add(mesh.Name, 0);
                    o.AppendLine($"o {Meshname}");
                    o.AppendLine($"g _{UsedNames[mesh.Name]}");
                }

                foreach (IOVertex v in mesh.Vertices)
                {
                    o.AppendLine($"v {v.Position.X} {v.Position.Y} {v.Position.Z}");
                    o.AppendLine($"vn {v.Normal.X} {v.Normal.Y} {v.Normal.Z}");
                    o.AppendLine($"vt {v.UV0.X} {v.UV0.Y}");
                }
                for(int i = 0; i < mesh.Indices.Count; i+=3)
                {
                    o.AppendLine($"f {VertexCount+mesh.Indices[i]}/{VertexCount + mesh.Indices[i]}/{VertexCount + mesh.Indices[i]}" +
                        $" {VertexCount + mesh.Indices[i+1]}/{VertexCount + mesh.Indices[i+1]}/{VertexCount + mesh.Indices[i+1]}" +
                        $" {VertexCount + mesh.Indices[i+2]}/{VertexCount + mesh.Indices[i+2]}/{VertexCount + mesh.Indices[i+2]}");
                }
                VertexCount += mesh.Vertices.Count;
            }

            File.WriteAllText(FileName, o.ToString());
        }
    }
}
