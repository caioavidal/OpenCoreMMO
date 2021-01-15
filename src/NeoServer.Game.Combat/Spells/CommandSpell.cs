using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Contracts.Spells;

namespace NeoServer.Game.Combat.Spells
{
    public abstract class CommandSpell : Spell<CommandSpell>, ICommandSpell
    {
        public override bool ShouldSay => false;
        public object[] Params { get; set; }
        public override EffectT Effect => EffectT.GlitterRed;
        public override uint Duration => 0;
        public override ConditionType ConditionType => ConditionType.None;
    }
}
