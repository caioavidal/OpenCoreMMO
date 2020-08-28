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
    public class Necklace : MoveableItem, INecklaceItem
    {
        public Necklace(IItemType type, Location location) : base(type, location)
        {
            Charges = Metadata.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Charges);
            Duration = Metadata.Attributes.GetAttribute<ushort>(Enums.ItemAttribute.Duration);
        }

        public byte Charges { get; private set; }
        public byte Defense => Metadata.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Armor);
        public Dictionary<DamageType, byte> DamageProtection => Metadata.Attributes.DamageProtection;

        public bool Expired => Duration <= 0 && Charges <= 0;

        public ushort Duration { get; private set; }

        public ImmutableHashSet<VocationType> AllowedVocations => null;

        public void DecreaseCharges() => Charges--;

        public static bool IsApplicable(IItemType type) => type.BodyPosition == Slot.Necklace;

        public void StartDecaying()
        {
            throw new NotImplementedException();
        }
    }
}
