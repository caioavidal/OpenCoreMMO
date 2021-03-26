using System.Collections.Generic;

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
