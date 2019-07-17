using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static void GenerateAABB(IEnumerable<SSBHVertexAttribute> points, out SSBHVertexAttribute max, out SSBHVertexAttribute min)
        {
            max = new SSBHVertexAttribute(-float.MaxValue, -float.MaxValue, -float.MaxValue);
            min = new SSBHVertexAttribute(float.MaxValue, float.MaxValue, float.MaxValue);

            foreach (SSBHVertexAttribute p in points)
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
