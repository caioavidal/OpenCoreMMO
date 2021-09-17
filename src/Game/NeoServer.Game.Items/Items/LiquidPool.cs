using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items
{
    public struct LiquidPool : ILiquid
    {
        public bool IsLiquidPool => Metadata.Group == ItemGroup.Splash;
        public bool IsLiquidSource => Metadata.Flags.Contains(ItemFlag.LiquidSource);
        public bool IsLiquidContainer => Metadata.Group == ItemGroup.ItemGroupFluid;
        public LiquidColor LiquidColor { get; }
        public Location Location { get; set; }

        public IItemType Metadata { get; private set; }
        public string CustomLookText => "You see a liquid pool"; //todo: revise
        public ushort ClientId => Metadata.ClientId;

        public LiquidPool(IItemType type, Location location,
            IDictionary<ItemAttribute, IConvertible> attributes) : this()
        {
            Metadata = type;
            Location = location;
            LiquidColor = LiquidColor.Empty;
            StartedToDecayTime = default;
            Elapsed = 0;
            LiquidColor = GetLiquidColor(attributes);
        }

        public LiquidPool(IItemType type, Location location, LiquidColor color) : this()
        {
            Metadata = type;
            Location = location;
            LiquidColor = LiquidColor.Empty;
            StartedToDecayTime = DateTime.Now.Ticks;
            Elapsed = 0;

            LiquidColor = GetLiquidColor(color);
        }

        public LiquidColor GetLiquidColor(LiquidColor color)
        {
            if (!IsLiquidPool && !IsLiquidContainer) return 0x00;

            return color;
        }

        public LiquidColor GetLiquidColor(IDictionary<ItemAttribute, IConvertible> attributes)
        {
            if (!IsLiquidPool && !IsLiquidContainer) return 0x00;
            if (attributes != null && attributes.TryGetValue(ItemAttribute.Count, out var count))
                return new LiquidTypeMap()[(byte) count];
            return LiquidColor.Empty;
        }

        public Span<byte> GetRaw()
        {
            Span<byte> cache = stackalloc byte[3];
            var idBytes = BitConverter.GetBytes(ClientId);

            cache[0] = idBytes[0];
            cache[1] = idBytes[1];
            cache[2] = (byte) LiquidColor;

            return cache.ToArray();
        }

        public static bool IsApplicable(IItemType type)
        {
            return type.Group == ItemGroup.Splash ||
                   type.Flags.Contains(ItemFlag.LiquidSource) ||
                   type.Group == ItemGroup.ItemGroupFluid;
        }

        public Func<IItemType> DecaysTo { get; init; }
        public uint Duration => Metadata.Attributes.GetAttribute<uint>(ItemAttribute.Duration);
        public long StartedToDecayTime { get; private set; }
        public bool StartedToDecay => StartedToDecayTime != default;
        public bool Expired => StartedToDecayTime + TimeSpan.TicksPerSecond * Duration < DateTime.Now.Ticks;
        public uint Elapsed { get; }
        public uint Remaining => Duration - Elapsed;
        public bool ShouldDisappear => DecaysTo?.Invoke() == null;

        public bool TryDecay()
        {
            if (ShouldDisappear) return false;
            //if (!ItemTypeStore.Data.TryGetValue((ushort) DecaysTo, out var newItem)) return false;

            Metadata = DecaysTo?.Invoke();
            StartedToDecayTime = DateTime.Now.Ticks;
            return true;
        }

        public event DecayDelegate OnDecayed;

        public event PauseDecay OnPaused;
        public event StartDecay OnStarted;

        public void StartDecay()
        {
        }

        public void PauseDecay()
        {
        }

        public void SetDuration(ushort duration)
        {
            throw new NotImplementedException();
        }
    }
}