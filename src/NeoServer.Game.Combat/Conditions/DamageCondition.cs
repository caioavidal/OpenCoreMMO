using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Creatures.Structs;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Effects.Parsers;
using NeoServer.Game.Parsers.Effects;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Common.Conditions
{
    public class DamageCondition : BaseCondition
    {
        public DamageCondition(ConditionType type, int interval, ushort minDamage, ushort maxDamage, EffectT effect = EffectT.None) : base(0)
        {
            Type = type;
            Interval = interval;
            DamageType = ConditionTypeParser.Parse(type);
            MaxDamage = maxDamage;
            MinDamage = minDamage;
            Effect = effect;

        }
        public DamageCondition(ConditionType type, int interval, byte amount, ushort damage, EffectT effect = EffectT.None) : base(0)
        {
            if (amount == 0) return;

            Type = type;
            Interval = interval;
            DamageType = ConditionTypeParser.Parse(type);
            MaxDamage = damage;
            MinDamage = damage;
            Effect = effect;
            Amount = amount;
        }
        public byte Amount { get; }
        public override ConditionType Type { get; }
        public DamageType DamageType { get; set; }
        public EffectT Effect { get; }
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
            creature.ReceiveAttack(null, new CombatDamage(damage, DamageType, DamageEffectParser.Parse(DamageType)));
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
            if (Amount == 0)
            {
                GenerateDamageList();
            }
            else
            {
                GenerateDamageList(Amount);
            }
            return true;
        }
        public bool Restart(byte amount)
        {
            GenerateDamageList(amount);
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
        private void GenerateDamageList(byte amount)
        {
            DamageQueue = DamageQueue ?? new Queue<ushort>();
            for (int i = 0; i < amount - DamageQueue.Count; i++)
            {
                DamageQueue.Enqueue(MaxDamage);
            }
        }
        private void GenerateDamageList()
        {
            if (DamageQueue is null)
            {
                DamageQueue = new Queue<ushort>();
            }

            int amount = (ushort)GameRandom.Random.Next(minValue: MinDamage, maxValue: MaxDamage);
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

   
    }
}


