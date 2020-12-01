using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Creatures.Structs;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Helpers;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Common.Conditions
{
    public class DamageCondition : BaseCondition
    {
        public DamageCondition(ConditionType type, int interval, ushort minDamage, ushort maxDamage) : base(0)
        {
            Type = type;
            Interval = interval;
            DamageType = ToDamageType(type);
            MaxDamage = maxDamage;
            MinDamage = minDamage;

            GenerateDamageList();
        }
        public override ConditionType Type { get; }
        public DamageType DamageType { get; set; }
        public int Interval
        {
            set
            {
                Cooldown = new CooldownTime(DateTime.Now, value);
            }
        }
        private Queue<ushort> DamageQueue;
        public override bool HasExpired => DamageQueue.Count <= 0;
        private CooldownTime Cooldown;
        private ushort MinDamage;
        private ushort MaxDamage;

        public void Execute(ICombatActor creature)
        {
            if (!Cooldown.Expired) return;

            Cooldown.Reset();
            if (!DamageQueue.TryDequeue(out var damage))
            {
                End();
                return;
            }
            creature.ReceiveAttack(null, new CombatDamage(damage, DamageType));
        }
        public bool Start(ICreature creature, ushort minDamage, ushort maxDamage)
        {
            if (maxDamage < MaxDamage) return false;

            MinDamage = minDamage;
            MaxDamage = maxDamage;

            Start(creature);
            return true;
        }
        public override bool Start(ICreature creature)
        {
            GenerateDamageList();
            return true;
        }

        private int GetStartDamage(int maxDamage, int amount)
        {
            var startDamage = 0;
            if (startDamage > maxDamage)
            {
                startDamage = maxDamage;
            }
            else if (startDamage == 0)
            {
                startDamage = (int)Math.Max(1, Math.Ceiling(amount / 20.0));
            }
            return startDamage;
        }
        private void GenerateDamageList()
        {
            if (DamageQueue is null)
            {
                DamageQueue = new Queue<ushort>();
            }

            int amount = (ushort)ServerRandom.Random.Next(minValue: MinDamage, maxValue: MaxDamage);
            var start = GetStartDamage(MaxDamage, amount);

            DamageQueue.Clear();

            amount = Math.Abs(amount);
            var sum = 0;
            double x1, x2;

            for (var i = start; i > 0; --i)
            {
                var n = start + 1 - i;
                var med = (n * amount) / start;

                do
                {
                    sum += i;
                    DamageQueue.Enqueue((ushort)i);

                    x1 = Math.Abs(1.0 - ((float)sum + i) / med);
                    x2 = Math.Abs(1.0 - ((float)sum / med));
                } while (x1 < x2);
            }
        }

        private DamageType ToDamageType(ConditionType type)
        {
            return type switch
            {
                ConditionType.Poison => DamageType.Earth,
                ConditionType.Fire => DamageType.FireField,
                ConditionType.Energy => DamageType.Energy,
                _ => DamageType.None
            };
        }
    }
}


