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
                //TODO:
            }
            
            File.WriteAllText(FileName, o.ToString());
        }
    }
}
