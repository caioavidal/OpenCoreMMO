using Mediator;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Modules.Chat.Channel.ExitChannel;

namespace NeoServer.Modules.Chat.Channel.ExitNpcChannel;

public record ExitNpcChannelCommand(IPlayer Player) : ICommand;

public class ExitNpcChannelCommandHandler(IMap map) : ICommandHandler<ExitChannelCommand>
{
    public ValueTask<Unit> Handle(ExitChannelCommand command, CancellationToken cancellationToken)
    {
        var player = command.Player;
        
        foreach (var creature in map.GetCreaturesAtPositionZone(player.Location))
        {
            if (creature is not INpc npc) continue;
            npc.StopTalkingToCustomer(player);
        }

        return Unit.ValueTask;
    }
}