namespace NeoServer.Game.Contracts.Items.Types
{
    public interface ICumulativeItem : IPickupable
    {
        public byte Amount { get; set; }
        bool TryJoin(ref ICumulativeItem item);
        float CalculateWeight(byte amount);
        ICumulativeItem Clone(byte amount);
        void Reduce(byte amount);

        new float Weight { get; }
        byte AmountToComplete { get; }
    }
}
