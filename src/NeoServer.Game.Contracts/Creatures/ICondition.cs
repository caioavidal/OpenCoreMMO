using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface ICondition
    {
        bool Start(ICreature creature);
        ConditionIcon Icons { get; }
        ConditionType Type { get; }

        ConditionSlot ConditionSlot { get; }

        long EndTime { get; }
        void End();

        bool IsPersistent { get; }
        bool HasExpired { get; }
    }
}
