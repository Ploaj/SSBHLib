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

        public void RenderAttack(Shader shader, Matrix4 boneTransform)
        {
            if (!Enabled)
                return;

            shader.SetMatrix4x4("bone", ref boneTransform);
            shader.SetVector3("offset", Pos);

            Draw(shader, null);
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

    public class Collision : GenericMesh<Vector3>
    {
        public ulong Bone { get; set; }
        public float Size { get; set; }
        public Vector3 Pos { get; set; }
        public Vector3 Pos2 { get; set; }
        public Shape ShapeType { get; set; }
        public bool Enabled { get; set; }

        public Collision(ulong bone, float size, Vector3 pos)
            : base(DefaultSpherePositions(size), PrimitiveType.TriangleStrip)
        {
            Bone = bone;
            Pos = pos;
            Pos2 = Vector3.Zero;
            Size = size;
            ShapeType = Shape.sphere;
            Enabled = true;
        }
        public Collision(ulong bone, float size, Vector3 pos, Vector3 pos2)
            : base(DefaultSpherePositions(size), PrimitiveType.TriangleStrip)
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

        /// <summary>
        /// Renders the outline of the collision
        /// </summary>
        /// <param name="shader"></param>
        public virtual void Render(Shader shader)
        {
            if (!Enabled)
                return;
            //TODO: also this whole thing
            Draw(shader, null);
        }

        public enum Shape
        {
            sphere,
            aabb,
            capsule
        }

        private static List<Vector3> DefaultSpherePositions(float size, int precision = 30)
        {
            List<Vector3> vertices = SFShapes.ShapeGenerator.GetSpherePositions(Vector3.Zero, size, 20).Item1;
            //Later down the road, make my own method so I can support capsules
            return vertices;
        }
    }
}
