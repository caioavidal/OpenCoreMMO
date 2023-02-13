using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Helpers;

namespace NeoServer.Extensions.Spells.Commands;

public class PullPlayerCommand : CommandSpell
{
    public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
    {
        error = InvalidOperation.NotEnoughRoom;
        if (Params?.Length == 0) return false;

        var gameManager = IoC.GetInstance<IGameCreatureManager>();

        if (!gameManager.TryGetPlayer(Params[0].ToString(), out var player))
        {
            error = InvalidOperation.PlayerNotFound;
            return false;
        }

        var newLocation = actor.Location.GetNextLocation(actor.Direction);

        player.TeleportTo(newLocation);
        return true;
    }
}