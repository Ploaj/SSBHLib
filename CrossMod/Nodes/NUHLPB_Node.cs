using SSBHLib.Formats;
using SSBHLib;
using CrossMod.Rendering;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".nuhlpb")]
    public class NUHLPB_Node : FileNode
    {
        public HLPB helperBones;

        public NUHLPB_Node(string path): base(path)
        {
            ImageKey = "skeleton";
            SelectedImageKey = "skeleton";
        }
        
        public override void Open()
        {
            if (SSBH.TryParseSSBHFile(AbsolutePath, out ISSBH_File ssbhFile))
            {
                if (ssbhFile is HLPB)
                {
                    helperBones = (HLPB)ssbhFile;
                }
            }
        }

        public void AddToRenderSkeleton(RSkeleton Skeleton)
        {
            Skeleton.HelperBone.Clear();
            foreach (HLPB_Entry entry in helperBones.Entries)
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
                    AOI = new OpenTK.Vector3(entry.AoIX, entry.AoIY, entry.AoIZ)
                });
            }
        }
    }
}
