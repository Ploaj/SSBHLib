using OpenTK;

namespace CrossMod.Rendering
{
    public class Attack : Collision
    {
        public float Damage { get; set; }
        public int Angle { get; set; }
        public ulong VecTargetPos_node { get; set; }
        public Vector3 VecTargetPos_pos { get; set; }
        public float VecTargetPos_frames { get; set; }

        public Attack(ulong bone, float damage, int angle, float size, Vector3 pos) : base(bone, size, pos)
        {
            Damage = damage;
            Angle = angle;
        }
        public Attack(ulong bone, float damage, int angle, float size, Vector3 pos, Vector3 pos2) : base(bone, size, pos, pos2)
        {
            Damage = damage;
            Angle = angle;
        }

        public static Attack Default()
        {
            Attack def = new Attack(0x031ed91fca, 0, 0, 1, Vector3.Zero);
            def.Enabled = false;
            return def;
        }

        public void SetVecTargetPos(ulong node, Vector2 pos, float frames)
        {
            VecTargetPos_node = node;
            VecTargetPos_pos = new Vector3(pos);
            VecTargetPos_frames = frames;
        }
    }

    //stub class
    public class Catch : Collision
    {
        public Catch(ulong bone, float size, Vector3 pos) : base(bone, size, pos) { }

        public Catch(ulong bone, float size, Vector3 pos, Vector3 pos2) : base(bone, size, pos, pos2) { }

        public static Catch Default()
        {
            return new Catch(0x031ed91fca, 1, Vector3.Zero) { Enabled = false };
        }
    }
    
    public class Collision
    {
        public ulong Bone { get; set; }
        public float Size { get; set; }
        public Vector3 Pos { get; set; }
        public Vector3 Pos2 { get; set; }
        public Shape ShapeType { get; set; }
        public bool Enabled { get; set; }

        public Collision(ulong bone, float size, Vector3 pos)
        {
            Bone = bone;
            Pos = pos;
            Pos2 = Vector3.Zero;
            Size = size;
            ShapeType = Shape.sphere;
            Enabled = true;
        }
        public Collision(ulong bone, float size, Vector3 pos, Vector3 pos2)
        {
            Bone = bone;
            Pos = pos;
            Pos2 = pos2;
            Size = size;
            ShapeType = Shape.capsule;
            Enabled = true;
        }

        public enum Shape
        {
            sphere,
            aabb,
            capsule
        }

        //from Smash 4 values for Duck Hunt Reticle color (based on player ID)
        public static Vector3[] IDColors = new Vector3[]
        {
            new Vector3(1f, 0f, 0f),
            new Vector3(0.7843f, 0.3529f, 1f),
            new Vector3(1f, 0.7843f, 0.7843f),
            new Vector3(0.7843f, 0.7059f, 0f),
            new Vector3(1f, 0.4706f, 0f),
            new Vector3(0f, 1f, 0.8431f),
            new Vector3(0.7843f, 0f, 1f),
            new Vector3(0.3765f, 0.2863f, 0.5294f),
            new Vector3(0.6f, 0.6f, 0.6f),
        };

        public static Vector3 DefaultColor = new Vector3(1, 1, 1);
    }
}
