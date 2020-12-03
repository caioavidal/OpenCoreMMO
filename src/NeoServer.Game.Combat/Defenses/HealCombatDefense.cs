using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Combat.Defenses
{
    public class HealCombatDefense : BaseCombatDefense
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public override void Defende(ICombatActor actor)
        {
            var hpToIncrease = ServerRandom.Random.NextInRange(Min, Max);
            actor.Heal((ushort)hpToIncrease);
        }
    }
}
