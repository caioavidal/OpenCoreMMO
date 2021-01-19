using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Runes;
using NeoServer.Game.Contracts.Items.Types.Useables;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Effects.Magical;

namespace NeoServer.Game.Items.Events
{
    public class FieldRuneUsedEventHandler : IGameEventHandler
    {
        private readonly IMap map;
        private readonly IItemFactory itemFactory;
        public FieldRuneUsedEventHandler(IMap map, IItemFactory itemFactory)
        {
            this.map = map;
            this.itemFactory = itemFactory;
        }

        public void Execute(ICreature usedBy, ITile onTile, IUseableOnTile item)
        {
            if (item is not IFieldRune rune) return;

            if (!string.IsNullOrWhiteSpace(rune.Area))
            {
                foreach (var coordinate in AreaEffect.Create(onTile.Location, rune.Area))
                {
                    var location = coordinate.Location;
                    var field = ItemFactory.Instance.Create(rune.Field, location, null);

                    if (map[location] is not IDynamicTile tile) continue;

                    tile.AddItem(field);

                    CauseDamageToCreaturesOnTile(field, tile);
                }
            }
            else
            {
                var field = ItemFactory.Instance.Create(rune.Field, onTile.Location, null);
                onTile.AddItem(field);

                CauseDamageToCreaturesOnTile(field, onTile);
            }
        }

        public void CauseDamageToCreaturesOnTile(IItem item, ITile tile)
        {
            if (item is not IMagicField field) return;
            if (tile is not IDynamicTile onTile) return;

            if (!onTile.HasCreature) return;

            foreach (var creature in onTile.Creatures)
            {
                field.CauseDamage(creature.Value);
            }
        }
    }
}
