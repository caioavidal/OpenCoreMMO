using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface ISkillModel
    {
        int Level { get; set; }
        int Count { get; set; }
    }
}
