using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;

namespace NeoServer.Server.Events.Creature
{
    public class CreatureDroppedLootEventHandler
    {
        private readonly Game game;
        private readonly IItemFactory itemFactory;
        public CreatureDroppedLootEventHandler(Game game, IItemFactory itemFactory)
        {
            this.game = game;
            this.itemFactory = itemFactory;
        }
        public void Execute(ICombatActor creature, ILoot loot)
        {
            CreateLoot(creature, loot);

            foreach (var spectator in game.Map.GetPlayersAtPositionZone(creature.Location))
            {
                if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out IConnection connection))
                {
                    continue;
                }

                var message = $"Loot of a {creature.Name.ToLower()}: {creature.Corpse?.ToString()}."; //todo 
                connection.OutgoingPackets.Enqueue(new TextMessagePacket(message, TextMessageOutgoingType.MessageInfoDescription));
                connection.Send();
            }
        }

        public void CreateLoot(ICombatActor creature, ILoot loot)
        {
            if (creature is not IMonster monster) return;

            CreateLootItems(loot.Items, monster.Location, monster.Corpse);
        }

        public void CreateLootItems(ILootItem[] items, Location location, IContainer container)
        {
            var attributes = new Dictionary<ItemAttribute, IConvertible>();

            foreach (var item in items)
            {
                if (item.Amount > 1) attributes.TryAdd(ItemAttribute.Count, item.Amount);

                var itemToDrop = itemFactory.Create(item.ItemId, location, attributes);

                if (itemToDrop is IContainer && item.Items?.Length == 0) continue;

                if (itemToDrop is IContainer c && item.Items?.Length > 0) CreateLootItems(item.Items, location, c);

                container?.TryAddItem(itemToDrop);
            }
        }
    }
}
