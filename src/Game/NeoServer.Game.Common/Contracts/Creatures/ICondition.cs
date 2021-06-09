using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface ICondition
    {
        ConditionIcon Icons { get; }
        ConditionType Type { get; }

        long EndTime { get; }

        bool IsPersistent { get; }
        bool HasExpired { get; }

        /// <summary>
        ///     Remaining time in milliseconds
        /// </summary>
        long RemainingTime { get; }

        bool Start(ICreature creature);
        void End();

        /// <summary>
        ///     Extends condition duration in milliseconds
        /// </summary>
        /// <param name="duration"></param>
        void Extend(uint duration, uint maxDuration = uint.MaxValue);
    }
}