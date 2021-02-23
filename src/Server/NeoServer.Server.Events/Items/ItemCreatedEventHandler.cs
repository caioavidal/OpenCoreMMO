using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Contracts;

namespace NeoServer.Server.Events
{
    public class ItemCreatedEventHandler
    {
        private readonly IGameServer game;
        public ItemCreatedEventHandler(IGameServer game)
        {
            this.game = game;
        }
        public void Execute(IItem item)
        {
            if (!(item is IDecayable decayable)) return;
            game.DecayableItemManager.Add(decayable);
        }
    }
}
