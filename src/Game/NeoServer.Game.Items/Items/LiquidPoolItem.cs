using NeoServer.Game.Common;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Items;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Items.Items
{
    public struct LiquidPoolItem : ILiquid
    {
        public bool IsLiquidPool => Metadata.Group == ItemGroup.Splash;
        public bool IsLiquidSource => Metadata.Flags.Contains(ItemFlag.LiquidSource);
        public bool IsLiquidContainer => Metadata.Group == ItemGroup.ITEM_GROUP_FLUID;
        public LiquidColor LiquidColor { get; }
        public Location Location { get; set; }

        public IItemType Metadata { get; private set; }
        public ushort ClientId => Metadata.ClientId;

        public LiquidPoolItem(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes)
        {
            Metadata = type;
            Location = location;
            LiquidColor = LiquidColor.Empty;
            StartedToDecayTime = default;
            LiquidColor = GetLiquidColor(attributes);
        }
        public LiquidPoolItem(IItemType type, Location location, LiquidColor color)
        {
            Metadata = type;
            Location = location;
            LiquidColor = LiquidColor.Empty;
            StartedToDecayTime = DateTime.Now.Ticks;
            LiquidColor = GetLiquidColor(color);
        }
        public LiquidColor GetLiquidColor(LiquidColor color)
        {
            if (!IsLiquidPool && !IsLiquidContainer) return 0x00;
            
            return color;
        }
        public LiquidColor GetLiquidColor(IDictionary<ItemAttribute, IConvertible> attributes)
        {
            if (!IsLiquidPool && !IsLiquidContainer)
            {
                return 0x00;
            }
            if (attributes != null && attributes.TryGetValue(ItemAttribute.Count, out var count))
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

        public int DecaysTo => Metadata.Attributes.GetAttribute<int>(ItemAttribute.ExpireTarget);
        public int Duration => Metadata.Attributes.GetAttribute<int>(ItemAttribute.Duration);
        public long StartedToDecayTime { get; private set; }
        public bool StartedToDecay => StartedToDecayTime != default;
        public bool Expired => StartedToDecayTime + TimeSpan.TicksPerSecond * Duration < DateTime.Now.Ticks;
        public bool ShouldDisappear => DecaysTo == 0;
        public bool Decay()
        {
            if (DecaysTo <= 0) return false;
            if (!ItemTypeData.InMemory.TryGetValue((ushort)DecaysTo, out var newItem)) return false;

            Metadata = newItem;
            StartedToDecayTime = DateTime.Now.Ticks;
            return true;
        }
    }
}