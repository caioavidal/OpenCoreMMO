using NeoServer.Game.Contracts.Creatures;
using System.Collections.Generic;

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
        public byte GainSoulTicks { get; set; }
        public string FromVoc { get; set; }
        public IVocationFormula Formula { get; set; }
        public Dictionary<byte, float> Skill { get; set; }
        public byte VocationType => byte.Parse(Id);
    }

}
