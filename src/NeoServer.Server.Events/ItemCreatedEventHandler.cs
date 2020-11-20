using NeoServer.Game.Contracts.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Events
{
    public class ItemCreatedEventHandler
    {
        private readonly Game game;
        public ItemCreatedEventHandler(Game game)
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
