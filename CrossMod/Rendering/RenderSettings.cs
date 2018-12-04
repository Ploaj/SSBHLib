namespace CrossMod.Rendering
{
    public static class RenderSettings
    {
        public static bool useDebugShading = false;

        public static bool useDittoForm = false;

        public static bool enableDiffuse = true;
        public static bool enableSpecular = true;

        public static bool enableWireframe = false;

        public static int renderMode = 0;

        public static OpenTK.Vector4 renderChannels = new OpenTK.Vector4(1);

        public static long paramId = 0;
    }
}
