using Mediator;
using NeoServer.Application.Features.Combat.Attacks.MeleeAttack;
using NeoServer.Game.Combat;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Results;

namespace NeoServer.Application.Features.Combat.PlayerAttack.WeaponAttack;

public record PlayerWeaponAttackCommand(IPlayer Player, IThing Enemy, AttackParameter AttackParameter)
    : ICommand<Result>;

public class PlayerWeaponAttackCommandHandler(MeleeAttackStrategy meleeAttackStrategy)
    : ICommandHandler<PlayerWeaponAttackCommand, Result>
{
    private readonly MeleeAttackStrategy _meleeAttackStrategy = meleeAttackStrategy;

    public ValueTask<Result> Handle(PlayerWeaponAttackCommand command, CancellationToken cancellationToken)
    {
        meleeAttackStrategy.Execute(new AttackInput(command.Player, command.Enemy)
        {
            Attack = command.AttackParameter
        });
        return new ValueTask<Result>();
    }
}