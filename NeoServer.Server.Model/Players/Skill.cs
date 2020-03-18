using NeoServer.Server.Model.Creatures;

namespace NeoServer.Server.Model.Players
{
    public class Skill
    {
        public SkillType SkillAttribute { get; set; }
        public int Level { get; set; }
        public int Percent { get; set; }
    }
}