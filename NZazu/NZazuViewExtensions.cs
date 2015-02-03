using System;
using System.Collections.Generic;
using System.Linq;

namespace NZazu
{
    public static class NZazuViewExtensions
    {
        public static IDictionary<string, string> GetFieldValues(this INZazuView view)
        {
            if (view == null) throw new ArgumentNullException("view");
            return view.FormDefinition.Fields.ToDictionary(f => f.Key, f => view.GetField(f.Key).Value);
        }

        public static void SetFieldValues(this INZazuView view, IEnumerable<KeyValuePair<string, string>> fieldValues)
        {
            if (view == null) throw new ArgumentNullException("view");
            if (fieldValues == null) throw new ArgumentNullException("fieldValues");
            foreach (var kvp in fieldValues)
                view.GetField(kvp.Key).Value = kvp.Value;
        }

    }
}