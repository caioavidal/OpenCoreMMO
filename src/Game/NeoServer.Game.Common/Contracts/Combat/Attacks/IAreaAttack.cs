using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Contracts.Combat
{
    public interface IAreaAttack
    {
         Coordinate[] AffectedArea { get; }
    }
}
