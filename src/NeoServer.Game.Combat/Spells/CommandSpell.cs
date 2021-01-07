using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Combat.Spells
{
    public abstract class CommandSpell : Spell<CommandSpell>, ICommandSpell
    {
        public override bool ShouldSay => false;
        public object[] Params { get; set; }
        public override EffectT Effect => EffectT.GlitterBlue;
        public override uint Duration => 0;
        public override ConditionType ConditionType => ConditionType.None;

    }
}
