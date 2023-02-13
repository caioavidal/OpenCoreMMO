using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;

namespace NeoServer.Game.Items.Factories.AttributeFactory;

public class SkillBonusFactory : IFactory
{
    public event CreateItem OnItemCreated;

    public ISkillBonus Create(IItemType itemType)
    {
        //if (itemType.Attributes.SkillBonuses is not { } skillBonuses) return null;
        //if (!skillBonuses.Any()) return null;

        //return new SkillBonus();
        return null;
    }
}