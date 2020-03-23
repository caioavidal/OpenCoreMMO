using NeoServer.Game.Creatures.Enums;
using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void OnLevelAdvance(SkillType skillType);

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

        void IncreaseCounter(double value);
    }
}
