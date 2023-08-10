using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Common.Services;
using NeoServer.Game.Items.Items.Weapons;
using NeoServer.Game.Systems.Combat.Attacks.Spell;

namespace NeoServer.Extensions.Spells.Attack.Knight;

public class WhirlwindThrow : AttackSpell
{
    public override DamageType DamageType => DamageType.MagicalPhysical;
    public override byte Range => 5;
    public override bool NeedsTarget => true;

    public override Result CanCast(ICombatActor actor)
    {
        if (actor is not IPlayer player) return Result.NotApplicable;

        var canCastResult = base.CanCast(actor);

        if (canCastResult.Failed) return canCastResult;

        var weapon = player.Inventory.Weapon;

        if (weapon is MeleeWeapon) return Result.Success;

        OperationFailService.Send(player, "You need to equip a weapon to use this spell.");
        return Result.NotPossible;
    }

    public override CombatAttackParams PrepareAttack(ICombatActor actor)
    {
        if (actor is not IPlayer player) return null;

        var weapon = player.Inventory.Weapon;

        var shootType = weapon?.Type switch
        {
            WeaponType.Axe => ShootType.WhirlwindAxe,
            WeaponType.Club => ShootType.WhirlwindClub,
            WeaponType.Sword => ShootType.WhirlwindSword,
            _ => ShootType.None
        };

        if (shootType is ShootType.None) return null;

        var attack = player.Inventory.TotalAttack;

        var min = player.MinimumAttackPower + player.Skills[player.SkillInUse].Level * attack * 0.01 + 1;
        var max = player.MinimumAttackPower + player.Skills[player.SkillInUse].Level * attack * 0.03 + 6;

        var damage = GameRandom.Random.NextInRange(min, max);

        return new CombatAttackParams
        {
            DamageType = DamageType,
            ShootType = shootType,
            Damages = new[]
            {
                new CombatDamage((ushort)damage, DamageType, EffectT.XGray),
            }
        };
    }

    public override ISpellCombatAttack CombatAttack => SpellCombatAttack.Instance;
}