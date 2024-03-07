using Mediator;
using NeoServer.Application.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;

namespace NeoServer.Application.Features.Trade.RequestTrade;

public record TradeRequestCommand(IPlayer Player, IItem Item, IPlayer SecondPlayer) : ICommand;

public class TradeRequestCommandHandler : ICommandHandler<TradeRequestCommand>
{
    private readonly SafeTradeSystem _tradeSystem;

    public TradeRequestCommandHandler(SafeTradeSystem tradeSystem, IMap map, IGameCreatureManager creatureManager)
    {
        _tradeSystem = tradeSystem;
    }

    public ValueTask<Unit> Handle(TradeRequestCommand command, CancellationToken cancellationToken)
    {
        command.Deconstruct(out var player, out var item, out var secondPlayer);

        _tradeSystem.Request(player, secondPlayer, item);
        return Unit.ValueTask;
    }
}