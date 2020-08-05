using NeoServer.Game.Enums.Creatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Model.Monsters
{
    public abstract class CombatAttack
    {
        public string Name { get; set; }
        public ushort Interval { get; set; }
        //public Tuple<SkillType, byte> Skill { get; set; }
        //public byte Chance { get; set; }
        //public byte Range { get; set; }
        //public byte Radius { get; set; }
        //public byte Target { get; set; }
        //public Tuple<short,short> MinMax { get; set; }

        //public IDictionary<string,string> Attributes { get; set; }


    }
}
