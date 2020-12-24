using NeoServer.Game.Common.Creatures;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void OnLevelAdvance(SkillType skillType, int fromLevel, int toLevel);
    public delegate void OnIncreaseSkillPoints(SkillType skillType);
    public interface ISkill
    {
        event OnLevelAdvance OnAdvance;
        event OnIncreaseSkillPoints OnIncreaseSkillPoints;

        SkillType Type { get; }

        ushort Level { get; }

        double Count { get; }

        float Rate { get; }

        double Target { get; }

        double BaseIncrease { get; }
        double Percentage { get; }

        void IncreaseCounter(double value);
    }
}
