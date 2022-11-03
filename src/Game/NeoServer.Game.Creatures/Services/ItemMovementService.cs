using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Texts;

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

        if (item.Location.Type == LocationType.Ground)
        {
            if (item.Location.Z < player.Location.Z)
            {
                OperationFailService.Display(player.CreatureId, TextConstants.FIRST_GO_UPSTAIRS);
                return Result<OperationResult<IItem>>.NotPossible;
            }

            if (item.Location.Z > player.Location.Z)
            {
                OperationFailService.Display(player.CreatureId, TextConstants.FIRST_GO_DOWNSTAIRS);
                return Result<OperationResult<IItem>>.NotPossible;
            }
        }

        if (!item.IsCloseTo(player))
        {
            _walkToMechanism.WalkTo(player,
                () => player.MoveItem(item, from, destination, amount, fromPosition, toPosition), item.Location);
            return Result<OperationResult<IItem>>.Success;
        }

        return player.MoveItem(item, from, destination, amount, fromPosition, toPosition);
    }
}