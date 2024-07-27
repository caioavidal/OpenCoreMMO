using NeoServer.Application.Common;
using NeoServer.Application.Common.Contracts;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Services;

namespace NeoServer.Extensions.Spells.Commands;

public class GoToCommand : CommandSpell
{
    public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
    {
        error = InvalidOperation.NotPossible;
        if (!HasAnyParameter) return false;

        var actorPlayer = (IPlayer)actor;

        // just only GOD can teleport to other players
        if (Params.Length == 1 && actorPlayer.VocationType == 11)
        {
            var creatureManager = IoC.GetInstance<IGameCreatureManager>();
            var input = Params[0].ToString();
            creatureManager.TryGetPlayer(input, out var target);

            if (target == null || target.CreatureId == actorPlayer.CreatureId)
            {
                error = InvalidOperation.None;
                OperationFailService.Send(actorPlayer, "Player or npc not found.");
                return false;
            }

            actorPlayer.TeleportTo(target.Location);
            return true;
        }

        if (Params?.Length != 3) return false;

        ushort.TryParse(Params[0].ToString(), out var x);
        ushort.TryParse(Params[1].ToString(), out var y);
        byte.TryParse(Params[2].ToString(), out var z);

        actor.TeleportTo(x, y, z);

        return true;
    }
}