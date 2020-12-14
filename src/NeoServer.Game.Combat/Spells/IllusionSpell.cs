using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using System;
using System.Collections.Generic;
using System.Text;
using NeoServer.Game.Common.Creatures;

namespace NeoServer.Game.Creatures.Spells
{
    public class IllusionSpell : Spell<IllusionSpell>
    {
        public override string Name => "Illusion";
        public override EffectT Effect { get; } = EffectT.GlitterGreen;
        public override uint Duration { get; } = 5000;
        public override ushort Mana => 100;
        public override ConditionType ConditionType => ConditionType.Illusion;
        public virtual IMonsterDataManager Monsters { get; }
        public virtual string CreatureName { get; }
      
        public IllusionSpell(uint duration, string creatureName, IMonsterDataManager monsters, EffectT effect)
        {
            Monsters = monsters;
            Duration = duration;
            Effect = effect;
            CreatureName = creatureName;
        }

        public override void OnCast(ICombatActor actor)
        {
            if (!Monsters.TryGetMonster(CreatureName, out var monster)) return;

            var look = monster.Look;

            look.TryGetValue(LookType.Type, out var lookType);
            look.TryGetValue(LookType.Addon, out var addon);
            look.TryGetValue(LookType.Body, out var body);
            look.TryGetValue(LookType.Feet, out var feet);
            look.TryGetValue(LookType.Legs, out var legs);
            look.TryGetValue(LookType.Head, out var head);

            actor.SetTemporaryOutfit(lookType, 0, (byte)head, (byte)body, (byte)legs, (byte)feet, (byte)addon);
        }
        public override void OnEnd(ICombatActor actor)
        {
            actor.DisableTemporaryOutfit();
            base.OnEnd(actor);
        }
    }
}
