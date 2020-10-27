using OpenTK;

namespace CrossMod.Rendering.GlTools
{
    public static class UniqueColors
    {
        // Pick some visually distinct colors for different materials.
        //https://sashamaps.net/docs/tools/20-colors/
        private static readonly Vector3[] MaterialColors = new Vector3[]
        {
            new Vector3(230,25,75),
            new Vector3(255,255,25),
            new Vector3(70,240,240),
            new Vector3(170,255,195),
            new Vector3(0,130,200),
            new Vector3(245,130,48),
            new Vector3(128,128,128),
            new Vector3(210,245,60),
            new Vector3(250,190,212),
            new Vector3(255,215,180),
            new Vector3(220,190,255),
            new Vector3(240,50,230),
            new Vector3(145,30,180),
            new Vector3(170,110,40),
            new Vector3(60,180,75),
            new Vector3(128,128,0),
            new Vector3(0,128,128),
            new Vector3(255,250,200),
            new Vector3(128, 0, 0),
            new Vector3(0,0,128),
            new Vector3(0,0,0),
        };

        /// <summary>
        /// Selects a unique color based on the index. Out of range indices are assigned to white.
        /// </summary>
        /// <param name="index">the index into the unique color list</param>
        /// <returns>The color associated with <paramref name="index"/></returns>
        public static Vector3 IndexToColor(int index)
        {
            // TODO: This doesn't contain enough colors for some models with many materials.
            if (index >= MaterialColors.Length || index < 0)
                return Vector3.One;

            return MaterialColors[index];
        }
    }
}
