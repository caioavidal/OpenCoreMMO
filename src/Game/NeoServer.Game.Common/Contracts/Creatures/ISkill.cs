using NeoServer.Game.Common.Creatures;

namespace NeoServer.Game.Common.Contracts.Creatures
{
    public delegate void LevelAdvance(SkillType skillType, int fromLevel, int toLevel);

    public delegate void IncreaseSkillPoints(SkillType skillType);

    public interface ISkill
    {
        SkillType Type { get; }

        ushort Level { get; }

        double Count { get; }

        float Rate { get; }

        double Target { get; }

        double BaseIncrease { get; }
        double Percentage { get; }
        byte Bonus { get; }
        event LevelAdvance OnAdvance;
        event IncreaseSkillPoints OnIncreaseSkillPoints;

        void IncreaseCounter(double value);
        void AddBonus(byte increase);
        void RemoveBonus(byte decrease);
    }
}