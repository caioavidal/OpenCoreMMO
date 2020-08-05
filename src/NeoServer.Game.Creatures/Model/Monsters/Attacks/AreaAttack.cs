using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Model.Monsters.Attacks
{
    public class AreaAttack: MagicalAttack
    {
        public byte Radius { get; set; }
        public byte Length { get; set; }
        public byte Spread { get; set; }
        public byte Target { get; set; }
    }
}
