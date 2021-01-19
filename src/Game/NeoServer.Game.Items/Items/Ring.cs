using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Body;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NeoServer.Game.Items.Items
{
    public class Ring : Decayable, IRing
    {
        public Ring(IItemType type, Location location) : base(type, location)
        {
        }

        public Dictionary<DamageType, byte> DamageProtection => Metadata.Attributes.DamageProtection;

        private bool inUse;
        public byte Defense => Metadata.Attributes.GetAttribute<byte>(Common.ItemAttribute.Armor);
        public Span<byte> GetRaw()
        {
            Span<byte> cache = stackalloc byte[2];

            var clientId = 0;

            if (inUse)
            {
                clientId = Metadata.ClientId;
            }
            var idBytes = BitConverter.GetBytes(inUse ? Metadata.TransformTo : Metadata.ClientId);

            cache[0] = idBytes[0];
            cache[1] = idBytes[1];

            return cache.ToArray();
        }

        public ImmutableHashSet<VocationType> AllowedVocations => new HashSet<VocationType>().ToImmutableHashSet();

        public static bool IsApplicable(IItemType type) => type.BodyPosition == Common.Players.Slot.Ring;

    }
}
