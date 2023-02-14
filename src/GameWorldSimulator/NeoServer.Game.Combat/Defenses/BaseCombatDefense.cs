using NeoServer.Game.Common.Contracts.Combat;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Combat.Defenses;

public abstract class BaseCombatDefense : ICombatDefense
{
    /// <summary>
    ///     Minumum interval to occurr
    /// </summary>
    public ushort Interval { get; init; }

    /// <summary>
    ///     Chance to occurr - 0 to 100
    /// </summary>
    public byte Chance { get; init; }

    /// <summary>
    ///     action to execute when defence occurrs
    /// </summary>
    /// <param name="actor"></param>
    public abstract void Defend(ICombatActor actor);
}