using NeoServer.Game.Contracts.Items;
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
        private readonly Func<ushort, IItem> itemFactory;
        private readonly PlayerTurnToDirectionEventHandler playerTurnToDirectionEventHandler;
        

        public PlayerFactory(Func<ushort, IItem> itemFactory, PlayerTurnToDirectionEventHandler playerTurnToDirectionEventHandler)
        {
            this.itemFactory = itemFactory;
            this.playerTurnToDirectionEventHandler = playerTurnToDirectionEventHandler;
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
                ConvertToInventory(player.Inventory),
                player.Speed,
                player.Location
                );

            newPlayer.OnTurnedToDirection += playerTurnToDirectionEventHandler.Execute;
            return newPlayer;
        }

        private IDictionary<Slot, Tuple<IItem, ushort>> ConvertToInventory(Dictionary<Slot, ushort> inventory)
        {
            var inventoryDic = new Dictionary<Slot, Tuple<IItem, ushort>>();
            foreach (var slot in inventory)
            {
                inventoryDic.Add(slot.Key, new Tuple<IItem, ushort>(itemFactory(slot.Value), slot.Value));
            }
            return inventoryDic;
        }
    }
}
