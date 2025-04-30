using System.Collections.Generic;
using System.Linq;

namespace GameLab.Core
{
    /// <summary>
    /// will be true if eny key in the inner dictionary is true
    /// </summary>
    public class MultiLock
    {
        private readonly Dictionary<string, bool> _locks = new();

        public bool this[string key]
        {
            set => _locks[key] = value;
            get => _locks.ContainsKey(key) && _locks[key];
        }

        public static implicit operator bool(MultiLock multiLock)
        {
            return multiLock._locks.Count > 0 && multiLock._locks.Any(x => x.Value);
        }
    }
}