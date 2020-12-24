using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using System;
using System.Collections.Generic;

namespace NeoServer.Server.Model.Players
{
    public class Skill : ISkill
    {
        public event OnLevelAdvance OnAdvance;
        public event OnIncreaseSkillPoints OnIncreaseSkillPoints;

        public SkillType Type { get; }

        public ushort Level { get; private set; }

        public double Count { get; private set; }

        public float Rate { get; }

        public double Target { get; }

        public double BaseIncrease => SkillsRates[Type].Item1;

        public double Percentage => CalculatePercentage(Count);

        //BaseIncrease and skill offset
        private IDictionary<SkillType, Tuple<double, double>> SkillsRates = new Dictionary<SkillType, Tuple<double, double>>()
        {
            {SkillType.Fist, new Tuple<double,double>(50,10) },
            {SkillType.Club, new Tuple<double,double>(50,10) },
            {SkillType.Sword, new Tuple<double,double>(50,10) },
            {SkillType.Axe, new Tuple<double,double>(50,10) },
            {SkillType.Distance, new Tuple<double,double>(30,10) },
            {SkillType.Shielding, new Tuple<double,double>(100,10) },
            {SkillType.Fishing, new Tuple<double,double>(20,10) },
            {SkillType.Magic, new Tuple<double,double>(1600,0) },
        };

        public Skill(SkillType type, float rate, ushort level = 0, double count = 0)
        {
        
            if (rate < 0)
            {
                throw new Exception($"{nameof(rate)} must be positive.");
            }

            if (count < 0)
            {
                throw new Exception($"{nameof(count)} cannot be negative.");
            }
            Type = type;
            Level =  level;
            Rate = rate;

            Count = count;
        }

        private double GetExpForLevel(int level) => ((50 * Math.Pow(level, 3)) / 3) - (100 * Math.Pow(level, 2)) + ((850 * level) / 3) - 200;
        private double GetPointsForLevel(int skillLevel, float vocationRate) => SkillsRates[Type].Item1 * Math.Pow(vocationRate, skillLevel - SkillsRates[Type].Item2);

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
                return CalculatePercentage(count, GetPointsForLevel(Level + 1, Rate));
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
            if (Type ==  SkillType.Level) IncreaseLevel();
            else IncreaseSkillLevel();
        }

        public void IncreaseLevel()
        {
            if (Type != SkillType.Level) return;

            var oldLevel = Level;
            while (Count >= GetExpForLevel(Level + 1))
            {
                Level++;
            }

            if (oldLevel != Level)
            {
                OnAdvance?.Invoke(Type, oldLevel, Level);
            }
        }
        public void IncreaseSkillLevel()
        {
            if (Type == SkillType.Level) return;

            var oldLevel = Level;
            while (Count >= GetPointsForLevel(Level + 1, Rate))
            {
                Count = 0;
                Level++;
            }

            if (oldLevel != Level)
            {
                OnAdvance?.Invoke(Type, oldLevel, Level);
            }

            OnIncreaseSkillPoints?.Invoke(Type);
        }
    }
}