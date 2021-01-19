using NeoServer.Game.Common.Location;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Useables;
using NeoServer.Game.Contracts.World;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Movement;
using NeoServer.Server.Model.Players.Contracts;
using System;

namespace NeoServer.Server.Commands.Player
{

    public class PlayerUseItemOnCreatureCommand : Command
    {
        private readonly Game game;
        private UseItemOnCreaturePacket useItemPacket;
        private readonly IPlayer player;

        public PlayerUseItemOnCreatureCommand(IPlayer player, Game game, UseItemOnCreaturePacket useItemPacket)
        {
            this.game = game;
            this.player = player;
            this.useItemPacket = useItemPacket;
        }

        public override void Execute()
        {
            if (!game.CreatureManager.TryGetCreature(useItemPacket.CreatureId, out var creature)) return;

            IThing itemToUse = null;
            if (useItemPacket.FromLocation.Type == LocationType.Ground)
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
                WalkToMechanism.DoOperation(player, action, useItemPacket.FromLocation, game);
                return;
            }

            action?.Invoke();
        }
    }
}