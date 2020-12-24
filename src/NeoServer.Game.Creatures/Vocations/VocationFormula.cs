using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures.Vocations
{
   
    public class VocationFormula : IVocationFormula
    {
        public string MeleeDamage { get; set; }
        public string DistDamage { get; set; }
        public string Defense { get; set; }
        public string Armor { get; set; }
    }
}
