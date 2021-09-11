using NeoServer.Game.Common.Contracts.Items.Types;

namespace NeoServer.Game.Common.Contracts.Items
{
    public interface IEquipment: IDecayable, ISkillBonus, IDressable, IProtection, ITransformable
    {
    }
}
