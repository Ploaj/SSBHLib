using SSBHLib.Formats;
using SSBHLib;
using CrossMod.Rendering;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".nuhlpb")]
    public class NuhlpbNode : FileNode
    {
        public Hlpb helperBones;

        public NuhlpbNode(string path): base(path)
        {
            ImageKey = "skeleton";
        }
        
        public override void Open()
        {
            Ssbh.TryParseSsbhFile(AbsolutePath, out helperBones);
        }

        public void AddToRenderSkeleton(RSkeleton Skeleton)
        {
            Skeleton.HelperBone.Clear();
            foreach (HlpbRotateInterpolation entry in helperBones.InterpolationEntries)
            {
                Skeleton.HelperBone.Add(new RHelperBone()
                {
                    HelperBoneName = entry.DriverBoneName,
                    WatcherBone = entry.ParentBoneName,
                    ParentBone = entry.BoneName,
                    MinRange = new OpenTK.Vector3(entry.MinRangeX, entry.MinRangeY, entry.MinRangeZ),
                    MaxRange = new OpenTK.Vector3(entry.MaxRangeX, entry.MaxRangeY, entry.MaxRangeZ),
                    WatchRotation = new OpenTK.Quaternion(entry.Quat1X, entry.Quat1Y, entry.Quat1Z, entry.Quat1W),
                    HelperTargetRotation = new OpenTK.Quaternion(entry.Quat2X, entry.Quat2Y, entry.Quat2Z, entry.Quat2W),
                    Aoi = new OpenTK.Vector3(entry.AoIx, entry.AoIy, entry.AoIz)
                });
            }
        }
    }
}
