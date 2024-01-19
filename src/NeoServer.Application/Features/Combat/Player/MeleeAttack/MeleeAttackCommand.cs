using Mediator;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Application.Features.Combat.Player.MeleeAttack;

public sealed record MeleeAttackCommand
    (IPlayer Player) : ICommand;

public class MeleeAttackCommandHandler : ICommandHandler<MeleeAttackCommand>
{
    public ValueTask<Unit> Handle(MeleeAttackCommand command, CancellationToken cancellationToken)
    {
        
        return Unit.ValueTask;
    }
}
