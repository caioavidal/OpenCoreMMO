using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Common.Contracts;

public interface IFactory
{
    public event CreateItem OnItemCreated;
}