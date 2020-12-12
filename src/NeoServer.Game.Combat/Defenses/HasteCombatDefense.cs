using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Spells;

namespace NeoServer.Game.Contracts.Combat.Defenses
{
    public class HasteCombatDefense : BaseCombatDefense
    {
        public ushort SpeedBoost { get; set; }
        public uint Duration { get; set; }
        public override void Defende(ICombatActor actor) => HasteSpell.Instance.Invoke(actor, out var error);
    }
}