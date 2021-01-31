using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossModGui.Tools
{
    public sealed class CustomVectorDescriptions
    {
        public class CustomVectorParamDescription
        {
            public string ParamName { get; set; } = "";
            public string Label1 { get; set; } = "";
            public float? Min1 { get; set; }
            public float? Max1 { get; set; }
            public string Label2 { get; set; } = "";
            public float? Min2 { get; set; }
            public float? Max2 { get; set; }
            public string Label3 { get; set; } = "";
            public float? Min3 { get; set; }
            public float? Max3 { get; set; }
            public string Label4 { get; set; } = "";
            public float? Min4 { get; set; }
            public float? Max4 { get; set; }
        }

        public static CustomVectorDescriptions Instance => instance.Value;
        private static readonly Lazy<CustomVectorDescriptions> instance = new Lazy<CustomVectorDescriptions>(() => new CustomVectorDescriptions());

        public Dictionary<string, CustomVectorParamDescription> ParamDescriptionsByName { get; }

        private CustomVectorDescriptions()
        {
            // Group by the name for easier searching later.
            var records = ResourceCsvReading.ReadResourceCsv<CustomVectorParamDescription>(@"/CrossModGui;component/Resources/materialparams.csv");
            ParamDescriptionsByName = records.ToDictionary(r => r.ParamName, r => r);
        }
    }
}
