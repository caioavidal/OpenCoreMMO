using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Services;
using System;
using System.Linq;

namespace NeoServer.Game.Creatures.Services
{
    /// <summary>
    /// A service for determining party eligability and amount of shared experience.
    /// </summary>
    /// <remarks>
    /// Comments reflect values matching tibia, but configuration allows for any specific value to be changed.
    /// </remarks>
    public class PartyShareExperienceService : IPartyShareExperienceService
    {
        private readonly IPartyShareExperienceConfiguration Configuration;
        private readonly IParty Party;

        public PartyShareExperienceService(IParty party, IPartyShareExperienceConfiguration configuration)
        {
            Party = party ?? throw new ArgumentNullException(nameof(party));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Combines all of the experience sharing requirements into a single check.
        /// </summary>
        /// <param name="monster">The monster killed by the party.</param>
        public bool CanPartyReceiveSharedExperience(IMonster monster)
        {
            return IsExperienceSharingTurnedOn()
                && DoesMonsterQualify(monster)
                && ArePartyCloseEnoughToEachOther()
                && ArePartyLevelsInProperRange()
                && IsEveryMemberActive(monster);
        }

        /// <summary>
        /// Calculates the amount of experience the party will receive in total from killing the monster.
        /// This is the total experience, not individual player experience.
        /// </summary>
        /// <param name="monster">The monster killed by the party.</param>
        public double GetTotalPartyExperience(IMonster monster)
        {
            if (CanPartyReceiveSharedExperience(monster) == false) { return 0; }

            var uniqueVocationCount = Party.Members.GroupBy(x => x.Vocation.Name).Count();
            var bonusExperienceFactor = GetUniqueVocationCountBonusFactor(uniqueVocationCount);
            return monster.Experience * bonusExperienceFactor;
        }

        /// <summary>
        /// Checks if the party has experience sharing turned on.
        /// </summary>
        public bool IsExperienceSharingTurnedOn()
        {
            return Configuration.IsSharedExperienceAlwaysOn || Party.ExperienceSharingEnabled;
        }

        /// <summary>
        /// If shared experience is activated, an experience bonus of 20% is granted 
        /// if the defeated creature usually yields at least 20 experience points.
        /// </summary>
        /// <param name="monster">The creature defeated by the party.</param>
        public bool DoesMonsterQualify(IMonster monster)
        {
            return monster.Experience >= Configuration.MinimumMonsterExperienceToBeShared;
        }

        /// <summary>
        /// Only characters that are of similar levels can share experience when hunting together.
        /// To be more precise, the lowest character in a party may not have less than two-thirds of the levels of the highest character.
        /// This means a level 40 can share experience with a level 60 but not with a level 20; or a level 200 can share experience with a level 300.
        /// </summary>
        public bool ArePartyLevelsInProperRange()
        {
            if (Configuration.RequirePartyMemberLevelProximity == false) { return true; }

            var lowestLevel = int.MaxValue;
            var highestLevel = 0;

            foreach (var member in Party.Members)
            {
                lowestLevel = Math.Min(lowestLevel, member.Level);
                highestLevel = Math.Max(highestLevel, member.Level);
            }

            return highestLevel <= lowestLevel * Configuration.LowestLevelSupportedMultipler;
        }

        /// <summary>
        /// The distance of all party members to the leader must be smaller or equal to 30 fields, 
        /// this works also if you are one floor up or down.
        /// </summary>
        public bool ArePartyCloseEnoughToEachOther()
        {
            if (Configuration.RequirePartyProximity == false) { return true; }

            var members = Party.Members;

            foreach (var memberA in members)
            {
                foreach (var memberB in members)
                {
                    if (memberA == memberB) { continue; } // No need to check distance between the same character.

                    var horizontalDistance = memberA.Location.GetSqmDistance(memberB.Location);
                    if (horizontalDistance > Configuration.MaximumPartyDistanceToReceiveExperienceSharing) { return false; }

                    var verticalDistance = memberA.Location.GetOffSetZ(memberB.Location);
                    if (verticalDistance > Configuration.MaximumPartyVerticalDistanceToReceiveExperienceSharing) { return false; }
                }
            }

            return true;
        }

        /// <summary>
        /// Finally, all party members must be actively involved. 
        /// This means they must either have healed another member or attacked an aggressive monster.
        /// </summary>
        public bool IsEveryMemberActive(IMonster monster)
        {
            if (Configuration.RequirePartyMemberParticipation == false) { return true; }

            foreach (var member in Party.Members)
            {
                // TODO: Figure out how to track heals.

                monster.Damages.TryGetValue(member, out var damageDealt);
                if (damageDealt <= 0) { return false; }
            }

            return true;
        }

        /// <summary>
        /// If party members of 2 vocations are involved in the fight, the bonus is raised to 30%;
        /// 3 different vocations yield a bonus of 60%;
        /// and if party members of all 4 vocations participate, the bonus will even be raised to 100%. 
        /// </summary>
        /// <param name="uniqueVocationCount">The number of unique vocations in the party.</param>
        public double GetUniqueVocationCountBonusFactor(int uniqueVocationCount)
        {
            var maxVocations = Configuration.UniqueVocationBonusExperienceFactor.Count;
            uniqueVocationCount = Math.Min(maxVocations, Math.Max(1, uniqueVocationCount)); // Force vocation count to be between 1 and 4*.
            return Configuration.UniqueVocationBonusExperienceFactor.TryGetValue(uniqueVocationCount, out var bonusFactor)
                 ? bonusFactor
                 : 0;
        }
    }
}