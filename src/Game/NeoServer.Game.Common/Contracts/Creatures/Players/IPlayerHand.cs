using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Common.Contracts.Creatures.Players;

public interface IPlayerHand
{
    Result<OperationResultList<IItem>> Move(IItem item, IHasItem from, IHasItem destination, byte amount,
        byte fromPosition, byte? toPosition);

    Result<OperationResultList<IItem>> PickItemFromGround(IItem item, ITile tile, byte amount = 1);
}