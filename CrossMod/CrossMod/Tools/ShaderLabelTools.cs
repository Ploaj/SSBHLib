using System;

namespace CrossMod.Tools
{
    public static class ShaderLabelTools
    {
        public static string GetRenderPass(string? shaderLabel)
        {
            if (string.IsNullOrEmpty(shaderLabel))
                return "";

            return shaderLabel[Math.Min(25, shaderLabel.Length)..];
        }

        public static string WithRenderPass(string? shaderLabel, string renderPass)
        {
            if (string.IsNullOrEmpty(shaderLabel))
                return $"_{renderPass}";

            return $"{shaderLabel[..24]}_{renderPass}";
        }
    }
}
