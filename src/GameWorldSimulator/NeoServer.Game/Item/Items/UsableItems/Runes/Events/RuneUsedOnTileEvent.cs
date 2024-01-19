using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Contracts.World.Tiles;

namespace NeoServer.Game.Item.Items.UsableItems.Runes.Events;

public sealed record FieldRuneUsedOnTileEvent(IPlayer Player, IFieldRune Rune, IDynamicTile OnTile) : IGameEvent;