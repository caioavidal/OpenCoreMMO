using Mediator;
using NeoServer.Application.Features.Combat.Attacks.DistanceAttack;
using NeoServer.Application.Features.Combat.Attacks.MeleeAttack;
using NeoServer.Application.Features.Combat.PlayerDefense;
using NeoServer.Game.Combat;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Results;

namespace NeoServer.Application.Features.Combat.PlayerAttack.WeaponAttack;

public record PlayerWeaponAttackCommand(IPlayer Player, IThing Victim, AttackParameter AttackParameter)
    : ICommand<Result>;

public class PlayerWeaponAttackCommandHandler(MeleeAttackStrategy meleeAttackStrategy, DistanceAttackStrategy distanceAttackStrategy)
    : ICommandHandler<PlayerWeaponAttackCommand, Result>
{
    public ValueTask<Result> Handle(PlayerWeaponAttackCommand command, CancellationToken cancellationToken)
    {
        if (command.Player.UsingDistanceWeapon)
        {
            distanceAttackStrategy.Execute(new AttackInput(command.Player, command.Victim)
            {
                Parameters = command.AttackParameter
            });
        }
        
        meleeAttackStrategy.Execute(new AttackInput(command.Player, command.Victim)
        {
            Parameters = command.AttackParameter
        });

        return new ValueTask<Result>();
    }
}