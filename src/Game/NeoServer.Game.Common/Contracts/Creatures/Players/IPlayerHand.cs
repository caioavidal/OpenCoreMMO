using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World.Tiles;

namespace NeoServer.Game.Common.Contracts.Creatures.Players;

public interface IPlayerHand
{
    Result<OperationResult<IItem>> Move(IItem item, IHasItem from, IHasItem destination, byte amount,
        byte fromPosition, byte? toPosition);

    Result<OperationResult<IItem>> PickItemFromGround(IItem item, ITile tile, byte amount = 1);
}