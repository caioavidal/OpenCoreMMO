using NeoServer.Game.Common.Creatures;

namespace NeoServer.Game.Common.Contracts.Creatures;

public delegate void LevelAdvance(SkillType skillType, int fromLevel, int toLevel);
public delegate void LevelRegress(SkillType skillType, int fromLevel, int toLevel);

public delegate void IncreaseSkillPoints(SkillType skillType);

public interface ISkill
{
    SkillType Type { get; }

    ushort Level { get; }

    double Count { get; }
    double Target { get; }
    double BaseIncrease { get; }
    sbyte Bonus { get; }
    double GetPercentage(float rate);
    event LevelAdvance OnAdvance;
    event LevelRegress OnRegress;
    event IncreaseSkillPoints OnIncreaseSkillPoints;

    void IncreaseCounter(double value, float rate);
    void AddBonus(sbyte increase);
    void RemoveBonus(sbyte decrease);
    void DecreaseLevel(double lostExperience);
}