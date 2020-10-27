using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Combat.Defenses
{
    public abstract class BaseCombatDefense:ICombatDefense
    {
        /// <summary>
        /// Minumum interval to occurr
        /// </summary>
        public ushort Interval { get; set; }
        /// <summary>
        /// Chance to occurr - 0 to 100
        /// </summary>
        public byte Chance { get; set; }
        public EffectT Effect { get; set; }

        /// <summary>
        /// action to execute when defence occurrs
        /// </summary>
        /// <param name="actor"></param>
        public abstract void Defende(ICreature actor);
    }
}
