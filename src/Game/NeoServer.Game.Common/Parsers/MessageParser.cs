using NeoServer.Game.Common.Creatures;
using System;

namespace NeoServer.Game.Common.Parsers
{
    public class MessageParser //todo: maybe here is not the best place to this class stay
    {
        public static string GetSkillAdvancedMessage(SkillType type, int level)
        {
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

            return $"You advanced to {skillText} level { level}.";
        }
    }
}
