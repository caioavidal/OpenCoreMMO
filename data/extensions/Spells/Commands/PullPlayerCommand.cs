using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Location;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Extensions.Spells.Commands
{
    public class PullPlayerCommand : CommandSpell
    {
        private readonly IGameCreatureManager gameCreatureManager;

        public PullPlayerCommand(IGameCreatureManager gameCreatureManager)
        {
            this.gameCreatureManager = gameCreatureManager;
        }
        public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
        {
            error = InvalidOperation.NotEnoughRoom;
            if (Params?.Length == 0) return false;

            gameCreatureManager.TryGetPlayer(Params[0].ToString(), out IPlayer player);

            if (player == null)
            {
                error = InvalidOperation.PlayerNotFound;
                return false;
            }

            var location = actor.Location;

            switch (actor.Direction)
            {
                case Direction.North:
                    location.Update(location.X, (ushort)(location.Y - 1), location.Z);
                    break;
                case Direction.East:
                    location.Update((ushort)(location.X + 1), location.Y, location.Z);
                    break;
                case Direction.South:
                    location.Update(location.X, (ushort)(location.Y + 1), location.Z);
                    break;
                case Direction.West:
                    location.Update((ushort)(location.X - 1), location.Y, location.Z);
                    break;
                case Direction.None:
                default:
                    return false;
            }

            player.TeleportTo(location);
            return true;
        }
    }
}
