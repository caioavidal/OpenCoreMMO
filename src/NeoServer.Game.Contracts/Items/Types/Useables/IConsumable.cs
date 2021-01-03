using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items.Types.Useables;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Contracts.Items.Types
{
    public delegate void Use(ICreature usedBy, ICreature creature, IItem item);
    public interface IConsumable: IItemRequirement, IUseableOnCreature, IItem
    {
        public event Use OnUsed;
        public string Sentence => Metadata.Attributes.GetAttribute(ItemAttribute.Sentence);
    }
}
