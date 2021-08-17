using NeoServer.Game.Common.Contracts.Services;
using System.Collections.Generic;

namespace NeoServer.Game.Creatures.Model.Players
{
    public class PartySharedExperienceConfiguration : IPartyShareExperienceConfiguration
    {
        public bool IsSharedExperienceAlwaysOn { get; }
        public bool RequirePartyProximity { get; }
        public int MaximumPartyDistanceToReceiveExperienceSharing { get; }
        public int MaximumPartyVerticalDistanceToReceiveExperienceSharing { get; }
        public bool RequirePartyMemberLevelProximity { get; }
        public bool RequirePartyMemberParticipation { get; }
        public uint MinimumMonsterExperienceToBeShared { get; }
        public double LowestLevelSupportedMultipler { get; }
        public IDictionary<int, double> UniqueVocationBonusExperienceFactor { get; }

        public PartySharedExperienceConfiguration(
            bool? isSharedExperienceAlwaysOn,
            bool? requirePartyProximity,
            int? maximumPartyDistanceToReceiveExperienceSharing,
            int? maximumPartyVerticalDistanceToReceiveExperienceSharing,
            bool? requirePartyMemberLevelProximity,
            bool? requirePartyMemberParticipation,
            uint? minimumMonsterExperienceToBeShared,
            IDictionary<int, double> uniqueVocationBonusExperienceFactor,
            double? lowestLevelSupportedMultipler
        )
        {
            IsSharedExperienceAlwaysOn = isSharedExperienceAlwaysOn ?? false;
            RequirePartyProximity = requirePartyProximity ?? true;
            MaximumPartyDistanceToReceiveExperienceSharing = maximumPartyDistanceToReceiveExperienceSharing ?? 30;
            MaximumPartyVerticalDistanceToReceiveExperienceSharing = maximumPartyVerticalDistanceToReceiveExperienceSharing ?? 1;
            RequirePartyMemberLevelProximity = requirePartyMemberLevelProximity ?? true;
            RequirePartyMemberParticipation = requirePartyMemberParticipation ?? true;
            MinimumMonsterExperienceToBeShared = minimumMonsterExperienceToBeShared ?? 20;
            LowestLevelSupportedMultipler = lowestLevelSupportedMultipler ?? (3/2);
            UniqueVocationBonusExperienceFactor = uniqueVocationBonusExperienceFactor ?? new Dictionary<int, double>()
            {
                { 1, 0.0 },
                { 2, 0.2 },
                { 3, 0.6 },
                { 4, 1.0 }
            };
        }
    }
}