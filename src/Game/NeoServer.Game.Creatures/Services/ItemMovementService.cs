using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Services;

namespace NeoServer.Game.Creatures.Services;

public class ItemMovementService : IItemMovementService
{
    private readonly IWalkToMechanism _walkToMechanism;

    public ItemMovementService(IWalkToMechanism walkToMechanism)
    {
        _walkToMechanism = walkToMechanism;
    }

    public Result<OperationResult<IItem>> Move(IPlayer player, IItem item, IHasItem from, IHasItem destination,
        byte amount,
        byte fromPosition, byte? toPosition)
    {
        if (player is null) return Result<OperationResult<IItem>>.NotPossible;

        if (item.IsCloseTo(player))
        {
            _walkToMechanism.WalkTo(player,
                () => player.MoveItem(item,from, destination,  amount, fromPosition, toPosition), item.Location);
        }
        return Result<OperationResult<IItem>>.Success;
    }
}