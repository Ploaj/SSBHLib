using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CrossModGui.Tools
{
    public static class ObservableCollectionExtensions
    {
        public static ObservableCollection<T> AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }

            // Return the collection to support chaining.
            return collection;
        }
    }
}
