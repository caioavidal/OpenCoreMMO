using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoServer.Game.Common.Creatures;

namespace NeoServer.Game.Common.Contracts.Items.Types
{
    public interface ISkillBonus
    {
        Dictionary<SkillType, byte> SkillBonus { get; }
    }
}
