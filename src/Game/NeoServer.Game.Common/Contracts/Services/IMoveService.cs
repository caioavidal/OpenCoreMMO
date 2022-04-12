using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Common.Contracts.Services;

public interface IMoveService
{
    Result<OperationResult<IItem>> Move(IItem item, IHasItem from, IHasItem destination, byte amount, byte fromPosition, byte? toPosition);
}