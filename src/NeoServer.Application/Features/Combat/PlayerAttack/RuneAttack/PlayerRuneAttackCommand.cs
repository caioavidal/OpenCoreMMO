using Mediator;
using NeoServer.Application.Features.Combat.Attacks.DistanceAttack;
using NeoServer.Game.Combat;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Item.Items.UsableItems.Runes;

namespace NeoServer.Application.Features.Combat.PlayerAttack.RuneAttack;

public record PlayerRuneAttackCommand(IPlayer Player, IThing Victim, IAttackRune Rune, AttackParameter AttackParameter)
    : ICommand;

public class PlayerRuneAttackCommandHandler(DistanceAttackStrategy distanceAttackStrategy,
    AttackRuneCooldownManager cooldownManager,
    GameConfiguration gameConfiguration)
    : ICommandHandler<PlayerRuneAttackCommand>
{
    public ValueTask<Unit> Handle(PlayerRuneAttackCommand command, CancellationToken cancellationToken)
    {
        var (player, victim, rune, attackParameter) = command;
        
        if (!cooldownManager.Expired(player))
        {
            player.RaiseExhaustionEvent();
            return Unit.ValueTask;
        }
        
        if (!command.Rune.CanBeUsed(command.Player)) return Unit.ValueTask;
        
        var result = distanceAttackStrategy.Execute(new AttackInput(command.Player, command.Victim)
        {
            Parameters = command.AttackParameter
        });

        if (!result.Succeeded) return Unit.ValueTask;
        
        if(!gameConfiguration.InfiniteRune) command.Rune.Reduce(1);

        cooldownManager.Start(player, rune);

        return Unit.ValueTask;
    }
}