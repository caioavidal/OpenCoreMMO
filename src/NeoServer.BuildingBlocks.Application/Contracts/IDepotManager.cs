using NeoServer.Game.Common.Contracts.Items.Types.Containers;

namespace NeoServer.BuildingBlocks.Application.Contracts;

public interface IDepotManager
{
    void Load(uint playerId, IDepot depot);
    IDepot Get(uint playerId);
    bool Get(uint playerId, out IDepot depot);
    void Unload(uint playerId);
}