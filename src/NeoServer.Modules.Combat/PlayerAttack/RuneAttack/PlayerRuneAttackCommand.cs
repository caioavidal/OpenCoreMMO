using Mediator;
using NeoServer.Game.Combat;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Services;
using NeoServer.Modules.Combat.Attacks.DistanceAttack;

namespace NeoServer.Modules.Combat.PlayerAttack.RuneAttack;

public record PlayerRuneAttackCommand(IPlayer Player, IThing Victim, IAttackRune Rune, AttackParameter AttackParameter)
    : ICommand;

public class PlayerRuneAttackCommandHandler(
    DistanceAttackStrategy distanceAttackStrategy,
    AttackRuneCooldownManager cooldownManager,
    GameConfiguration gameConfiguration,
    IMap map)
    : ICommandHandler<PlayerRuneAttackCommand>
{
    public ValueTask<Unit> Handle(PlayerRuneAttackCommand command, CancellationToken cancellationToken)
    {
        var (player, target, rune, attackParameter) = command;

        if (target is null) return Unit.ValueTask;

        if (target is not ICreature)
        {
            var tile = map[target.Location];
            var creature = tile?.TopCreatureOnStack;

            if (creature is null && attackParameter.NeedTarget)
            {
                OperationFailService.Send(player, InvalidOperation.CanOnlyUseRuneOnCreature, EffectT.Puff);
                return Unit.ValueTask;
            }

            if (attackParameter.NeedTarget) target = creature;
        }

        if (!cooldownManager.Expired(player))
        {
            player.RaiseExhaustionEvent();
            return Unit.ValueTask;
        }

        var runeCanBeUsed = command.Rune.CanBeUsed(command.Player);
        if (runeCanBeUsed.Failed)
        {
            OperationFailService.Send(player, runeCanBeUsed.Error, EffectT.Puff);
            return Unit.ValueTask;
        }

        var result = distanceAttackStrategy.Execute(new AttackInput(player, target)
        {
            Parameters = attackParameter
        });

        if (!result.Succeeded) return Unit.ValueTask;

        if (!gameConfiguration.InfiniteRune) command.Rune.Reduce();

        cooldownManager.Start(player, attackParameter.Cooldown);

        return Unit.ValueTask;
    }
}