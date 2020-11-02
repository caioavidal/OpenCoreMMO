using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Creatures.Players;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NeoServer.Game.Creatures.Model.Conditions
{
    public abstract class BaseCondition : ICondition
    {
        private bool isBuff;

        protected BaseCondition(uint duration)
        {
            Duration = duration;
        }

        public Action OnEnd { private get; set; }


        public ConditionIcon Icons => isBuff ? ConditionIcon.PartyBuff : 0;

        public abstract ConditionType Type { get; }
        public uint Duration { get; }
        public long EndTime { get; private set; }

        public int Ticks { get; private set; }

        public void End()
        {
            OnEnd?.Invoke();
        }
    
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


        public void SetTicks(uint ticks)
        {
            throw new NotImplementedException();
        }

        public bool Start(ICreature creature)
        {
            if (UpdateCondition())
            {
                return true;
            }

            if (Ticks > 0)
            {
                EndTime = DateTime.Now.Ticks + Ticks;
            }
            return true;
        }

        protected bool UpdateCondition()
        {
            if (Ticks > 0) //condition expired
            {
                return false;
            }

            if (Ticks >= 0 && EndTime > (DateTime.Now.Ticks + Ticks))
            {
                return false;
            }

            return true;
        }
    }
}
