using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Model.Monsters.Attacks
{
    public class MagicalAttack : CombatAttack
    {
        public byte Chance { get; set; }
        public short Min { get; set; }
        public short Max { get; set; }
    }
}
