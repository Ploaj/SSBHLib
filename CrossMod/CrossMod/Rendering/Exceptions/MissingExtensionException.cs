using System;

namespace CrossMod.Rendering.Exceptions
{
    public class MissingExtensionException : Exception
    {
        public MissingExtensionException(string extensionName) : base($"The extension {extensionName} is not supported by the current graphics context.")
        {
        }
    }
}
