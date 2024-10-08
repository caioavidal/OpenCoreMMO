using NeoServer.Game.Item.Items.Weapons;

namespace NeoServer.Game.Common.Contracts.Items;

public interface IHasElementalDamage
{
    ElementalDamage ElementalDamage { get; }
}