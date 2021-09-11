using System;
using System.Collections.Generic;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures;
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

            var item = ItemFactory.Instance.Create(Params[0].ToString(), actor.Location,
                new Dictionary<ItemAttribute, IConvertible> {{ItemAttribute.Count, amount}});

            if (item is null) return false;

            if (actor is IPlayer player && player.Inventory.BackpackSlot is IContainer container &&
                container.AddItem(item, true).IsSuccess) return true;

            if (actor.Tile is ITile tile && tile.AddItem(item).IsSuccess) return true;

            error = InvalidOperation.NotEnoughRoom;
            return false;
        }
    }
}