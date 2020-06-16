using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
namespace CrossMod.Rendering
{
    // Taken from Smash Forge - Smash Forge/Rendering/Drawing/FloorDrawing.cs
    public static class FloorDrawing
    {
        // TODO - Setup scale settings in RenderSettings
        private const float Scale = 70f;
        public static void DrawAxis(Matrix4 mvpMatrix)
        {
            GL.UseProgram(0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref mvpMatrix);

            // objects shouldn't show through opaque parts of floor
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.LineWidth(1f);
            GL.Disable(EnableCap.DepthTest);
            GL.Color3(Color.White);

            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(-Scale, 0f, 0);
            GL.Vertex3(Scale, 0f, 0);
            GL.Vertex3(0, 0f, -Scale);
            GL.Vertex3(0, 0f, Scale);
            GL.End();

            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.DepthTest);

            // Draw X line
            GL.Color3(Color.Olive);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0, 0f, 0f);
            GL.Vertex3(0, 0f, 5f);
            GL.End();

            // Draw Y line
            GL.Color3(Color.LightGray);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0, 5, 0);
            GL.Vertex3(0, 0, 0);
            GL.End();


            // Draw Z line
            GL.Color3(Color.OrangeRed);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0f, 0f, 0);
            GL.Vertex3(5f, 0f, 0);
            GL.End();

            GL.Enable(EnableCap.DepthTest);
        }

        public static void DrawFloor(Matrix4 mvpMatrix)
        {
            GL.UseProgram(0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref mvpMatrix);

            // objects shouldn't show through opaque parts of floor
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.LineWidth(1f);
            GL.Color3(Color.White);
            GL.Begin(PrimitiveType.Lines);
            for (var i = -Scale / 2; i <= Scale / 2; i++)
            {
                if (i != 0)
                {
                    GL.Vertex3(-Scale, 0f, i * 2);
                    GL.Vertex3(Scale, 0f, i * 2);
                    GL.Vertex3(i * 2, 0f, -Scale);
                    GL.Vertex3(i * 2, 0f, Scale);
                }
            }

            GL.End();
        }
    }
}
