using SELib;

namespace CrossMod.IO
{
    public class IO_SEANIM
    {
        public static void ExportIOAnimation(string FileName, IOAnimation animData)
        {
            SEAnim seOut = new SEAnim(); //init new SEAnim

            foreach (IOAnimNode node in animData.Nodes) //iterate through each node
            {
                for (int i = 0; i < animData.FrameCount; i++)
                {
                    OpenTK.Vector3 pos = node.GetPosition(i, OpenTK.Vector3.Zero);
                    OpenTK.Quaternion rot = node.GetQuaternionRotation(i, new OpenTK.Quaternion(0, 0, 0, 0));
                    OpenTK.Vector3 sca = node.GetScaling(i, OpenTK.Vector3.Zero);

                    seOut.AddTranslationKey(node.Name, i, pos.X, pos.Y, pos.Z);
                    seOut.AddRotationKey(node.Name, i, rot.X, rot.Y, rot.Z, rot.W);
                    seOut.AddScaleKey(node.Name, i, sca.X, sca.Y, sca.Z);
                }
            }

            seOut.Write(FileName); //write it!
        }
    }
}
