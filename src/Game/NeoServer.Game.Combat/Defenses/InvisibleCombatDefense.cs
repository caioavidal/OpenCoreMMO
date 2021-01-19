using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Creatures.Spells;

namespace NeoServer.Game.Contracts.Combat.Defenses
{
    public class InvisibleCombatDefense : BaseCombatDefense
    {
        public ISpell Spell { get; }
        public InvisibleCombatDefense(uint duration, EffectT effect)
        {
            Spell = new InvisibleSpell(duration, effect);
        }
        public override void Defende(ICombatActor actor) => Spell?.Invoke(actor,null, out var error);
    }
}
