using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.Services
{
    public interface IPartyShareExperienceService
    {
        /// <summary>
        /// Checks if the experience from the killed monster can be shared with party.
        /// </summary>
        /// <param name="monster">The monster killed by the party.</param>
        /// <returns>
        ///     Whether the experience from the killed monster can be shared.
        /// </returns>
        bool CanPartyReceiveSharedExperience(IParty party, IMonster monster);

        /// <summary>
        /// Calculates the amount of experience the whole party will receive in total from killing the monster.
        /// This is the total experience, not individual player experience.
        /// </summary>
        /// <param name="monster">The monster killed by the party.</param>
        /// <returns>
        ///     The amount of experience (unrounded) to be split among the party members.
        /// </returns>
        double GetTotalPartyExperience(IParty party, IMonster monster);
    }
}