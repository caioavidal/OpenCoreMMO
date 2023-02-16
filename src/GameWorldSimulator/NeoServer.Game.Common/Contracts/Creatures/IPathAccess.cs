namespace NeoServer.Game.Common.Contracts.Creatures;

public interface IPathAccess
{
    PathFinder FindPathToDestination { get; }
    CanGoToDirection CanGoToDirection { get; }
}