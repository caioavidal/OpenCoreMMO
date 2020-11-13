namespace NeoServer.Game.Creatures.Combat.Attacks
{
    //public class SpreadCombatAttack : DistanceCombatAttack, IDistanceSpreadCombatAttack, IAreaAttack
    //{
    //    public SpreadCombatAttack(DamageType damageType, CombatAttackOption option) : base(damageType, option)
    //    {
    //    }

    //    public override void BuildAttack(ICombatActor actor, ICombatActor enemy)
    //    {
    //        var i = 0;

    //        var affectedLocations = SpreadEffect.Create(actor.Direction, Length);
    //        AffectedArea = new Coordinate[affectedLocations.Length];

    //        foreach (var location in affectedLocations)
    //        {
    //            AffectedArea[i++] = actor.Location.Translate() + location;
    //        }

    //        base.BuildAttack(actor, enemy);
    //    }

    //    public override ushort CalculateDamage(ushort attackPower, ushort minAttackPower) => base.CalculateDamage(Option.MaxDamage, Option.MinDamage);

    //    public override void CauseDamage(ICombatActor actor, ICombatActor enemy)
    //    {
    //        base.CauseDamage(actor, enemy);
    //    }

    //    public byte Spread => Option.Spread;
    //    public byte Length => Option.Length;
    //    public Coordinate[] AffectedArea { get; private set; }
    //}
}
