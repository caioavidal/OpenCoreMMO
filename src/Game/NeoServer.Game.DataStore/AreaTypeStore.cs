using System;
using System.Collections.Generic;
using System.Reflection;

namespace NeoServer.Game.DataStore
{
    public class AreaTypeStore
    {
        private static readonly Dictionary<string, FieldInfo> areas = new(StringComparer.InvariantCultureIgnoreCase);

        public static void Add(string name, FieldInfo area)
        {
            areas.TryAdd(name, area);
        }

        public static byte[,] Get(string name)
        {
            return areas.TryGetValue(name, out var area) ? (byte[,]) area.GetValue(null) : default;
        }
    }
}