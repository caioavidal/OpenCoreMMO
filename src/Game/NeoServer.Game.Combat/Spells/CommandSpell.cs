using NeoServer.Game.Common.Contracts.Spells;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;

namespace NeoServer.Game.Combat.Spells
{
    public abstract class CommandSpell : Spell<CommandSpell>, ICommandSpell
    {
        public override uint Duration => 0;
        public override bool ShouldSay => false;
        public object[] Params { get; set; }
        public override EffectT Effect => EffectT.GlitterRed;
        public override ConditionType ConditionType => ConditionType.None;
    }
}