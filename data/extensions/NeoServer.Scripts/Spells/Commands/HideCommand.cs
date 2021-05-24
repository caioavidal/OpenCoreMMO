using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Scripts.Spells.Commands
{
    public class HideCommand : CommandSpell
    {
        public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
        {
            error = InvalidOperation.NotEnoughRoom;
            actor.TurnInvisible();
            return true;
        }
    }
}
