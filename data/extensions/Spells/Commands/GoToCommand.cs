using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Extensions.Spells.Commands;

public class GoToCommand : CommandSpell
{
    public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
    {
        error = InvalidOperation.NotPossible;
        if (Params?.Length == 0) return false;

        if (Params?.Length == 3)
        {
            ushort.TryParse(Params[0].ToString(), out var x);
            ushort.TryParse(Params[1].ToString(), out var y);
            byte.TryParse(Params[2].ToString(), out var z);

            actor.TeleportTo(x, y, z);
        }

        return true;
    }
}