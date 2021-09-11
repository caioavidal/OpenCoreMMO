using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Items.Weapons;

namespace NeoServer.Game.Items.Factories
{
    public class WeaponFactory : IFactory
    {
        public event CreateItem OnItemCreated;
        

        public IItem Create(IItemType itemType, Location location, IDictionary<ItemAttribute, IConvertible> attributes)
        {
            if (MeleeWeapon.IsApplicable(itemType)) return new MeleeWeapon(itemType, location);
            if (DistanceWeapon.IsApplicable(itemType)) return new DistanceWeapon(itemType, location);
            if (MagicWeapon.IsApplicable(itemType)) return new MagicWeapon(itemType, location);

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
