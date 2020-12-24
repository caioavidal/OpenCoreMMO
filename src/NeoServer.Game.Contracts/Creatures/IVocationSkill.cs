using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface IVocationSkill
    {
        string Id { get; set; }
        string Multiplier { get; set; }
    }

}
