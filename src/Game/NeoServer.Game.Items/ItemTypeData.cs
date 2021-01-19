using NeoServer.Game.Contracts.Items;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NeoServer.Server.Items
{
    public class ItemTypeData
    {
        /// <summary>
        /// InMemory data of all game's item types
        /// </summary>
        /// <value></value>
        public static ImmutableDictionary<ushort, IItemType> InMemory { get; private set; }

        /// <summary>
        /// Loads item types into memory
        /// This data is used in the ItemFactory to create Item instances
        /// </summary>
        /// <param name="items"></param>
        public static void Load(Dictionary<ushort, IItemType> items)
        {
            if (InMemory == null)
            {
                InMemory = items.ToImmutableDictionary();
            }
        }
    }
}
