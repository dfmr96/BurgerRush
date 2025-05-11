using System.Collections.Generic;

namespace Save
{
    public class PlayerStatsSaveData
    {
        public List<StatEntry> stats = new();

        [System.Serializable]
        public class StatEntry
        {
            public string key;
            public string value;
        }
    }
}