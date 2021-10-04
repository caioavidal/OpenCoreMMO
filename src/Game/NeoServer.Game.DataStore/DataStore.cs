using System;
using System.Collections.Generic;

namespace NeoServer.Game.DataStore
{
    public class DataStore<TStore, TKey, TValue> where TStore : DataStore<TStore, TKey, TValue>
    {
        private static DataStore<TStore, TKey, TValue> data;

        private readonly Dictionary<TKey, TValue> values = new();

        public static DataStore<TStore, TKey, TValue> Data
        {
            get
            {
                data ??= new DataStore<TStore, TKey, TValue>();
                return data;
            }
        }

        public virtual IEnumerable<TValue> All => values.Values;
        public virtual IDictionary<TKey, TValue> Map => values;

        public virtual void Add(TKey key, TValue value)
        {
            values.TryAdd(key, value);
        }
        public virtual TValue Get(TKey key)
        {
            return values.TryGetValue(key, out var value) ? value : default;
        }

        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            return values.TryGetValue(key, out value);
        }

        public virtual bool Contains(TKey key)
        {
            return values.ContainsKey(key);
        }
    }
}