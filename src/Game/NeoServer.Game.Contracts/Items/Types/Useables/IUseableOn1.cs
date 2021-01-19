using NeoServer.Enums.Creatures.Enums;

namespace NeoServer.Game.Contracts.Items.Types.Useables
{
    public interface IUseableOn: IItem
    {
        public EffectT Effect => Metadata.Attributes.GetEffect();

    }
}
