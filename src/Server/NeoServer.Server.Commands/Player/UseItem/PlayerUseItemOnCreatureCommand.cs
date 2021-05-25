using NeoServer.Game.Common.Location;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Useables;
using NeoServer.Game.Contracts.World;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Contracts;
using NeoServer.Server.Contracts.Commands;
using System;

namespace NeoServer.Server.Commands.Player
{

    public class PlayerUseItemOnCreatureCommand : ICommand
    {
        private readonly IGameServer game;
        private UseItemOnCreaturePacket useItemPacket;
        private readonly IPlayer player;

        public PlayerUseItemOnCreatureCommand(IGameServer game)
        {
            this.game = game;
        }

        public void Execute(IPlayer player, UseItemOnCreaturePacket useItemPacket)
        {
            if (!game.CreatureManager.TryGetCreature(useItemPacket.CreatureId, out var creature)) return;

            IThing itemToUse = null;

            if (useItemPacket.FromLocation.IsHotkey)
            {
                if (player.Inventory?.BackpackSlot?.GetFirstItem(useItemPacket.ClientId) is not IThing thing) return; //todo: slow method. we need a cache
                itemToUse = thing;
            }
            else if (useItemPacket.FromLocation.Type == LocationType.Ground)
            {
                if (game.Map[useItemPacket.FromLocation] is not ITile tile) return;
                if (tile.TopItemOnStack is null) return;
                itemToUse = tile.TopItemOnStack;
            }
            else if (useItemPacket.FromLocation.Type == LocationType.Slot)
            {
                if (player.Inventory[useItemPacket.FromLocation.Slot] is null) return;
                itemToUse = player.Inventory[useItemPacket.FromLocation.Slot];
            }
            else if (useItemPacket.FromLocation.Type == LocationType.Container)
            {
                if (player.Containers[useItemPacket.FromLocation.ContainerId][useItemPacket.FromLocation.ContainerSlot] is not IThing thing) return;
                itemToUse = thing;
            }

            if (itemToUse is not IUseableOn useableOn) return;

            Action action = () => player.Use(useableOn, creature);

            if (useItemPacket.FromLocation.Type == LocationType.Ground)
            {
                action?.Invoke();
                return;
            }

            action?.Invoke();
        }
    }
}