using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Enums;
using System;

namespace NeoServer.Server.Model.Players
{
    public class Skill:ISkill
    {
        public event OnLevelAdvance OnAdvance;

        public SkillType Type { get; }

        public ushort Level { get; private set; }

        public ushort MaxLevel { get; }

        public ushort DefaultLevel { get; }

        public double Count { get; private set; }

        public double Rate { get; }

        public double Target { get; private set; }

        public double BaseIncrease { get; }

        public Skill(SkillType type, ushort defaultLevel, double rate, double baseIncrease, ushort level = 0, ushort maxLevel = 1, double count = 0)
        {
            if (defaultLevel < 1)
            {
                throw new Exception($"{nameof(defaultLevel)} must be positive.");
            }

            if (maxLevel < 1)
            {
                throw new Exception($"{nameof(maxLevel)} must be positive.");
            }

            if (rate < 1)
            {
                throw new Exception($"{nameof(rate)} must be positive.");
            }

            if (baseIncrease < 1)
            {
                throw new Exception($"{nameof(baseIncrease)} must be positive.");
            }

            if (count < 0)
            {
                throw new Exception($"{nameof(count)} cannot be negative.");
            }

            if (maxLevel < defaultLevel)
            {
                throw new Exception($"{nameof(maxLevel)} must be at least the same value as {nameof(defaultLevel)}.");
            }

            Type = type;
            DefaultLevel = defaultLevel;
            MaxLevel = maxLevel;
            Level = Math.Min(MaxLevel, level == 0 ? defaultLevel : level);
            Rate = rate;
            BaseIncrease = baseIncrease;

            Target = CalculateNextTarget();
            Count = Math.Min(count, Target);
        }

        private double CalculateNextTarget()
        {
            var nextTarget = (Target * Rate) + BaseIncrease;

            if (Math.Abs(Target) < 0.001) // need to recalculate everything.
            {
                for (int i = 0; i < DefaultLevel - Level; i++) // how many advances we need to calculate
                {
                    nextTarget = (nextTarget * Rate) + BaseIncrease;
                }
            }

            return nextTarget;
        }

        public void IncreaseCounter(double value)
        {
            Count = Math.Min(Target, Count + value);

            if (Math.Abs(Count - Target) < 0.001) // Skill level advance
            {
                Level++;
                Target = CalculateNextTarget();

                // Invoke any subscribers to the level advance.
                OnAdvance?.Invoke(Type);
            }
        }
    }
}