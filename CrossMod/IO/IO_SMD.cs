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
                    for(int i = 0; i < mesh.Indices.Count; i+=3)
                    {
                        o.AppendLine(Meshname);
                        {
                            IOVertex v = mesh.Vertices[(int)mesh.Indices[i]];
                            o.AppendLine($"0 {v.Position.X} {v.Position.Y} {v.Position.Z} {v.Normal.X} {v.Normal.Y} {v.Normal.Z} {v.UV0.X} {v.UV0.Y} " + CountWeights(v.BoneWeights) + " " + CreateWeightList(v, CountWeights(v.BoneWeights)));
                        }
                        {
                            IOVertex v = mesh.Vertices[(int)mesh.Indices[i+1]];
                            o.AppendLine($"0 {v.Position.X} {v.Position.Y} {v.Position.Z} {v.Normal.X} {v.Normal.Y} {v.Normal.Z} {v.UV0.X} {v.UV0.Y} " + CountWeights(v.BoneWeights) + " " + CreateWeightList(v, CountWeights(v.BoneWeights)));
                        }
                        {
                            IOVertex v = mesh.Vertices[(int)mesh.Indices[i+2]];
                            o.AppendLine($"0 {v.Position.X} {v.Position.Y} {v.Position.Z} {v.Normal.X} {v.Normal.Y} {v.Normal.Z} {v.UV0.X} {v.UV0.Y} " + CountWeights(v.BoneWeights) + " " + CreateWeightList(v, CountWeights(v.BoneWeights)));
                        }
                    }
                }

                // done with triangle section
                o.AppendLine("end");
            }
            
            File.WriteAllText(FileName, o.ToString());
        }

        private static string CreateWeightList(IOVertex v, int Count)
        {
            StringBuilder WeightList = new StringBuilder();
            for(int i = 0; i < Count; i++)
            {
                WeightList.Append($"{v.BoneIndices[i]} {v.BoneWeights[i]} ");
            }
            return WeightList.ToString();
        }

        private static int CountWeights(Vector4 Weight)
        {
            int c = 0;
            if (Weight.X != 0) c += 1; else return c;
            if (Weight.Y != 0) c += 1; else return c;
            if (Weight.Z != 0) c += 1; else return c;
            if (Weight.W != 0) c += 1; else return c;
            return c;
        }
    }
}
