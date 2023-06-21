using System;
using NeoServer.Game.Common.Creatures;

namespace NeoServer.Game.Common.Parsers;

public class MessageParser //todo: maybe here is not the best place to this class stay
{
    public static string GetSkillAdvancedMessage(SkillType type, int fromLevel, int toLevel)
    {
        if (type == SkillType.Level)
            return $"You advanced from level {fromLevel} to level {toLevel}.";

        var skillText = type switch
        {
            SkillType.Sword => "sword fighting",
            SkillType.Club => "club fighting",
            SkillType.Axe => "axe fighting",
            SkillType.Distance => "distance fighting",
            SkillType.Magic => "magic",
            SkillType.Fishing => "fishing",
            SkillType.Shielding => "shielding",

            _ => throw new ArgumentException()
        };

        return $"You advanced to {skillText} level {toLevel}.";
    }

    public static string GetSkillRegressedMessage(SkillType type, int fromLevel, int toLevel)
    {
        if (type == SkillType.Level)
            return $"You were downgraded from level {fromLevel} to level {toLevel}.";

        var skillText = type switch
        {
            SkillType.Sword => "sword fighting",

            _ => throw new ArgumentException()
        };

        return $"Your {skillText} was downgraded from level {fromLevel} to level {toLevel}.";
    }
}