using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types;

namespace NeoServer.Game.Common.Contracts.Items
{
    public interface IEquipment: IDecayable, ISkillBonus, IDressable, IProtection, ITransformable
    {
    }
}
