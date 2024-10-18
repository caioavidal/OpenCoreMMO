namespace NeoServer.Game.Common.Combat.Formula;

public readonly struct DamageFormula
{
    public required CombatFormula Type { get; init; }
    public required double MinA { get; init; }
    public required double MinB { get; init; }
    public required double MaxA { get; init; }
    public required double MaxB { get; init; }
}