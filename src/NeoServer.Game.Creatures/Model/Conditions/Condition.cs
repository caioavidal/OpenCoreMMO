using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Creatures.Players;
using System;

namespace NeoServer.Game.Creatures.Model.Conditions
{
    public abstract class Condition : ICondition
    {
        private bool isBuff;
        public ConditionIcon Icons => isBuff ? ConditionIcon.PartyBuff : 0;

        public ConditionType Type => throw new NotImplementedException();

        public long EndTime { get; private set; }

        public int Ticks { get; private set; }

        public bool IsPersistent
        {
            get
            {
                if (Ticks == -1)
                {
                    return false;
                }
                if (!(ConditionSlot == ConditionSlot.Default ||
                    ConditionSlot == ConditionSlot.Combat ||
                    Type == ConditionType.Muted))
                {
                    return false;
                }

                return true;
            }
        }

        public ConditionSlot ConditionSlot => throw new NotImplementedException();

        public abstract void Add(ICreature creature, ICondition condition);

        public abstract ICondition Clone();

        public abstract void End(ICreature creature);

        public bool Execute(ICreature creature, int interval)
        {
            if (Ticks == -1)
            {
                return true;
            }

            Ticks = Math.Max(0, Ticks - interval);
            return EndTime >= DateTime.Now.Ticks;
        }

        public void SetTicks(uint ticks)
        {
            throw new NotImplementedException();
        }

        public bool Start(ICreature creature)
        {
            if (Ticks > 0)
            {
                EndTime = DateTime.Now.Ticks + Ticks;
            }
            return true;
        }

        protected bool UpdateCondition(Condition condition)
        {
            if (Type != condition.Type)
            {
                return false;
            }

            if (Ticks == -1 && condition.Ticks > 0) //condition expired
            {
                return false;
            }

            if (condition.Ticks >= 0 && EndTime > (DateTime.Now.Ticks + condition.Ticks))
            {
                return false;
            }

            return true;
        }
    }
}
