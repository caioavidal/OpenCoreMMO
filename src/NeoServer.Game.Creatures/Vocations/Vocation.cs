using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures.Vocations
{
   
    public class Vocation : IVocation
    {
        public string Id { get; set; }
        public string Clientid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ushort GainCap { get; set; }
        public ushort GainHp { get; set; }
        public ushort GainMana { get; set; }
        public byte GainHpTicks { get; set; }
        public byte GainManaTicks { get; set; }
        public ushort GainHpAmount { get; set; }
        public ushort GainManaAmount { get; set; }
        public string ManaMultiplier { get; set; }
        public string AttackSpeed { get; set; }
        public ushort BaseSpeed { get; set; }
        public byte SoulMax { get; set; }
        public string GainSoulTicks { get; set; }
        public string FromVoc { get; set; }
        public IVocationFormula Formula { get; set; }
        public List<IVocationSkill> Skill { get; set; }
        public VocationType VocationType => VocationTypeParser.Parse(Name);
    }

}
