using Mediator;
using NeoServer.BuildingBlocks.Application;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Effects.Magical;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.PacketHandler.Features.Player.UseItem.UseFieldRune;

public sealed record UseFieldRuneCommand(IPlayer Player, IFieldRune Rune, Location Location) : ICommand;

public class UseFieldRuneCommandHandler : ICommandHandler<UseFieldRuneCommand>
{
    private readonly IAreaEffectStore _areaEffectStore;
    private readonly IItemFactory _itemFactory;
    private readonly IMap _map;
    private readonly IMediator _mediator;

    public UseFieldRuneCommandHandler(IMap map, IItemFactory itemFactory, IAreaEffectStore areaEffectStore,
        IMediator mediator)
    {
        _map = map;
        _itemFactory = itemFactory;
        _areaEffectStore = areaEffectStore;
        _mediator = mediator;
    }

    public ValueTask<Unit> Handle(UseFieldRuneCommand request, CancellationToken cancellationToken)
    {
        var player = request.Player;
        var location = request.Location;

        var item = request.Rune;

        var tile = _map.GetTile(location);

        item.Use(player, tile);

        _mediator.PublishGameEvents(item);

        ThrowField(player, item, tile as IDynamicTile);

        return Unit.ValueTask;
    }

    private void ThrowField(ICreature usedBy, IFieldRune rune, IDynamicTile onTile)
    {
        if (Guard.AnyNull(usedBy, rune, onTile)) return;

        if (string.IsNullOrWhiteSpace(rune.Area))
        {
            var field = _itemFactory.Create(rune.Field, onTile.Location, null);
            onTile.AddItem(field);

            CauseDamageToCreaturesOnTile(field, onTile);
            return;
        }

        var template = _areaEffectStore.Get(rune.Area);
        foreach (var coordinate in AreaEffect.Create(onTile.Location, template))
        {
            var location = coordinate.Location;
            var field = _itemFactory.Create(rune.Field, location, null);

            if (_map[location] is not IDynamicTile tile) continue;

            tile.AddItem(field);

            CauseDamageToCreaturesOnTile(field, tile);
        }
    }

    private void CauseDamageToCreaturesOnTile(IItem item, ITile tile)
    {
        if (item is not IMagicField field) return;
        if (tile is not IDynamicTile onTile) return;

        if (!onTile.HasCreature) return;

        foreach (var creature in onTile.Creatures) field.CauseDamage(creature);
    }
}