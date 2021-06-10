using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Types.Useables
{
    public interface IUseableOn : IItem
    {
        public EffectT Effect => Metadata.Attributes.GetEffect();

        public int CooldownTime => Metadata.Attributes.HasAttribute(ItemAttribute.CooldownTime)
            ? Metadata.Attributes.GetAttribute<int>(ItemAttribute.CooldownTime)
            : 1000;
    }
}