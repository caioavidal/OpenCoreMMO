using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Body;

namespace NeoServer.Game.Items.Items
{
    public class Necklace : MoveableItem, INecklaceItem
    {
        public Necklace(IItemType type, Location location) : base(type, location)
        {
            Charges = Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Charges);
            Duration = Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.Duration);
        }

        public ImmutableHashSet<VocationType> AllowedVocations => null;

        public byte Charges { get; private set; }
        public byte Defense => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Armor);
        public Dictionary<DamageType, byte> DamageProtection => Metadata.Attributes.DamageProtection;

        public bool Expired => Duration <= 0 && Charges <= 0;

        public ushort Duration { get; }

        public void DecreaseCharges()
        {
            Charges--;
        }

        public void StartDecaying()
        {
            throw new NotImplementedException();
        }

        public static bool IsApplicable(IItemType type)
        {
            return type.BodyPosition == Slot.Necklace;
        }
    }
}