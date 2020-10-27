using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures;
using NeoServer.Game.Creatures.Model.Conditions;
using NeoServer.Game.Creatures.Spells;
using NeoServer.Server.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Combat.Defenses
{
    public class HasteCombatDefence: BaseCombatDefense
    {
        public ushort SpeedBoost { get; set; }
        public uint Duration { get; set; }

        public override void Defende(ICreature actor) => HasteSpell.Instance.Invoke(actor, SpeedBoost, Duration);
    }
}
