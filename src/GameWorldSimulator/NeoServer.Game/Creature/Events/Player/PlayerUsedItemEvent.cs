using Mediator;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Creature.Events.Player;

public record PlayerUsedItemEvent(IPlayer Player, IItem Item, IThing Target) : IGameEvent;