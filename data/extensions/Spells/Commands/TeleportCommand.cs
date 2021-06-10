using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Location;

namespace NeoServer.Extensions.Spells.Commands
{
    public class TeleportCommand : CommandSpell
    {
        public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
        {
            error = InvalidOperation.NotEnoughRoom;

            var steps = (Params?.Length ?? 0) == 0 ? 1 : int.Parse(Params[0].ToString());

            var location = actor.Location;
            switch (actor.Direction)
            {
                case Direction.North:
                    location.Update(location.X, (ushort) (location.Y - steps), location.Z);
                    break;
                case Direction.East:
                    location.Update((ushort) (location.X + steps), location.Y, location.Z);
                    break;
                case Direction.South:
                    location.Update(location.X, (ushort) (location.Y + steps), location.Z);
                    break;
                case Direction.West:
                    location.Update((ushort) (location.X - steps), location.Y, location.Z);
                    break;
                case Direction.None:
                    return false;
                default:
                    return false;
            }

            actor.TeleportTo(location);
            return true;
        }
    }
}