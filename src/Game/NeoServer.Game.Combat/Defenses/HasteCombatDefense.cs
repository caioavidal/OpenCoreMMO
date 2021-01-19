using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Spells;

namespace NeoServer.Game.Contracts.Combat.Defenses
{
    public class HasteCombatDefense : BaseCombatDefense
    {
        public ISpell Spell { get; }
        public HasteCombatDefense(uint duration, ushort speedBoost, EffectT effect)
        {
            Spell = new HasteSpell(duration, speedBoost, effect);
        }
        public override void Defende(ICombatActor actor) => Spell?.Invoke(actor,null, out var error);
    }
}