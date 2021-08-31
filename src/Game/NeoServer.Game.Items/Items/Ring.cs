using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Items.Protections;

namespace NeoServer.Game.Items.Items
{
    public class Ring : Accessory, IRing
    {
        private bool inUse;

        public Ring(IItemType type, Location location) : base(type, location)
        {
        }

        public ImmutableHashSet<VocationType> AllowedVocations => new HashSet<VocationType>().ToImmutableHashSet();

    
        public byte Defense => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Armor);

        public Span<byte> GetRaw()
        {
            Span<byte> cache = stackalloc byte[2];

            var clientId = 0;

            if (inUse) clientId = Metadata.ClientId;
            var idBytes = BitConverter.GetBytes(inUse ? Metadata.TransformTo : Metadata.ClientId);

            cache[0] = idBytes[0];
            cache[1] = idBytes[1];

            return cache.ToArray();
        }

        public static bool IsApplicable(IItemType type)
        {
            return type.BodyPosition == Slot.Ring;
        }

        public byte Charges { get; }
        public void DecreaseCharges()
        {
            throw new NotImplementedException();
        }

        public int DecaysTo { get; }
        public int Duration { get; }
        public bool Expired { get; }
        public bool StartedToDecay { get; }
        public long StartedToDecayTime { get; }
        public bool ShouldDisappear { get; }
        public bool Decay()
        {
            throw new NotImplementedException();
        }

        public void DressedIn(IPlayer player)
        {
            throw new NotImplementedException();
        }

        public void UndressFrom(IPlayer player)
        {
            throw new NotImplementedException();
        }
    }
}