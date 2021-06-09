using NeoServer.Game.Common;

namespace NeoServer.Game.Contracts.Items.Types
{
    public interface IFood : IItem
    {
        public ushort Duration => Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.Duration);
    }
}