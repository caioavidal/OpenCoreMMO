using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace NeoServer.Game.Items.Items
{
    public class Ring : MoveableItem, IRing
    {
        public Ring(IItemType type, Location location) : base(type, location)
        {
            Charges = Metadata.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Charges);
            Duration = Metadata.Attributes.GetAttribute<ushort>(Enums.ItemAttribute.Duration);
        }

        public byte Charges { get; private set; }

        public Dictionary<DamageType, byte> DamageProtection => Metadata.Attributes.DamageProtection;


        public bool Expired => Duration <= 0 && Charges <= 0; //todo: duplicated from necklace
        public ushort Duration { get; private set; }
        public byte Defense => Metadata.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Armor);

        public ImmutableHashSet<VocationType> AllowedVocations => new HashSet<VocationType>().ToImmutableHashSet();

        public void DecreaseCharges()
        {
            if (!Expired) Charges--;
        }

        public void StartDecaying()
        {
            throw new NotImplementedException();
        }

        public static bool IsApplicable(IItemType type) => type.BodyPosition == Enums.Players.Slot.Ring;

      
    }
}
