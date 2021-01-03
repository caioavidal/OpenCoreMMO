namespace NeoServer.Game.Contracts.Items.Types
{
    public interface IFood:IItem
    {
        public ushort Duration => Metadata.Attributes.GetAttribute<ushort>(Common.ItemAttribute.Duration);
    }
}
