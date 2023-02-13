using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Common.Services;
using NeoServer.Game.Common.Texts;

namespace NeoServer.Game.Creatures.Services;

public class ItemMovementService : IItemMovementService
{
    private readonly IWalkToMechanism _walkToMechanism;

    public ItemMovementService(IWalkToMechanism walkToMechanism)
    {
        _walkToMechanism = walkToMechanism;
    }

    public Result<OperationResultList<IItem>> Move(IPlayer player, IItem item, IHasItem from, IHasItem destination,
        byte amount,
        byte fromPosition, byte? toPosition)
    {
        if (player is null) return Result<OperationResultList<IItem>>.NotPossible;

        if (item.Location.Type == LocationType.Ground)
        {
            if (item.Location.Z < player.Location.Z)
            {
                OperationFailService.Send(player.CreatureId, TextConstants.FIRST_GO_UPSTAIRS);
                return Result<OperationResultList<IItem>>.NotPossible;
            }

            if (item.Location.Z > player.Location.Z)
            {
                OperationFailService.Send(player.CreatureId, TextConstants.FIRST_GO_DOWNSTAIRS);
                return Result<OperationResultList<IItem>>.NotPossible;
            }
        }

        if (!item.IsCloseTo(player))
        {
            _walkToMechanism.WalkTo(player,
                () => player.MoveItem(item, from, destination, amount, fromPosition, toPosition), item.Location);
            return Result<OperationResultList<IItem>>.Success;
        }

        return player.MoveItem(item, from, destination, amount, fromPosition, toPosition);
    }
}