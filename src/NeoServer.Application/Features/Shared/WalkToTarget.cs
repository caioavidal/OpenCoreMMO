using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Results;

namespace NeoServer.Application.Features.Shared;

public class WalkToTarget
{
    private readonly IPathFinder _pathFinder;
    private readonly IWalkToMechanism _walkToMechanism;

    public WalkToTarget(IPathFinder pathFinder, IWalkToMechanism walkToMechanism)
    {
        _pathFinder = pathFinder;
        _walkToMechanism = walkToMechanism;
    }

    public Result Go(IPlayer player, Location location, Action whenCloseToItem)
    {
        if (player.IsNextTo(location)) return Result.Success;

        var path = _pathFinder.Find(player, location, player.PathSearchParams, player.TileEnterRule);

        if (!path.Founded) return Result.Fail(InvalidOperation.ThereIsNoWay);

        _walkToMechanism.WalkTo(player, whenCloseToItem, location,
            path: path.Directions);

        return Result.Success;
    }

    public Result Go(IPlayer player, IThing target, Action whenCloseToItem)
    {
        return Go(player, target.Location, whenCloseToItem);
    }
}