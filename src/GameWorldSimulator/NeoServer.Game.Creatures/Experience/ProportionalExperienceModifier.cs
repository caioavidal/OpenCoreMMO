using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Creatures.Monster.Summon;

namespace NeoServer.Game.Creatures.Experience;

/// <summary>
///     Modifies the base experience of the monster that the player receives based on the portion of the damage they dealt.
/// </summary>
public class ProportionalExperienceModifier : IBaseExperienceModifier
{
    public string Name => "Proportional Damage";

    public uint GetModifiedBaseExperience(IPlayer player, IMonster monster, uint baseExperience)
    {
        var totalDamage = GetTotalMonsterDamage(monster);
        var playerDamage = GetTotalPlayerDamage(monster, player);
        var percentDamageFactor = playerDamage / (double)totalDamage;
        var experience = (uint)(baseExperience * percentDamageFactor);
        return experience;
    }

    public bool IsEnabled(IPlayer player, IMonster monster)
    {
        return true;
    }

    private int GetTotalMonsterDamage(IMonster monster)
    {
        var damage = 0;
        foreach (var kvp in monster.Damages) damage += kvp.Value;
        return damage;
    }

    private int GetTotalPlayerDamage(IMonster monster, IPlayer player)
    {
        var damage = 0;
        foreach (var kvp in monster.Damages)
        {
            if (kvp.Key != player && (kvp.Key as Summon)?.Master != player) continue;
            damage += kvp.Value;
        }

        return damage;
    }
}