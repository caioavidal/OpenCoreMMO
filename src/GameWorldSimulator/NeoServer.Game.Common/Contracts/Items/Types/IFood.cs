using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Types;

public interface IFood : IItem
{
    public ushort Duration => Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.Duration);
}