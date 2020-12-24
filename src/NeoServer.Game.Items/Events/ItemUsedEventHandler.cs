using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Items.Events
{
    public class ItemUsedEventHandler: IGameEventHandler
    {
        private readonly IMap map;
        private readonly IItemFactory itemFactory;
        public ItemUsedEventHandler(IMap map, IItemFactory itemFactory)
        {
            this.map = map;
            this.itemFactory = itemFactory;
        }

        public void Execute(IPlayer usedBy, ICreature creature, IItem item)
        {
            Transform(usedBy, creature, item);
            Say(creature, item);
        }

        private void Transform(IPlayer usedBy, ICreature creature, IItem item)
        {
            if (item?.TransformTo == 0) return;
            var createdItem = itemFactory.Create(item.TransformTo, creature.Location, null);

            if (map[creature.Location] is not IDynamicTile tile) return;

            if (item?.Location.Type == Common.Location.LocationType.Ground)
            {
                tile.AddThing(createdItem);
            }

            if (item?.Location.Type == Common.Location.LocationType.Container)
            {
                var container = usedBy.Containers[item.Location.ContainerId];

                var result = container.AddThing(createdItem);
                if(!result.IsSuccess) tile.AddThing(createdItem);
            }
        }
        private void Say(ICreature creature, IItem item)
        {
            if (item is IConsumable consumable && !string.IsNullOrWhiteSpace(consumable.Sentence))
            {
                creature.Say(consumable.Sentence, Common.Talks.TalkType.MonsterSay);
            }          
        }
    }
}
