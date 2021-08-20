using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.Services
{
    public interface ISharedExperienceService
    {
        /// <summary>
        /// Does the party have experience sharing enabled?
        /// </summary>
        bool ExperienceSharingEnabled { get; set; }

        /// <summary>
        /// Sets the party the service is tracking.
        /// </summary>
        /// <param name="party">Party to be tracked by the service.</param>
        void StartTrackingPartyMembers(IParty party);

        /// <summary>
        /// Checks if the experience from the killed monster can be shared with party.
        /// </summary>
        /// <param name="monster">The monster killed by the party.</param>
        /// <returns>
        ///     Whether the experience from the killed monster can be shared.
        /// </returns>
        bool CanPartyReceiveSharedExperience(IMonster monster);

        /// <summary>
        /// Calculates the amount of experience the whole party will receive in total from killing the monster.
        /// This is the total experience, not individual player experience.
        /// </summary>
        /// <param name="monsterExperience">The experience provided from killing the monster.</param>
        /// <returns>
        ///     The amount of experience (unrounded) to be split among the party members.
        /// </returns>
        double GetTotalPartyBonusExperience(uint monsterExperience);
    }
}