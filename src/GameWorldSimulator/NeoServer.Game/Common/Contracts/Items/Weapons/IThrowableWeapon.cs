using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Contracts.Items.Weapons.Attributes;

namespace NeoServer.Game.Common.Contracts.Items.Weapons;

public interface IThrowableWeapon : ICumulative, IWeapon, IHasAttack
{
    byte Range { get; }
    byte ExtraHitChance { get; }
    bool ShouldBreak();
}