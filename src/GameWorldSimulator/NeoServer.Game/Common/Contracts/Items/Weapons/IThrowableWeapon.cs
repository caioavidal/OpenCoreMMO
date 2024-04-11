using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Body;

namespace NeoServer.Game.Common.Contracts.Items.Weapons;

public interface IThrowableWeapon : ICumulative, IWeapon, IHasAttack
{
    byte Range { get; }
    byte ExtraHitChance { get; }
    bool ShouldBreak();
}