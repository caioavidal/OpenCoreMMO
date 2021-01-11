using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Items;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;

namespace NeoServer.Scripts.Spells.Support
{
    public class ItemCreator : CommandSpell
    {
        public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
        {
            error = InvalidOperation.NotPossible;
            if (Params?.Length == 0)
            {
                return false;
            }

            var amount = Params.Length > 1 && byte.TryParse(Params[1].ToString(), out var count) ? count > 100 ? 100 : count : 1;

            var item = ItemFactory.Instance.Create(Params[0].ToString(), actor.Location, new Dictionary<ItemAttribute, IConvertible>() { { ItemAttribute.Count, amount } });

            if (item is null) return false;

            if (actor is IPlayer player && player.Inventory.BackpackSlot is IContainer container && container.AddItem(item).IsSuccess) return true;

            if (actor.Tile is ITile tile && tile.AddItem(item).IsSuccess) return true;
            else
            {
                error = InvalidOperation.NotEnoughRoom;
                return false;
            }
        }
    }
}
