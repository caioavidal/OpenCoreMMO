using Mediator;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Services;

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
    public ValueTask<Unit> Go(IPlayer player, Location location, Action whenCloseToItem)
    {
        if (player.IsNextTo(location)) return Unit.ValueTask;
        
        var path = _pathFinder.Find(player, location, player.PathSearchParams, player.TileEnterRule);

        if (!path.Founded) OperationFailService.Send(player, "There is no way.");

        _walkToMechanism.WalkTo(player, whenCloseToItem, location,
            path: path.Directions);
        
        return Unit.ValueTask;
    }
    public ValueTask<Unit> Go(IPlayer player, IThing target, Action whenCloseToItem) => Go(player, target.Location, whenCloseToItem);
}