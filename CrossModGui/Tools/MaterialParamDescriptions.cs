using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;

namespace CrossModGui.Tools
{
    public sealed class MaterialParamDescriptions
    {
        public class ParamDescription
        {
            public string ParamName { get; set; }
            public string Label1 { get; set; }
            public float? Min1 { get; set; }
            public float? Max1 { get; set; }
            public string Label2 { get; set; }
            public float? Min2 { get; set; }
            public float? Max2 { get; set; }
            public string Label3 { get; set; }
            public float? Min3 { get; set; }
            public float? Max3 { get; set; }
            public string Label4 { get; set; }
            public float? Min4 { get; set; }
            public float? Max4 { get; set; }
        }

        public static MaterialParamDescriptions Instance => instance.Value;
        private static readonly Lazy<MaterialParamDescriptions> instance = new Lazy<MaterialParamDescriptions>(() => new MaterialParamDescriptions());

        public Dictionary<string, ParamDescription> ParamDescriptionsByName { get; }

        private MaterialParamDescriptions()
        {
            var test = new Lazy<ParamDescription>();
            var resource = Application.GetResourceStream(new Uri(@"/CrossModGui;component/Resources/materialparams.csv", UriKind.Relative));
            using (StreamReader reader = new StreamReader(resource.Stream))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    // Group by the name for easier searching later.
                    var records = csv.GetRecords<ParamDescription>();
                    ParamDescriptionsByName = records.ToDictionary(r => r.ParamName, r => r);
                }
            }
        }
    }
}
