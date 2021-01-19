using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Creatures.Spells;

namespace NeoServer.Game.Contracts.Combat.Defenses
{
    public class IllusionCombatDefense : BaseCombatDefense
    {
        public ISpell Spell { get; }
        public IllusionCombatDefense(uint duration, string monsterName, EffectT effect, IMonsterDataManager dataManager) //todo: remove dataManager from here
        {
            Spell = new IllusionSpell(duration, monsterName, dataManager, effect);
        }
        public override void Defende(ICombatActor actor) => Spell?.Invoke(actor,null, out var error);
    }
}
