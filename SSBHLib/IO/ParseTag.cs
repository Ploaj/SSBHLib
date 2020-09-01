using System;
using System.Reflection;

namespace SSBHLib.IO
{
    public class ParseTag : Attribute
    {
        public bool Ignore { get; set; }

        public static bool ShouldSkipProperty(PropertyInfo prop)
        {
            bool shouldSkip = false;

            foreach (var attribute in prop.GetCustomAttributes(true))
            {
                if (attribute is ParseTag tag)
                {
                    if (tag.Ignore)
                        shouldSkip = true;
                }
            }

            return shouldSkip;
        }
    }
}
