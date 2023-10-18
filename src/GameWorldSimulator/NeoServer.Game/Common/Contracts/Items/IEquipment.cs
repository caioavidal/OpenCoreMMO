using System;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types;

namespace NeoServer.Game.Common.Contracts.Items;

public interface IEquipment : IDecay, ISkillBonus, IDressable, IProtection, ITransformableEquipment, IChargeable,
    IHasDecay,
    IEquipmentRequirement
{
    IPlayer PlayerDressing { get; }
    event Action<IEquipment> OnDressed;
    event Action<IEquipment> OnUndressed;
}