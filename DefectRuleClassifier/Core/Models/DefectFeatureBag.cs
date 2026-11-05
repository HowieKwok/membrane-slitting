using System.Collections.Generic;

namespace DefectRuleClassifier.Core.Models
{
    public sealed class DefectFeatureBag
    {
        private readonly Dictionary<string, double> values = new Dictionary<string, double>(System.StringComparer.OrdinalIgnoreCase);

        public IReadOnlyDictionary<string, double> Values => values;

        public void Set(string key, double value)
        {
            values[key] = value;
        }

        public bool Contains(string key)
        {
            return values.ContainsKey(key);
        }

        public bool TryGet(string key, out double value)
        {
            return values.TryGetValue(key, out value);
        }

        public double Get(string key, double defaultValue = 0)
        {
            double value;
            return values.TryGetValue(key, out value) ? value : defaultValue;
        }

        public DefectFeatureBag Clone()
        {
            DefectFeatureBag copy = new DefectFeatureBag();
            foreach (KeyValuePair<string, double> item in values)
                copy.Set(item.Key, item.Value);
            return copy;
        }
    }
}
