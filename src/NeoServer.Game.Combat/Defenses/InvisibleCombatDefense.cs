using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Creatures.Spells;

namespace NeoServer.Game.Contracts.Combat.Defenses
{
    public class InvisibleCombatDefense : BaseCombatDefense
    {
        public uint Duration { get; }
        public ISpell Spell { get; }
        public InvisibleCombatDefense(uint duration)
        {
            Spell = new InvisibleSpell(duration, Effect);
        }
        public override void Defende(ICombatActor actor) => Spell.Invoke(actor, out var error);
    }
}
