using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Items
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