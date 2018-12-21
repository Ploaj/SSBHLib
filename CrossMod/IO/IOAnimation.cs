using System.Collections.Generic;
using OpenTK;

namespace CrossMod.IO
{
    public enum IOInterpolationType
    {
        Constant,
        Linear,
        Hermite
    }

    public enum IOTrackType
    {
        POSX,
        POSY,
        POSZ,
        ROTX,
        ROTY,
        ROTZ,
        ROTW,
        SCAX,
        SCAY,
        SCAZ,
        VIS,
    }

    public enum IORotationType
    {
        Euler,
        Quaternion
    }

    public class IOAnimation
    {
        public string Name;
        public float FrameCount;
        public IORotationType RotationType { get; set; }
        
        public List<IOAnimNode> Nodes = new List<IOAnimNode>();

        public bool TryGetNodeByName(string Name, out IOAnimNode Node)
        {
            Node = null;
            foreach (IOAnimNode node in Nodes)
            {
                if (node.Name.Equals(Name))
                {
                    Node = node;
                    return true;
                }
            }
            return false;
        }

        public void AddKey(string BoneName, IOTrackType TrackType, float Frame, float Value, IOInterpolationType InterpolationType = IOInterpolationType.Linear, float In = 0, float Out = 0)
        {
            IOAnimNode Node = null;

            if(!TryGetNodeByName(BoneName, out Node))
            {
                Node = new IOAnimNode(BoneName);
                Nodes.Add(Node);
            }

            Node.AddKey(TrackType, new IOAnimKey()
            {
                Frame = Frame,
                Value = Value,
                InterpolationType = InterpolationType,
                In = In,
                Out = Out
            });
        }
    }

    public class IOAnimNode
    {
        public string Name;
        private Dictionary<IOTrackType, List<IOAnimKey>> Tracks = new Dictionary<IOTrackType, List<IOAnimKey>>();

        public IOAnimNode(string Name)
        {
            this.Name = Name;
        }

        public void AddKey(IOTrackType TrackType, IOAnimKey Key)
        {
            if (!Tracks.ContainsKey(TrackType))
                Tracks.Add(TrackType, new List<IOAnimKey>());

            List<IOAnimKey> Keys = Tracks[TrackType];
            for(int i = 0; i < Keys.Count-1; i++)
            {
                if(Keys[i].Frame <= Key.Frame && Keys[i+1].Frame >= Key.Frame)
                {
                    // insert here
                    Keys.Insert(i+1, Key);
                    return;
                }
            }
            Keys.Add(Key);
        }

        public bool TryGetValue(IOTrackType Type, float Frame, out float Value)
        {
            if (!Tracks.ContainsKey(Type))
            {
                Value = 0;
                return false;
            }
            else
            {
                List<IOAnimKey> Keys = Tracks[Type];
                if(Keys.Count == 0)
                {
                    Value = 0;
                    return false;
                }
                for(int i = 0; i < Keys.Count - 1; i++)
                {
                    // at frame
                    if (Frame == Keys[i].Frame)
                    {
                        Value = Keys[i].Value;
                        return true;
                    }

                    // need interpolation
                    if(Frame > Keys[i].Frame && Frame < Keys[i+1].Frame)
                    {
                        // TODO: implement interpolation
                    }
                }
                // last key
                Value = Keys[Keys.Count - 1].Value;
                return true;
            }
        }

        public Vector3 GetPosition(float Frame, Vector3 DefaultPositions)
        {
            return new Vector3(TryGetValue(IOTrackType.POSX, Frame, out float ValueX) ? ValueX : DefaultPositions.X,
                TryGetValue(IOTrackType.POSY, Frame, out float ValueY) ? ValueY : DefaultPositions.Y,
                TryGetValue(IOTrackType.POSZ, Frame, out float ValueZ) ? ValueZ : DefaultPositions.Z);
        }

        public Quaternion GetQuaternionRotation(float Frame, Quaternion DefaultPositions)
        {
            return new Quaternion(TryGetValue(IOTrackType.ROTX, Frame, out float ValueX) ? ValueX : DefaultPositions.X,
                TryGetValue(IOTrackType.ROTY, Frame, out float ValueY) ? ValueY : DefaultPositions.Y,
                TryGetValue(IOTrackType.ROTZ, Frame, out float ValueZ) ? ValueZ : DefaultPositions.Z,
                TryGetValue(IOTrackType.ROTW, Frame, out float ValueW) ? ValueW : DefaultPositions.W);
        }

        public Vector3 GetScaling( float Frame, Vector3 DefaultScale )
        {
            return new Vector3( TryGetValue(IOTrackType.SCAX, Frame, out float ValueX) ? ValueX: DefaultScale.X,
                TryGetValue( IOTrackType.SCAY, Frame, out float ValueY) ? ValueY : DefaultScale.Y ,
                TryGetValue( IOTrackType.SCAZ, Frame, out float ValueZ) ? ValueZ : DefaultScale.Z );
        }
    }

    public class IOAnimKey
    {
        public IOInterpolationType InterpolationType;
        public float Frame;
        public float Value;
        public float In;
        public float Out;
    }
}
