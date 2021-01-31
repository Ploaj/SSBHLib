using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Linq;

namespace CrossModGui.Tools
{
    public static class ResourceCsvReading
    {
        public static List<T> ReadResourceCsv<T>(string resourceName)
        {
            var resource = Application.GetResourceStream(new Uri(resourceName, UriKind.Relative));
            using var reader = new StreamReader(resource.Stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            return csv.GetRecords<T>().ToList();
        }
    }
}
