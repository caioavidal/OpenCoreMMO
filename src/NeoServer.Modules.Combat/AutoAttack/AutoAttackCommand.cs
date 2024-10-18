// using Mediator;
// using NeoServer.Application.Features.Combat.Attacks;
// using NeoServer.Application.Features.Combat.PlayerAttack;
// using NeoServer.Game.Combat;
// using NeoServer.Game.Common;
// using NeoServer.Game.Common.Contracts.Creatures;
// using NeoServer.Game.Common.Results;
//
// namespace NeoServer.Application.Features.Combat.AutoAttack;
// public sealed record AutoAttackCommand(ICombatActor Aggressor, ICombatActor Victim) : ICommand<Result>;
// public class AutoAttackCommandHandler(AttackLibrary attackLibrary, PlayerAttackBuilder playerAttackBuilder, GameConfiguration gameConfiguration) : ICommandHandler<AutoAttackCommand, Result>
// {
//     public ValueTask<Result> Handle(AutoAttackCommand command, CancellationToken cancellationToken)
//     {
//         command.Deconstruct(out var aggressor, out var victim);
//
//         if (aggressor is IMonster) return new ValueTask<Result>(Result.Success);
//
//         var attackParameters = playerAttackBuilder.Build(aggressor as IPlayer);
//         
//         var attackResult = Result.NotPossible;
//
//         var attackCount = 0;
//         
//         foreach (var attackParameter in attackParameters)
//         {
//            var attackStrategy = attackLibrary.Get(attackParameter.Name);
//            if (attackStrategy is null) continue;
//
//            var attackInput = new AttackInput(aggressor, victim)
//            {
//                Attack = attackParameter
//            };
//            
//            attackResult = attackStrategy.Execute(attackInput);
//            if (attackResult.Failed) break;
//
//            if (gameConfiguration.Combat.MaxAttacksPerTurn == attackCount++) break;
//         }
//         
//         return new ValueTask<Result>(attackResult);
//     }
// }

