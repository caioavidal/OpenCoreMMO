using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Creatures.Spells;

namespace NeoServer.Game.Contracts.Combat.Defenses
{
    public class HealCombatDefense : BaseCombatDefense
    {
        public ISpell Spell { get; }
        public HealCombatDefense(int min, int max, EffectT effect) //todo: remove dataManager from here
        {
            Spell = new HealSpell(new MinMax(min, max), effect);
        }
        public override void Defende(ICombatActor actor) => Spell?.Invoke(actor,null, out var error);
    }
}