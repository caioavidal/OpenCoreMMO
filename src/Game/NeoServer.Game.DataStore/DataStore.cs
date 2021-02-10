using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace NeoServer.Game.DataStore
{
    public class DataStore<TStore, TKey, TValue> where TStore : DataStore<TStore, TKey, TValue>
    {
        public DataStore()
        {
        }
        private static DataStore<TStore, TKey, TValue> data;
        public static DataStore<TStore, TKey, TValue> Data
        {
            get
            {
                data = data ?? new DataStore<TStore, TKey, TValue>();
                return data;
            }
        }

        private Dictionary<TKey, TValue> values = new Dictionary<TKey, TValue>();

        public virtual void Add(TKey key, TValue value) => values.TryAdd(key, value);

        public virtual TValue Get(TKey key) => values.TryGetValue(key, out var value) ? value : default;
        public virtual bool TryGetValue(TKey key, out TValue value) => values.TryGetValue(key, out value);
        public virtual IEnumerable<TValue> All => values.Values;
        public virtual IDictionary<TKey, TValue> Map => values;

        public virtual bool Contains(TKey key) => values.ContainsKey(key);

    }
}
