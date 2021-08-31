using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items.Protections
{
    public class Necklace : ProtectionItem, INecklace
    {
        public Necklace(IItemType type, Location location) : base(type, location)
        {
            Charges = Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Charges);
            Duration = Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.Duration);
        }

        public Necklace(IItemType type, Location location, byte charges, ushort duration) : base(type, location)
        {
            Charges = charges;
            Duration = duration;
        }

        public ImmutableHashSet<VocationType> AllowedVocations => null;

        public Dictionary<SkillType, byte> SkillBonus => Metadata.Attributes.SkillBonus;

        public byte Charges { get; private set; }
        public byte Defense => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Armor);
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

        protected override void Protect(ref CombatDamage damage)
        {
            if (Expired) return;
            DecreaseCharges();
            base.Protect(ref damage);
        }
    }
}