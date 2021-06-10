using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Types.Useables
{
    public delegate void Use(ICreature usedBy, ICreature creature, IItem item);

    public interface IConsumable : IItemRequirement, IUseableOnCreature, IItem
    {
        public string Sentence => Metadata.Attributes.GetAttribute(ItemAttribute.Sentence);
        public event Use OnUsed;
    }
}