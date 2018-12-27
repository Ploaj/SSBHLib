using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ANIMCMD.Simulator
{
    public class ACMD_Runner
    {
        private float Frame { get; set; } = 1;
        private float AsynchronusFrame { get; set; } = 1;


        private bool Executing { get; set; } = false;


        private Dictionary<string, MethodInfo> Methods = new Dictionary<string, MethodInfo>();

        public float FrameSpeed { get; set; } = 1;

        private Dictionary<int, Hitbox> Hitboxes = new Dictionary<int, Hitbox>();

        public int HitboxCount { get { return Hitboxes.Count; } }

        public ACMD_Runner()
        {
            Init();
        }

        private void Init()
        {
            MethodInfo[] methods = GetType().GetMethods();

            Methods.Clear();
            foreach (MethodInfo info in methods)
                Methods.Add(info.Name, info);
        }
        
        public Hitbox GetHitbox(int HitboxIndex)
        {
            if (Hitboxes.ContainsKey(HitboxIndex))
                return Hitboxes[HitboxIndex];
            else
                return null;
        }

        public virtual void ProcessFrame()
        {

        }

        public void SetFrame(float Frame)
        {
            Clear();
            this.Frame = 1;
            for (int i = 0; i < Frame; i++)
                NextFrame();
        }

        public void NextFrame()
        {
            Frame += 1;
            ProcessFrame();
        }

        public void Clear()
        {
            Hitboxes.Clear();
            FrameSpeed = 1;
        }

        public object RunFunction(string FunctionName, object[] Parameters)
        {
            if (Methods.ContainsKey(FunctionName))
            {
                MethodInfo method = Methods[FunctionName];

                ParameterInfo[] parameters = method.GetParameters();

                if(Parameters.Length < parameters.Length)
                {
                    System.Diagnostics.Debug.WriteLine("Not Enough Parameters " + FunctionName + " " + Parameters.Length);
                    return null;
                }

                object[] FinalParameters = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    Type type = parameters[i].ParameterType;

                    TypeConverter converter = TypeDescriptor.GetConverter(type);

                    FinalParameters[i] = converter.ConvertFromString(null,
                        CultureInfo.InvariantCulture, (string)Parameters[i]);
                }

                return method.Invoke(this, FinalParameters);
            }
            else
            {
                //System.Diagnostics.Debug.WriteLine("Unknown Function " + FunctionName + " " + Parameters.Length);
                return null;
            }
        }


        public bool is_excute()
        {
            return Executing;
        }

        public void frame(float frameindex)
        {
            Executing = (frameindex == Frame);
            AsynchronusFrame = frameindex;
        }

        public void wait(float framecount)
        {
            Executing = (Frame == AsynchronusFrame + framecount);
        }

        public void FT_MOTION_RATE(float Speed)
        {
            FrameSpeed = Speed;
        }

        public void ATTACK(int ID, int Part, long BoneHash, float Damage, int Angle, int KBG, int WKB, int BKB, float Size, float X, float Y, float Z)
        {
            Hitboxes.Add(ID, new Hitbox()
            {
                BoneCRC = (uint)BoneHash,
                Damage = Damage,
                Size = Size,
                X = X,
                Y = Y,
                Z = Z
            });
        }

        #region fighter module
        public void method_A()
        {
            Hitboxes.Clear();
        }

        public void method_23() // set bit
        {

        }

        public void method_25() // clear bit
        {

        }

        #endregion
    }
}
