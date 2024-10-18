using NeoServer.BuildingBlocks.Domain;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Results;

namespace NeoServer.PacketHandler.Features.Shared;

public class WalkToTarget(IPathFinder pathFinder, IWalkToMechanism walkToMechanism) : IWalkToTarget
{
    public Result Go(IPlayer player, Location location, Action whenCloseToItem)
    {
        if (player.IsNextTo(location)) return Result.Success;

        var path = pathFinder.Find(player, location, player.PathSearchParams, player.TileEnterRule);

        if (!path.Founded) return Result.Fail(InvalidOperation.ThereIsNoWay);

        walkToMechanism.WalkTo(player, whenCloseToItem, location,
            path: path.Directions);

        return Result.Success;
    }

    public Result Go(IPlayer player, IThing target, Action whenCloseToItem)
    {
        return Go(player, target.Location, whenCloseToItem);
    }
}