namespace NeoServer.Game.Contracts.Creatures
{
    public interface IVocationFormula
    {
        string Armor { get; set; }
        string Defense { get; set; }
        string DistDamage { get; set; }
        string MeleeDamage { get; set; }
    }

}
