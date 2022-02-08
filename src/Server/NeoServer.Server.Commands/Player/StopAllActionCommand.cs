using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Commands;

namespace NeoServer.Server.Commands.Player;

public class StopAllActionCommand : ICommand
{
    private readonly IGameServer game;

    public StopAllActionCommand(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(IPlayer player)
    {
        if (player.Attacking)
        {
            player.StopAttack();
            player.StopWalking();
        }

        if (player.IsFollowing) 
        {
            player.StopFollowing();
            player.StopWalking();
        }
    }
}