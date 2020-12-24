using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Loaders.Vocations
{

    public class VocationData
    {
        public List<Vocation> Vocations { get; set; }

        public class Formula
        {
            public string meleeDamage { get; set; }
            public string distDamage { get; set; }
            public string defense { get; set; }
            public string armor { get; set; }
        }

        public class Skill
        {
            public string id { get; set; }
            public string multiplier { get; set; }
        }

        public class Vocation
        {
            public string id { get; set; }
            public string clientid { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string gaincap { get; set; }
            public string gainhp { get; set; }
            public string gainmana { get; set; }
            public string gainhpticks { get; set; }
            public string gainhpamount { get; set; }
            public string gainmanaticks { get; set; }
            public string gainmanaamount { get; set; }
            public string manamultiplier { get; set; }
            public string attackspeed { get; set; }
            public string basespeed { get; set; }
            public string soulmax { get; set; }
            public string gainsoulticks { get; set; }
            public string fromvoc { get; set; }
            public Formula formula { get; set; }
            public List<Skill> skill { get; set; }
        }
    }

}
