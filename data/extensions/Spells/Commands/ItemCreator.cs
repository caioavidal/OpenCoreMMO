using System;
using System.Collections.Generic;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Items.Factories;

namespace NeoServer.Extensions.Spells.Commands
{
    public class ItemCreator : CommandSpell
    {
        public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
        {
            error = InvalidOperation.NotPossible;
            if (Params?.Length == 0) return false;

            var amount = Params.Length > 1 && byte.TryParse(Params[1].ToString(), out var count)
                ? count > 100 ? 100 : count
                : 1;

            var item = Item(actor, amount);
            var result = CreateItem(actor, item);

            if (!result)
                error = InvalidOperation.NotEnoughRoom;

            return result;
        }

        private IItem Item(ICombatActor actor, int amount)
        {
            if (ushort.TryParse(Params[0].ToString(), out var typeId))
                return ItemFactory.Instance.Create(typeId, actor.Location,
                    new Dictionary<ItemAttribute, IConvertible> { { ItemAttribute.Count, amount } });

            var item = ItemFactory.Instance.Create(Params[0].ToString(), actor.Location,
                new Dictionary<ItemAttribute, IConvertible> { { ItemAttribute.Count, amount } });

            return item;
        }

        private bool CreateItem(ICombatActor actor, IItem item)
        {
            if (item is null) return false;

            if (!item.IsPickupable && actor.Tile is { } tile && tile.AddItem(item).Succeeded) return true;

            if (item.IsPickupable && actor is IPlayer player && player.Inventory.BackpackSlot is { } container &&
                container.AddItem(item, true).Succeeded) return true;

            return false;
        }
    }
}