using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Weapons;

public interface IDistanceWeapon : IWeapon
{
    sbyte ExtraHitChance => Metadata.Attributes.GetAttribute<sbyte>(ItemAttribute.HitChance);
    byte Range => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Range);

}