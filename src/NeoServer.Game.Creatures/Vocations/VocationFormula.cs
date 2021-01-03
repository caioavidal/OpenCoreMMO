using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Creatures.Vocations
{

    public class VocationFormula : IVocationFormula
    {
        public string MeleeDamage { get; set; }
        public string DistDamage { get; set; }
        public string Defense { get; set; }
        public string Armor { get; set; }
    }
}
