using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Common.Contracts.Combat.Attacks
{
    public interface IAreaAttack
    {
        Coordinate[] AffectedArea { get; }
    }
}