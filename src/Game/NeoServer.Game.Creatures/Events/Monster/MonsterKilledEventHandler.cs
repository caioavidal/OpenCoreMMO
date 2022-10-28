using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Creatures.Experience;
using NeoServer.Game.Creatures.Party;

namespace NeoServer.Game.Creatures.Events.Monster;

public class MonsterKilledEventHandler : IGameEventHandler
{
    // TODO: Find a better way to declare these, like dependency injection or something.
    // I'm not super faimilar with how EventHandlers are created yet.

    // Each base experience modifier and experience bonus contains its own logic for when to apply and how much.

    private readonly IEnumerable<IBaseExperienceModifier> BaseExperienceModifiers = new List<IBaseExperienceModifier>
    {
        new ProportionalExperienceModifier()
    };

    private readonly IEnumerable<IExperienceBonus> ExperienceBonuses = new List<IExperienceBonus>
    {
        new SharedExperienceBonus(new SharedExperienceConfiguration())
    };

    public void Execute(ICreature creature, IThing by, ILoot loot)
    {
        if (creature is not IMonster monster || monster.IsSummon) return;

        var players = monster.Damages.Where(x => x.Key is IPlayer).Select(x => x.Key as IPlayer);
        foreach (var player in players)
        {
            // Apply all base experience modifiers (e.g. monster experience based on portion of damage dealt).
            var baseExperience = monster.Experience;
            foreach (var modifier in BaseExperienceModifiers)
                baseExperience = modifier.GetModifiedBaseExperience(player, monster, baseExperience);

            // Determine each grouping of bonuses. All bonuses of the same type are added together.
            var bonusesByGroup = ExperienceBonuses
                .Where(x => x.IsEnabled(player, monster))
                .GroupBy(x => x.BonusType)
                .ToDictionary(
                    x => x.Key,
                    x => CalculateTotalExperienceBonus(x, player, monster)
                );

            // Multiply the base experience by each bonus group value.
            // Ex. Monster Experience * Standard Bonuses * Stamina Bonuses * World Multiplier.
            var experience = baseExperience;
            foreach (var bonus in bonusesByGroup) experience = (uint)(experience * (1 + bonus.Value));

            // Now that it's been calculated. Grant the player experience.
            player.GainExperience(experience);
        }
    }

    private static double CalculateTotalExperienceBonus(IEnumerable<IExperienceBonus> bonuses, IPlayer player,
        IMonster monster)
    {
        var amount = 0.0;
        foreach (var bonus in bonuses) amount += bonus.GetBonusFactorAmount(player, monster);
        return amount;
    }
}