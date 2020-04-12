using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Creatures.Players;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface ICondition
    {
        bool Start(ICreature creature);
        bool Execute(ICreature creature, int interval);
        void End(ICreature creature);
        void Add(ICreature creature, ICondition condition);
        ConditionIcon Icons { get; }
        ConditionType Type { get; }

        ICondition Clone();

        ConditionSlot ConditionSlot { get; }


        long EndTime { get; }
        int Ticks { get; }
        void SetTicks(uint ticks);

        bool IsPersistent { get; }
    }
}
