using System;
using System.Collections.Generic;
using System.Reflection;
using NeoServer.Game.Common.Contracts.DataStores;

namespace NeoServer.Data.InMemory.DataStores
{
    public class AreaTypeStore: IAreaTypeStore
    {
        private static readonly Dictionary<string, FieldInfo> areas = new(StringComparer.InvariantCultureIgnoreCase);

        public void Add(string name, FieldInfo area)
        {
            areas.TryAdd(name, area);
        }

        public byte[,] Get(string name)
        {
            return areas.TryGetValue(name, out var area) ? (byte[,]) area.GetValue(null) : default;
        }
    }
}