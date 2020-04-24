using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Items.Items
{
    public readonly struct LiquidPoolItem : ILiquidItem
    {
        public bool IsLiquidPool => Metadata.Group == ItemGroup.Splash;

        public bool IsLiquidSource => Metadata.Flags.Contains(ItemFlag.LiquidSource);

        public bool IsLiquidContainer => Metadata.Group == ItemGroup.ITEM_GROUP_FLUID;

        public LiquidColor LiquidColor { get; }

        public Location Location { get; }

        public IItemType Metadata { get; }
        public ushort ClientId { get; }


        public LiquidPoolItem(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes)
        {
            Metadata = type;
            Location = location;
            ClientId = type.ClientId;
            LiquidColor = LiquidColor.Empty;
            LiquidColor = GetLiquidColor(attributes);
        }

        public LiquidColor GetLiquidColor(IDictionary<ItemAttribute, IConvertible> attributes)
        {
            if (!IsLiquidPool && !IsLiquidContainer)
            {
                return 0x00;
            }
            if (attributes.TryGetValue(ItemAttribute.Count, out var count))
            {
                return new LiquidTypeMap()[(byte)count];
            }
            return LiquidColor.Empty;
        }

        public Span<byte> GetRaw()
        {
            Span<byte> cache = stackalloc byte[3];
            var idBytes = BitConverter.GetBytes(ClientId);

            cache[0] = idBytes[0];
            cache[1] = idBytes[1];
            cache[2] = (byte)LiquidColor;

            return cache.ToArray();
        }
        public static bool IsApplicable(IItemType type) => type.Group == ItemGroup.Splash ||
                                                           type.Flags.Contains(ItemFlag.LiquidSource) ||
                                                           type.Group == ItemGroup.ITEM_GROUP_FLUID;

    }
}
