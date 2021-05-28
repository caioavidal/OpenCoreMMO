using NeoServer.Enums.Creatures.Enums;

namespace NeoServer.Game.Contracts.Items.Types.Useables
{
    public interface IUseableOn : IItem
    {
        public EffectT Effect => Metadata.Attributes.GetEffect();
        public int CooldownTime => Metadata.Attributes.HasAttribute(Common.ItemAttribute.CooldownTime) ? Metadata.Attributes.GetAttribute<int>(Common.ItemAttribute.CooldownTime) : 1000;
    }
}
