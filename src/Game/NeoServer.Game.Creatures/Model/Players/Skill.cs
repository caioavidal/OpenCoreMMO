using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;

namespace NeoServer.Game.Creatures.Model.Players;

public class Skill : ISkill
{
    //BaseIncrease and skill offset
    private readonly IDictionary<SkillType, Tuple<double, double>> SkillsRates =
        new Dictionary<SkillType, Tuple<double, double>>
        {
            { SkillType.Fist, new Tuple<double, double>(50, 10) },
            { SkillType.Club, new Tuple<double, double>(50, 10) },
            { SkillType.Sword, new Tuple<double, double>(50, 10) },
            { SkillType.Axe, new Tuple<double, double>(50, 10) },
            { SkillType.Distance, new Tuple<double, double>(30, 10) },
            { SkillType.Shielding, new Tuple<double, double>(100, 10) },
            { SkillType.Fishing, new Tuple<double, double>(20, 10) },
            { SkillType.Magic, new Tuple<double, double>(1600, 0) }
        };

    public Skill(SkillType type, ushort level = 0, double count = 0)
    {
        if (count < 0) throw new Exception($"{nameof(count)} cannot be negative.");
        Type = type;
        Level = level;

        Count = count;
    }

    public event LevelAdvance OnAdvance;
    public event LevelRegress OnRegress;
    public event IncreaseSkillPoints OnIncreaseSkillPoints;

    public sbyte Bonus { get; private set; }

    public void AddBonus(sbyte increase)
    {
        Bonus = (sbyte)(Bonus + increase);
    }

    public void RemoveBonus(sbyte decrease)
    {
        Bonus = (sbyte)(Bonus - decrease);
    }

    public SkillType Type { get; }
    public ushort Level { get; private set; }
    public double Count { get; private set; }

    public double Target { get; }

    public double BaseIncrease => SkillsRates[Type].Item1;

    public double GetPercentage(float rate)
    {
        return CalculatePercentage(Count, rate);
    }

    public void IncreaseCounter(double value, float rate)
    {
        if (rate < 0) throw new Exception($"{nameof(rate)} must be positive.");

        Count += value;
        if (Type == SkillType.Level) IncreaseLevel();
        else IncreaseSkillLevel(rate);
    }

    private double GetExpForLevel(int level)
    {
        return 50 * Math.Pow(level, 3) / 3 - 100 * Math.Pow(level, 2) + 850 * level / 3 - 200;
    }

    private double GetPointsForLevel(int skillLevel, float vocationRate)
    {
        return SkillsRates[Type].Item1 * Math.Pow(vocationRate, skillLevel - SkillsRates[Type].Item2);
    }

    private double CalculatePercentage(double count, double nextLevelCount)
    {
        return Math.Min(100, count * 100 / nextLevelCount);
    }

    private double CalculatePercentage(double count, float rate)
    {
        if (Type == SkillType.Level)
        {
            var currentLevelExp = GetExpForLevel(Level);
            return CalculatePercentage(count, currentLevelExp);
        }

        if (Type == SkillType.Magic)
            return GetManaPercentage(count);
        return CalculatePercentage(count, GetPointsForLevel(Level + 1, rate));
    }

    private double GetManaPercentage(double manaSpent)
    {
        var rate = SkillsRates[Type].Item2;

        var reqMana = 1600 * Math.Pow(rate, Level);
        var modResult = reqMana % 20;
        if (modResult < 10)
            reqMana -= modResult;
        else
            reqMana -= modResult + 20;

        if (manaSpent > reqMana) manaSpent = 0;

        return CalculatePercentage(manaSpent, reqMana);
    }

    public void IncreaseLevel()
    {
        if (Type != SkillType.Level) return;

        var oldLevel = Level;
        while (Count >= GetExpForLevel(Level + 1)) Level++;

        if (oldLevel != Level) OnAdvance?.Invoke(Type, oldLevel, Level);
    }

    public void DecreaseLevel(double exp)
    {
        if (Type != SkillType.Level) return;

        var oldLevel = Level;
        Count -= exp;

        while (Count < 0)
        {
            Level--;
            var newExp = GetExpForLevel(Level);
            Count += newExp;
        }

        if (oldLevel != Level) OnRegress?.Invoke(Type, oldLevel, Level);
    }

    public void IncreaseSkillLevel(float rate)
    {
        if (Type == SkillType.Level) return;

        var oldLevel = Level;
        while (Count >= GetPointsForLevel(Level + 1, rate))
        {
            Count = 0;
            Level++;
        }

        if (oldLevel != Level) OnAdvance?.Invoke(Type, oldLevel, Level);

        OnIncreaseSkillPoints?.Invoke(Type);
    }
}