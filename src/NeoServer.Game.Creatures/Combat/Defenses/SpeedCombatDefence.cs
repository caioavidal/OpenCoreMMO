using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Combat.Defenses
{
    public class SpeedCombatDefence: BaseCombatDefense
    {
        public ushort SpeedIncrease { get; set; }
        public uint Duration { get; set; }
        public EffectT Effect { get; set; }

        public override void Defende(ICreature actor)
        {
            actor.IncreaseSpeed(SpeedIncrease);
        }
    }
}
