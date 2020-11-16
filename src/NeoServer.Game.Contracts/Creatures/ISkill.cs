using NeoServer.Game.Enums.Creatures;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void OnLevelAdvance(SkillType skillType, int fromLevel, int toLevel);

    public interface ISkill
    {
        event OnLevelAdvance OnAdvance;

        SkillType Type { get; }

        ushort Level { get; }

        ushort MaxLevel { get; }

        ushort DefaultLevel { get; }

        double Count { get; }

        double Rate { get; }

        double Target { get; }

        double BaseIncrease { get; }
        double Percentage { get; }

        void IncreaseCounter(double value);

    }
}
