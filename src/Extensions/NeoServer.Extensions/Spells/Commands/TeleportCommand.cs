using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Services;
using NeoServer.Game.Common.Texts;
using NeoServer.Game.World.Map;

namespace NeoServer.Extensions.Spells.Commands;

public class TeleportCommand : CommandSpell
{
    public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
    {
        error = InvalidOperation.NotEnoughRoom;

        var steps = (Params?.Length ?? 0) == 0 ? 1 : int.Parse(Params[0].ToString());
        var newLocation = actor.Location.GetNextLocation(actor.Direction, (ushort)steps);

        if (Map.Instance[newLocation] is not IDynamicTile)
        {
            OperationFailService.Send(actor.CreatureId, TextConstants.NOT_ENOUGH_ROOM);
            return false;
        }

        actor.TeleportTo(newLocation);
        return true;
    }
}