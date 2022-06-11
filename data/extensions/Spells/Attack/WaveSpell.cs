using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Server.Helpers;

namespace NeoServer.Extensions.Spells.Attack
{
    public abstract class WaveSpell : AttackSpell
    {
        protected abstract string AreaName { get; }

        public override CombatAttack CombatAttack => _areaCombatAttack;
        
        private AreaCombatAttack _areaCombatAttack;

        public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
        {
            var effectStore = Fabric.Return<IAreaEffectStore>();
            var area = effectStore.Get(AreaName, actor.Direction);

            _areaCombatAttack = new AreaCombatAttack(area);

            return base.OnCast(actor, words, out error);
        }
    }
}