using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Types.Body
{
    public interface INecklace : IChargeable, IDurable, IBodyEquipmentItem, IProtectionItem, ISkillBonus
    {
        bool Expired { get; }
        byte Defense { get; }
    }
}