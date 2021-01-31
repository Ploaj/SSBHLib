using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossModGui.Tools
{
    public sealed class MaterialParamDescriptions
    {
        public static MaterialParamDescriptions Instance => instance.Value;
        private static readonly Lazy<MaterialParamDescriptions> instance = new Lazy<MaterialParamDescriptions>(() => new MaterialParamDescriptions());

        public Dictionary<string, string> ParamDescriptionByName { get; }

        public string GetDescriptionText(string paramName)
        {
            if (ParamDescriptionByName.TryGetValue(paramName, out string? description))
            {
                return $"{paramName} ({description})";
            }
            else
            {
                return paramName;
            }
        }

        public class ParamDescription
        {
            public string ParamName { get; set; }
            public string Description { get; set; }
        }

        private MaterialParamDescriptions()
        {
            // Group by the name for easier searching later.
            var records = ResourceCsvReading.ReadResourceCsv<ParamDescription>(@"/CrossModGui;component/Resources/materialparameterdescriptions.csv");
            ParamDescriptionByName = records.ToDictionary(r => r.ParamName, r => r.Description);
        }
    }
}
