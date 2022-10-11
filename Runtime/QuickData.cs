using System;

namespace Quartzified.QuickKeep.Data
{
    [Serializable]
    public class QuickData
    {
        public string key;
        public string value;

        public QuickData(string _key, string _value)
        {
            key = _key;
            value = _value;
        }
    }
}