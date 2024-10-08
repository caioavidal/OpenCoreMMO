using Mediator;
using NeoServer.Application.Features.Combat.Attacks.MeleeAttack;
using NeoServer.Application.Features.Combat.PlayerDefense;
using NeoServer.Game.Combat;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Results;

namespace NeoServer.Application.Features.Combat.PlayerAttack.WeaponAttack;

public record PlayerWeaponAttackCommand(IPlayer Player, IThing Victim, AttackParameter AttackParameter)
    : ICommand<Result>;

public class PlayerWeaponAttackCommandHandler(MeleeAttackStrategy meleeAttackStrategy)
    : ICommandHandler<PlayerWeaponAttackCommand, Result>
{
    public ValueTask<Result> Handle(PlayerWeaponAttackCommand command, CancellationToken cancellationToken)
    {
        meleeAttackStrategy.Execute(new AttackInput(command.Player, command.Victim)
        {
            Attack = command.AttackParameter
        });
        
        return new ValueTask<Result>();
    }
}