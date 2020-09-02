using NeoServer.Game.Contracts.Combat;

namespace NeoServer.Game.Creatures.Model.Monsters
{

    public class CombatAttack : ICombatAttack
    {
        public string Name { get; set; }
        public ushort Interval { get; set; }
        public ushort Skill { get; set; }
        public ushort Attack { get; set; }
        //public Tuple<SkillType, byte> Skill { get; set; }
        //public byte Chance { get; set; }
        //public byte Range { get; set; }
        //public byte Radius { get; set; }
        //public byte Target { get; set; }
        //public Tuple<short,short> MinMax { get; set; }

        //public IDictionary<string,string> Attributes { get; set; }

    }
}
