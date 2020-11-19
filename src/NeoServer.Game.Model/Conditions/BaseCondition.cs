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
            Duration = duration * TimeSpan.TicksPerMillisecond;
        }

        public Action OnEnd { private get; set; }


        public ConditionIcon Icons => isBuff ? ConditionIcon.PartyBuff : 0;

        public abstract ConditionType Type { get; }
        public long Duration { get; }
        public long EndTime { get; private set; }

        public void End()
        {
            OnEnd?.Invoke();
        }

        public bool IsPersistent
        {
            get
            {
                //todo
                //if (Ticks == -1)
                //{
                //    return false;
                //}
                //if (!(ConditionSlot == ConditionSlot.Default ||
                //    ConditionSlot == ConditionSlot.Combat ||
                //    Type == ConditionType.Muted))
                //{
                //    return false;
                //}

                return true;
            }
        }

        public ConditionSlot ConditionSlot => throw new NotImplementedException();

        public bool Start(ICreature creature)
        {
           
            EndTime = DateTime.Now.Ticks + Duration;

            return true;
        }
        public bool HasExpired => EndTime < DateTime.Now.Ticks;
    }
}
