using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Creatures.Players;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface ICondition
    {
        bool Start(ICreature creature);
        ConditionIcon Icons { get; }
        ConditionType Type { get; }

        ConditionSlot ConditionSlot { get; }

        long EndTime { get; }
        int Ticks { get; }
        void SetTicks(uint ticks);

        bool IsPersistent { get; }
    }
}
