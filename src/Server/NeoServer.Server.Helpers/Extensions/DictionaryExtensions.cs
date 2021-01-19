using System;
using System.Collections.Generic;

namespace NeoServer.Server.Helpers.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool TryGetValue<T>(this Dictionary<string, object> dictionary, string key, out T value)
        {
           
            if (dictionary is null || key is null || !dictionary.TryGetValue(key, out var val))
            {
                value = default;
                return false;
            }

            try
            {
                value = (T)Convert.ChangeType(val, typeof(T));
                return true;
            }
            catch
            {
                value = default;
            }

            return false;
        }
    }
}
