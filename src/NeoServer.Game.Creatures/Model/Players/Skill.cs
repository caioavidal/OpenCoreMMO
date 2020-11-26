using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using System;
using System.Collections.Generic;

namespace NeoServer.Server.Model.Players
{
    public class Skill : ISkill
    {
        public event OnLevelAdvance OnAdvance;
        public SkillType Type { get; }

        public ushort Level { get; private set; }

        public ushort MaxLevel { get; }

        public ushort DefaultLevel { get; }

        public double Count { get; private set; }

        public double Rate { get; }

        public double Target { get; }

        public double BaseIncrease { get; }

        public double Percentage => CalculatePercentage(Count);


        //BaseIncrease and Rate
        private IDictionary<SkillType, Tuple<double, double>> SkillsRates = new Dictionary<SkillType, Tuple<double, double>>()
        {
            {SkillType.Fist, new Tuple<double,double>(50,1.5) },
            {SkillType.Club, new Tuple<double,double>(50,2.0) },
            {SkillType.Sword, new Tuple<double,double>(50,2.0) },
            {SkillType.Axe, new Tuple<double,double>(50,2.0) },
            {SkillType.Distance, new Tuple<double,double>(30,2.0) },
            {SkillType.Shielding, new Tuple<double,double>(100,1.5) },
            {SkillType.Fishing, new Tuple<double,double>(20,1.1) },
            {SkillType.Magic, new Tuple<double,double>(0,4) },
        };

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

            //Target = CalculateNextTarget(count);
            //Percentage = CalculatePercentage(count);
            Count = count;
        }

        private double GetExpForLevel(int level) => ((50 * Math.Pow(level, 3)) / 3) - (100 * Math.Pow(level, 2)) + ((850 * level) / 3) - 200;

        private double CalculatePercentage(double count, double nextLevelCount) => Math.Min(100, (count * 100) / nextLevelCount);
        private double CalculatePercentage(double count)
        {
            if (Type == SkillType.Level)
            {
                var currentLevelExp = GetExpForLevel(Level);

                var nextLevelExp = GetExpForLevel(Level + 1);

                if (count < currentLevelExp || count > nextLevelExp)
                {
                    Count = currentLevelExp;
                }
                return CalculatePercentage(count - currentLevelExp, nextLevelExp - currentLevelExp);
            }
            else if (Type == SkillType.Magic)
            {
                return GetManaPercentage(count);
            }
            else
            {

                var baseIncrease = SkillsRates[Type].Item1;
                var rate = SkillsRates[Type].Item2;

                var nextSkillTries = baseIncrease * Math.Pow((rate), Level - 10);
                return CalculatePercentage(count, nextSkillTries);
            }
        }

        private double GetManaPercentage(double manaSpent)
        {
            var rate = SkillsRates[Type].Item2;

            var reqMana = 1600 * Math.Pow(rate, Level);
            var modResult = reqMana % 20;
            if (modResult < 10)
            {
                reqMana -= modResult;
            }
            else
            {
                reqMana -= modResult + 20;
            }

            if (manaSpent > reqMana)
            {
                manaSpent = 0;
            }

            return CalculatePercentage(manaSpent, reqMana);
        }

        public void IncreaseCounter(double value)
        {

            Count += value;
            IncreaseLevel();
        }

        public void IncreaseLevel()
        {
            var oldLevel = Level;
            while (Count >= GetExpForLevel(Level + 1))
            {
                Level++;
            }

            if(oldLevel != Level)
            {
                OnAdvance?.Invoke(Type, oldLevel, Level);
            }
        }
    }
}