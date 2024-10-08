using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Parsers;

namespace NeoServer.Game.Creature.Monster.Actions;

//TODO: to be deleted
internal static class MonsterDefend
{
    public static int DefendUsingShield(Monster monster, int attack)
    {
        attack -= (ushort)GameRandom.Random.NextInRange(monster.Defense / 2f, monster.Defense);
        return attack;
    }

    public static int DefendUsingArmor(Monster monster, int attack)
    {
        switch (monster.ArmorRating)
        {
            case > 3:
                attack -= (ushort)GameRandom.Random.NextInRange(monster.ArmorRating / 2f,
                    monster.ArmorRating - (monster.ArmorRating % 2 + 1));
                break;
            case > 0:
                --attack;
                break;
        }

        return attack;
    }

    public static CombatDamage ImmunityDefend(Monster monster, CombatDamage damage)
    {
        if (damage.Damage <= 0) return damage;

        if (monster.HasImmunity(damage.Type.ToImmunity()))
        {
            damage.SetNewDamage(0);
            return damage;
        }

        if (monster.Metadata.ElementResistance is null) return damage;

        if (!monster.Metadata.ElementResistance.TryGetValue(damage.Type, out var resistance)) return damage;
        
        damage.ReduceDamageByPercent(resistance);

        return damage;
    }
}