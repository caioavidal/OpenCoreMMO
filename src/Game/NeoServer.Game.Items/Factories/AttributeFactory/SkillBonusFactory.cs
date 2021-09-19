using System.Linq;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Items.Items.Attributes;

namespace NeoServer.Game.Items.Factories.AttributeFactory
{
    public class SkillBonusFactory : IFactory
    {
        public SkillBonusFactory()
        {
        }

        public event CreateItem OnItemCreated;

        public ISkillBonus Create(IItemType itemType)
        {
            //if (itemType.Attributes.SkillBonuses is not { } skillBonuses) return null;
            //if (!skillBonuses.Any()) return null;

            //return new SkillBonus();
            return null;
        }
    }
}