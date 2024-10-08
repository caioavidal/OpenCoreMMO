using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Application.Features.Combat.PlayerDefense;

public static class PlayerBlockAttack
{
    public static CombatDamage TryBlock(ICombatActor defender, CombatDamage attack)
    {
        if (defender is not IPlayer player) return attack;

        int damage = attack.Damage;
        
        attack = ImmunityDefenseMethod.Defend(player, attack);
        if (attack.Damage <= 0)
        {
            attack.SetNewDamage(0);
            player.UpdateBlockCounter(BlockType.Armor);
            return attack;
        }

        attack = EquipmentDefenseMethod.Defend(player, attack);
        if (attack.Damage <= 0)
        {
            attack.SetNewDamage(0);
            player.UpdateBlockCounter(BlockType.Armor);
            return attack;
        }

        if (player.CanBlock(attack))
        {
            damage = ShieldDefenseMethod.Defend(player, attack);
            
            if (damage <= 0)
            {
                attack.SetNewDamage(0);
                player.UpdateBlockCounter(BlockType.Shield);
                return attack;
            }
        }

        if (!attack.IsElementalDamage)
        {
            damage = ArmorDefenseMethod.Defend(player, attack.Damage);
            
            if (damage <= 0)
            {
                attack.SetNewDamage(0);
                player.UpdateBlockCounter(BlockType.Armor);
                return attack;
            }
        }

        return attack;
    }
}