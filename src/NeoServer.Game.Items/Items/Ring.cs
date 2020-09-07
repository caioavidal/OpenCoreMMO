using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
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
        public byte Defense => Metadata.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Armor);
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

        public static bool IsApplicable(IItemType type) => type.BodyPosition == Enums.Players.Slot.Ring;

    }
}
