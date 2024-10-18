using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Results;

namespace NeoServer.BuildingBlocks.Domain;

public interface IWalkToTarget
{
    Result Go(IPlayer player, Location location, Action whenCloseToItem);
    Result Go(IPlayer player, IThing target, Action whenCloseToItem);
}