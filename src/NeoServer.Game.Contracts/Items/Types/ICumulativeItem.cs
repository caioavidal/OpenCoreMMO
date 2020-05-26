namespace NeoServer.Game.Contracts.Items.Types
{
    public interface ICumulativeItem : IPickupable
    {
        public byte Amount { get; set; }
        ICumulativeItem Split(byte amount);
        bool TryJoin(ref ICumulativeItem item);

        new float Weight { get; }
    }
}
