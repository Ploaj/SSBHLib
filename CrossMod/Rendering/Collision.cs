using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGenericModel;
using SFGenericModel.VertexAttributes;
using SFGraphics.GLObjects.Shaders;
using System.Collections.Generic;

namespace CrossMod.Rendering
{
    public class Attack : Collision
    {
        float Damage { get; set; }
        int Angle { get; set; }

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
        
        //from Smash 4 values for Duck Hunt Reticle color (based on player ID)
        public static Vector3[] AttackColors = new Vector3[]
        {
            new Vector3(1f, 0f, 0f),
            new Vector3(0.7843f, 0.3529f, 1f),
            new Vector3(1f, 0.7843f, 0.7843f),
            new Vector3(0.7843f, 0.7059f, 0f),
            new Vector3(1f, 0.4706f, 0f),
            new Vector3(0f, 1f, 0.8431f),
            new Vector3(0.7843f, 0f, 1f),
            new Vector3(0.3765f, 0.2863f, 0.5294f),
        };
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

    //this is probably a bad idea. Later I need to separate collision from shape drawing
    public class Collision : GenericMesh<Vector4>
    {
        public ulong Bone { get; set; }
        public float Size { get; set; }
        public Vector3 Pos { get; set; }
        public Vector3 Pos2 { get; set; }
        public Shape ShapeType { get; set; }
        public bool Enabled { get; set; }

        public static Vector4 DefaultColor { get; set; } = new Vector4(1, 1, 1, 1);

        public static List<Vector4> UnitSphere { get; set; }
        public static List<Vector4> UnitCapsule { get; set; }

        public Collision(ulong bone, float size, Vector3 pos)
            : base(UnitSphere, PrimitiveType.TriangleStrip)
        {
            Bone = bone;
            Pos = pos;
            Pos2 = Vector3.Zero;
            Size = size;
            ShapeType = Shape.sphere;
            Enabled = true;
        }
        public Collision(ulong bone, float size, Vector3 pos, Vector3 pos2)
            : base(UnitCapsule, PrimitiveType.TriangleStrip)
        {
            Bone = bone;
            Pos = pos;
            Pos2 = pos2;
            Size = size;
            ShapeType = Shape.capsule;
            Enabled = true;
        }

        public override List<VertexAttribute> GetVertexAttributes()
        {
            return new List<VertexAttribute>()
            {
                new VertexFloatAttribute("point", ValueCount.Four, VertexAttribPointerType.Float)
            };
        }

        public enum Shape
        {
            sphere,
            aabb,
            capsule
        }
    }
}
