using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures.Vocations
{
  
    public class VocationSkill : IVocationSkill
    {
        public string Id { get; set; }
        public string Multiplier { get; set; }
    }
}
