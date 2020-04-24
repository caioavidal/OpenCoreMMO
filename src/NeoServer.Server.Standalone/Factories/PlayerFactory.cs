using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
using NeoServer.Server.Events;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;

namespace NeoServer.Server.Standalone.Factories
{
    internal class PlayerFactory
    {
        private readonly Func<ushort, Location, IDictionary<ItemAttribute, IConvertible>, IItem> itemFactory;
        private readonly PlayerTurnToDirectionEventHandler playerTurnToDirectionEventHandler;
        private readonly PlayerWalkCancelledEventHandler playerWalkCancelledEventHandler;


        public PlayerFactory(Func<ushort, Location, IDictionary<ItemAttribute, IConvertible>, IItem> itemFactory, PlayerTurnToDirectionEventHandler playerTurnToDirectionEventHandler, PlayerWalkCancelledEventHandler playerWalkCancelledEventHandler)
        {
            this.itemFactory = itemFactory;
            this.playerTurnToDirectionEventHandler = playerTurnToDirectionEventHandler;
            this.playerWalkCancelledEventHandler = playerWalkCancelledEventHandler;
        }
        private readonly static Random random = new Random();
        public IPlayer Create(PlayerModel player)
        {
            var id = (uint)random.Next();

            var newPlayer = new Player(id,
                player.CharacterName,
                player.ChaseMode,
                player.Capacity,
                player.HealthPoints,
                player.MaxHealthPoints,
                player.Vocation,
                player.Gender,
                player.Online,
                player.Mana,
                player.MaxMana,
                player.FightMode,
                player.SoulPoints,
                player.MaxSoulPoints,
                player.Skills,
                player.StaminaMinutes,
                player.Outfit,
                ConvertToInventory(player.Inventory, player.Location),
                player.Speed,
                player.Location
                );

            newPlayer.OnTurnedToDirection += playerTurnToDirectionEventHandler.Execute;
            newPlayer.OnCancelledWalk += playerWalkCancelledEventHandler.Execute;
            return newPlayer;
        }

        private IDictionary<Slot, Tuple<IItem, ushort>> ConvertToInventory(Dictionary<Slot, ushort> inventory, Location location)
        {
            var inventoryDic = new Dictionary<Slot, Tuple<IItem, ushort>>();
            foreach (var slot in inventory)
            {

                inventoryDic.Add(slot.Key, new Tuple<IItem, ushort>(itemFactory(slot.Value, location, null), slot.Value));

            }
            return inventoryDic;
        }
    }
}
