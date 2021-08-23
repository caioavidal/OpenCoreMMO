using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Creatures.Experience;
using NeoServer.Game.Creatures.Model.Players;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Creatures.Events.Monsters
{
    public class MonsterKilledEventHandler : IGameEventHandler
    {
        // TODO: Find a better way to declare these, like dependency injection or something.
        // I'm not super faimilar with how EventHandlers are created yet.

        // Each base experience modifier and experience bonus contains its own logic for when to apply and how much.

        private IEnumerable<IBaseExperienceModifier> BaseExperienceModifiers = new List<IBaseExperienceModifier>()
        {
            new ProportionalExperienceModifier()
        };
        private IEnumerable<IExperienceBonus> ExperienceBonuses = new List<IExperienceBonus>()
        {
            new SharedExperienceBonus(new SharedExperienceConfiguration())
        };

        public void Execute(ICreature creature, IThing by, ILoot loot)
        {
            if (creature is not IMonster monster || monster.IsSummon) { return; }

            var players = monster.Damages.Where(x => x.Key is IPlayer).Select(x => x.Key as IPlayer);
            foreach (var player in players)
            {
                // Apply all base experience modifiers (e.g. monster experience based on portion of damage dealt).
                var baseExperience = monster.Experience;
                foreach (var modifier in BaseExperienceModifiers)
                {
                    baseExperience = modifier.GetModifiedBaseExperience(player, monster, baseExperience);
                }

                // Determine each grouping of bonuses. All bonuses of the same type are added together.
                var applicableBonuses = ExperienceBonuses.Where(x => x.IsEnabled(player, monster));
                var bonusesByGroup = applicableBonuses.GroupBy(x => x.BonusType)
                    .ToDictionary(
                        keySelector: g => g.Key,
                        elementSelector: g => g.Sum(x => x.GetBonusFactorAmount(player, monster))
                    );

                // Multiply the base experience by each bonus group value.
                // Ex. Monster Experience * Standard Bonuses * Stamina Bonuses * World Multiplier.
                var experience = baseExperience;
                foreach (var bonus in bonusesByGroup)
                {
                    experience = (uint)(experience * (1 + bonus.Value));
                }

                // Now that it's been calculated. Grant the player experience.
                player.GainExperience(experience);
            }
        }
    }
}