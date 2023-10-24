using Mediator;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Services;

namespace NeoServer.Application.Features.UseItem.Common;

public class WalkToItem
{
    private readonly IPathFinder _pathFinder;
    private readonly IWalkToMechanism _walkToMechanism;

    public WalkToItem(IPathFinder pathFinder, IWalkToMechanism walkToMechanism)
    {
        _pathFinder = pathFinder;
        _walkToMechanism = walkToMechanism;
    }
    public ValueTask<Unit> Go(IPlayer player, IItem item, Action whenCloseToItem)
    {
        if (player.IsNextTo(item)) return Unit.ValueTask;
        
        var path = _pathFinder.Find(player, item.Location, player.PathSearchParams, player.TileEnterRule);

        if (!path.Founded) OperationFailService.Send(player, "There is no way.");

        _walkToMechanism.WalkTo(player, whenCloseToItem, item.Location,
            path: path.Directions);
        
        return Unit.ValueTask;
    }
}