using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Creatures.Vocations
{

    public class VocationSkill : IVocationSkill
    {
        public string Id { get; set; }
        public string Multiplier { get; set; }
    }
}
