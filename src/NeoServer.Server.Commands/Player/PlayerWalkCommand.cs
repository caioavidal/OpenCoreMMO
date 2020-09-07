using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums.Location;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Commands.Player
{

    public class PlayerWalkCommand //: Command
    {
        private readonly Game game;
        private IPlayer player;
        private readonly Direction[] directions;

        public PlayerWalkCommand(IPlayer player, Game game, params Direction[] directions)
        {
            this.player = player;
            this.game = game;
            this.directions = directions;
        }

        public static void Execute(IPlayer player, Game game, params Direction[] directions)
        {
            if (player.NextStepId != 0)
            {
                game.Scheduler.CancelEvent(player.NextStepId);
                player.NextStepId = 0;
            }

            //if(game.Map.GetNextTile(player.Location, direction);

            player.StopWalking();

            player.TryWalkTo(directions);

            AddEventWalk(player, game, directions.Length == 1);
        }

        private static void AddEventWalk(IPlayer player, Game game, bool firstStep)
        {
            var creature = player as ICreature;
            creature.CancelNextWalk = false;

            if (creature.EventWalk != 0)
            {
                return;
            }

            if (firstStep)
            {
                MovePlayer(player, game);
            }

            creature.EventWalk = game.Scheduler.AddEvent(new SchedulerEvent(player.StepDelayMilliseconds, () => MovePlayer(player, game)));
        }

        private static void MovePlayer(IPlayer player, Game game)
        {
            var creature = player as ICreature;
            var thing = creature as IMoveableThing;

            if (creature.TryGetNextStep(out var direction))
            {
                var toTile = game.Map.GetNextTile(thing.Location, direction);
                if (!game.Map.TryMoveThing(ref thing, toTile.Location))
                {
                    player.CancelWalk();
                }
            }
            else
            {
                if (player.EventWalk != 0)
                {
                    game.Scheduler.CancelEvent(player.EventWalk);
                    player.EventWalk = 0;
                }
            }

            if (player.EventWalk != 0)
            {
                player.EventWalk = 0;
                AddEventWalk(player, game, false);
            }

            if (creature.IsRemoved)
            {
                creature.StopWalking();
                return;
            }
        }
    }
}
