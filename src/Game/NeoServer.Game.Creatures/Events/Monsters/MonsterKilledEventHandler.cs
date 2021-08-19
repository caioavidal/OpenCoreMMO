using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Creatures.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Creatures.Events.Monsters
{
    public class MonsterKilledEventHandler : IGameEventHandler
    {
        public void Execute(ICreature creature, IThing by, ILoot loot)
        {
            if (creature is not IMonster monster) { return; }
            GiveExperience(monster);
        }

        private void GiveExperience(IMonster monster)
        {
            if (monster.IsSummon) { return; }
            GrantExperienceBasedOnDamageContribution(monster);
        }

        private void GrantExperienceBasedOnDamageContribution(IMonster monster)
        {
            var contributions = new Dictionary<ICreature, ushort>(monster.Damages);
            var totalDamage = contributions.Sum(x => x.Value);

            ConvertSummonDamageToMasterDamage(contributions);
            GrantExperienceForNonPlayersOrPlayersNotInAParty(contributions, monster, totalDamage);
            GrantExperienceForParties(contributions, monster, totalDamage);
        }

        /// <summary>
        /// Looks for summons in the list of damage contributions and adds their damage contributions to their master.
        /// </summary>
        /// <param name="contributions">List of damage contributions by creature dealt to the monster.</param>
        /// <remarks>
        /// I wasn't sure this was actually needed, but it made sense to me that summons might end up in the Damages list,
        /// in which case the damage should probably be attributed to the master of the summon.
        /// </remarks>
        private void ConvertSummonDamageToMasterDamage(IDictionary<ICreature, ushort> contributions)
        {
            var contributionsToAdd = new Dictionary<ICreature, ushort>();
            var contributionsToRemove = new List<ICreature>();

            foreach (var creature in contributions.Keys)
            {
                if (creature is not Summon) { continue; }

                var master = (creature as Summon).Master;
                if (contributions.TryGetValue(master, out var damage))
                {
                    contributions[master] += contributions[creature];
                }
                else
                {
                    contributionsToAdd.Add(master, contributions[creature]);
                }

                contributionsToRemove.Add(creature);
            }

            foreach (var contribution in contributionsToRemove)
            {
                contributions.Remove(contribution);
            }

            foreach (var contribution in contributionsToAdd)
            {
                contributions.Add(contribution);
            }
        }

        /// <summary>
        /// Takes all non-players or players who are not in a party that is eligable for experience sharing for this monster and grants experience based on contribution percent.
        /// </summary>
        /// <param name="contributions">List of damage contributions by creature dealt to the monster.</param>
        /// <param name="monster">The monster that was killed.</param>
        /// <param name="totalDamage">Total damage dealt to the monster (no need to calcualte it multiple times).</param>
        private void GrantExperienceForNonPlayersOrPlayersNotInAParty(IDictionary<ICreature, ushort> contributions, IMonster monster, int totalDamage)
        {
            var nonGroupedContributions = contributions.Where(x =>
            {
                return (x.Key is not IPlayer player)
                    || player.IsInParty == false
                    || player.Party.SharedExperienceService.CanPartyReceiveSharedExperience(monster) == false;
            });

            foreach (var enemyDamage in nonGroupedContributions)
            {
                if (enemyDamage.Key is not ICombatActor actor) { continue; }

                var experience = BasicExperienceCalculation(enemyDamage.Value, totalDamage, monster.Experience);
                actor.GainExperience((uint)experience);
            }
        }

        /// <summary>
        /// Takes all players who are in a party that is capable of sharing experience for the specified monster,
        /// First calculating the party's contribution to the monster,
        /// then applying the experience sharing bonus to that and distributing the experience to each member of the party.
        /// </summary>
        /// <param name="contributions">List of damage contributions by creature dealt to the monster.</param>
        /// <param name="monster">The monster that was killed.</param>
        /// <param name="totalDamage">Total damage dealt to the monster (no need to calcualte it multiple times).</param>
        private void GrantExperienceForParties(IDictionary<ICreature, ushort> contributions, IMonster monster, int totalDamage)
        {
            var playersInPartyCapableOfExpShare = contributions.Where(x =>
            {
                return (x.Key is IPlayer player)
                    && player.IsInParty
                    && player.Party.SharedExperienceService.CanPartyReceiveSharedExperience(monster);
            }).Select(x => x.Key as IPlayer);

            var playersGroupedByParty = playersInPartyCapableOfExpShare.GroupBy(x => x.Party);
            var partyContributions = playersGroupedByParty.ToDictionary(
                keySelector: p => p,                                    // Party
                elementSelector: p => p.Sum(x => contributions[x])      // Total Party Damage
            );

            foreach (var partyGrouping in partyContributions.Keys)
            {
                var party = partyGrouping.Key;
                var partyExperience = BasicExperienceCalculation((ushort)partyContributions[partyGrouping], totalDamage, monster.Experience);
                var partySharingBonusExperience = party.SharedExperienceService.GetTotalPartyBonusExperience(partyExperience);
                var totalPartyExperience = (uint)Math.Round(partyExperience + partySharingBonusExperience);
                foreach (var partyMember in party.Members)
                {
                    var partyMemberDamage = contributions[partyMember];
                    var totalPartyDamage = partyContributions[partyGrouping];
                    var experience = BasicExperienceCalculation(partyMemberDamage, totalPartyDamage, totalPartyExperience);
                    partyMember.GainExperience(experience);
                }
            }
        }

        /// <summary>
        /// Calculates experience from the damage dealt, total damage, and the base experience.
        /// </summary>
        /// <param name="damageToMonster">Damage dealt by the actor or party.</param>
        /// <param name="totalDamageMonsterReceived">Total damage dealt to the monster.</param>
        /// <param name="baseMonsterExperience">Experience provided by the monster or after party bonus is applied.</param>
        /// <returns>Amount of experience that should be provided to the actor or party.</returns>
        private uint BasicExperienceCalculation(ushort damageToMonster, int totalDamageMonsterReceived, uint baseMonsterExperience)
        {
            var percentDamageToMonster = damageToMonster * 100 / totalDamageMonsterReceived;
            var experience = percentDamageToMonster * baseMonsterExperience / 100;
            return Convert.ToUInt32(experience);
        }
    }
}