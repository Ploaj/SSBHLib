using System;
using System.Collections.Generic;

namespace SSBHLib.Tools
{
    public class BoundingBoxGenerator
    {
        /// <summary>
        /// Generates a very simple Axis Aligned Bounding Box
        /// </summary>
        /// <param name="points"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        public static void GenerateAabb(IEnumerable<SsbhVertexAttribute> points, out SsbhVertexAttribute max, out SsbhVertexAttribute min)
        {
            max = new SsbhVertexAttribute(-float.MaxValue, -float.MaxValue, -float.MaxValue);
            min = new SsbhVertexAttribute(float.MaxValue, float.MaxValue, float.MaxValue);

            foreach (SsbhVertexAttribute p in points)
            {
                max.X = Math.Max(max.X, p.X);
                max.Y = Math.Max(max.Y, p.Y);
                max.Z = Math.Max(max.Z, p.Z);
                min.X = Math.Min(min.X, p.X);
                min.Y = Math.Min(min.Y, p.Y);
                min.Z = Math.Min(min.Z, p.Z);
            }
        }
    }
}
