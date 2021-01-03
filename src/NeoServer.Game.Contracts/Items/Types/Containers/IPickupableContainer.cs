namespace NeoServer.Game.Contracts.Items.Types.Containers
{
    public interface IPickupableContainer : IContainer, IPickupable
    {
        new float Weight { get; set; }
    }
}
