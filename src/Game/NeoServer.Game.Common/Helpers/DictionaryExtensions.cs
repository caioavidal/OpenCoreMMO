using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Common.Helpers
{
    public static class DictionaryExtensions
    {

        public static bool AddOrUpdate<TKey, TValue>(this IDictionary<TKey,TValue> map, TKey key, TValue value)
        {
            if(map.TryGetValue(key, out var v))
            {
                map[key] = value;
                return true;
            }

            return map.TryAdd(key, value);
        }
    }
}
