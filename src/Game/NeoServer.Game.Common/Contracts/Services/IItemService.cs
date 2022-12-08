using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Common.Contracts.Services;

public interface IItemService
{
    IItem Transform(Location.Structs.Location location, ushort fromItemId, ushort toItemId);
    IItem Create(Location.Structs.Location location, ushort id);
}