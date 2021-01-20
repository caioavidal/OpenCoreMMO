using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoServer.Game.DataStore
{
    public class DataStore<TKey, TValue>
    {
        public DataStore()
        {
        }
        private static DataStore<TKey, TValue> data;
        public static DataStore<TKey, TValue> Data { get
            {
                data =  data ?? new DataStore<TKey, TValue>();
                return data;
            }
        }

        private Dictionary<TKey, TValue> values = new Dictionary<TKey, TValue>();

        public virtual void Add(TKey key, TValue value) => values.TryAdd(key, value);

        public virtual TValue Get(TKey key) => values.TryGetValue(key, out var value) ? value : default;
        public virtual IEnumerable<TValue> All => values.Values;

    }
}
