using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Factories.AttributeFactory;
using NeoServer.Game.Items.Items.Weapons;

namespace NeoServer.Game.Items.Factories
{
    public class WeaponFactory : IFactory
    {
        public event CreateItem OnItemCreated;
        private readonly DecayableFactory _decayableFactory;
        private readonly ProtectionFactory _protectionFactory;
        private readonly SkillBonusFactory _skillBonusFactory;
        private readonly TransformableFactory _transformableFactory;
        private readonly ChargeableFactory _chargeableFactory;

        public WeaponFactory(DecayableFactory decayableFactory, ProtectionFactory protectionFactory, SkillBonusFactory skillBonusFactory, TransformableFactory transformableFactory, ChargeableFactory chargeableFactory)
        {
            _decayableFactory = decayableFactory;
            _protectionFactory = protectionFactory;
            _skillBonusFactory = skillBonusFactory;
            _transformableFactory = transformableFactory;
            _chargeableFactory = chargeableFactory;
        }

        public IItem Create(IItemType itemType, Location location, IDictionary<ItemAttribute, IConvertible> attributes)
        {
            var decayable = _decayableFactory.Create(itemType);
            var skillBonuses = _skillBonusFactory.Create(itemType);
            var protection = _protectionFactory.Create(itemType);
            var transformable = _transformableFactory.Create(itemType);
            var chargeable = _chargeableFactory.Create(itemType);

            if (MeleeWeapon.IsApplicable(itemType)) return new MeleeWeapon(itemType, location)
            {
                Decayable = decayable,
                SkillBonus = skillBonuses,
                Protection = protection,
                Transformable = transformable,
                Chargeable = chargeable
            };
            if (DistanceWeapon.IsApplicable(itemType)) return new DistanceWeapon(itemType, location)
            {
                Decayable = decayable,
                SkillBonus = skillBonuses,
                Protection = protection,
                Transformable = transformable,
                Chargeable = chargeable
            };
            if (MagicWeapon.IsApplicable(itemType)) return new MagicWeapon(itemType, location)
            {
                Decayable = decayable,
                SkillBonus = skillBonuses,
                Protection = protection,
                Transformable = transformable,
                Chargeable = chargeable
            };

            if (ICumulative.IsApplicable(itemType))
            {
                if (ThrowableDistanceWeapon.IsApplicable(itemType))
                    return new ThrowableDistanceWeapon(itemType, location, attributes);
                if (Ammo.IsApplicable(itemType)) return new Ammo(itemType, location, attributes);
            }

            return null;
        }
    }
}
