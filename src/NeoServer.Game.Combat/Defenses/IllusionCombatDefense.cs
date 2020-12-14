using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Creatures.Spells;

namespace NeoServer.Game.Contracts.Combat.Defenses
{
    public class IllusionCombatDefense : BaseCombatDefense
    {
        public uint Duration { get; }
        public ISpell Spell { get; }
        public IllusionCombatDefense(uint duration, string monsterName, IMonsterDataManager dataManager)
        {
            Spell = new IllusionSpell(duration, monsterName, dataManager, Effect);
        }
        public override void Defende(ICombatActor actor) => Spell.Invoke(actor, out var error);
    }
}
