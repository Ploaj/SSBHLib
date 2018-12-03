using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CrossMod.Rendering;
using System.Threading.Tasks;
using OpenTK;

namespace CrossMod.IO
{
    public class IO_SMD
    {
        public static void ExportIOModelAsSMD(string FileName, IOModel Model)
        {
            StringBuilder o = new StringBuilder();

            o.AppendLine("version 1");

            //skeleton
            if (Model.HasSkeleton)
            {
                o.AppendLine("nodes");
                foreach(RBone bone in Model.Skeleton.Bones)
                {
                    o.AppendLine($"{bone.ID} \"{bone.Name}\" {bone.ParentID}");
                }
                o.AppendLine("end");

                o.AppendLine("skeleton");
                o.AppendLine("time 0");
                foreach (RBone bone in Model.Skeleton.Bones)
                {
                    Vector3 Position = bone.Position;
                    Vector3 Rotation = bone.EulerRotation;
                    o.AppendLine($"{bone.ID} {Position.X} {Position.Y} {Position.Z} {Rotation.X} {Rotation.Y} {Rotation.Z}");
                }
                o.AppendLine("end");
            }

            //meshes
            if (Model.HasMeshes)
            {
                // keep track of used names so we don't have name overlap
                Dictionary<string, int> UsedNames = new Dictionary<string, int>();

                //begin triangles
                o.AppendLine("triangles");
                // go through each mesh
                foreach (IOMesh mesh in Model.Meshes)
                {
                    // append the index number for this mesh name
                    string Meshname = mesh.Name;
                    if (UsedNames.ContainsKey(mesh.Name))
                    {
                        UsedNames[mesh.Name] += 1;
                        Meshname += $"_{UsedNames[mesh.Name]}";
                    }
                    else
                        UsedNames.Add(mesh.Name, 0);
                    // add a triangle strip
                    // IOMesh are assumed to be triangles only
                    for(int i = 0; i < mesh.Indicies.Count; i+=3)
                    {
                        o.AppendLine(Meshname);
                        {
                            IOVertex v = mesh.Vertices[(int)mesh.Indicies[i]];
                            o.AppendLine($"0 {v.Position.X} {v.Position.Y} {v.Position.Z} {v.Normal.X} {v.Normal.Y} {v.Normal.Z} {v.UV0.X} {v.UV0.Y} 0");
                        }
                        {
                            IOVertex v = mesh.Vertices[(int)mesh.Indicies[i+1]];
                            o.AppendLine($"0 {v.Position.X} {v.Position.Y} {v.Position.Z} {v.Normal.X} {v.Normal.Y} {v.Normal.Z} {v.UV0.X} {v.UV0.Y} 0");
                        }
                        {
                            IOVertex v = mesh.Vertices[(int)mesh.Indicies[i+2]];
                            o.AppendLine($"0 {v.Position.X} {v.Position.Y} {v.Position.Z} {v.Normal.X} {v.Normal.Y} {v.Normal.Z} {v.UV0.X} {v.UV0.Y} 0");
                        }
                    }
                }

                // done with triangle section
                o.AppendLine("end");
            }
            
            File.WriteAllText(FileName, o.ToString());
        }
    }
}
