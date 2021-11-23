using System;
using NeoServer.Game.Common.Contracts.Items.Types;

namespace NeoServer.Game.Common.Contracts.Items
{
    public interface IEquipment: IDecayable, ISkillBonus, IDressable, IProtection, ITransformableEquipment, IChargeable,
        IEquipmentRequirement
    {
        event Action<IEquipment> OnDressed;
        event Action<IEquipment> OnUndressed;
    }
}
