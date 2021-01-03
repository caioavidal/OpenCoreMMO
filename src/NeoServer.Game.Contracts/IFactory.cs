using NeoServer.Game.Contracts.Items;

namespace NeoServer.Game.Contracts
{
    public interface IFactory
    {
        public event CreateItem OnItemCreated;
    }
}
