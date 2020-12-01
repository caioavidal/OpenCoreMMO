namespace NeoServer.Game.Contracts.Items.Types
{
    public interface ICumulative : IPickupable
    {
        public byte Amount { get; set; }
        bool TryJoin(ref ICumulative item);
        float CalculateWeight(byte amount);
        ICumulative Clone(byte amount);
        void Reduce(byte amount);
        void Increase(byte amount);
        ICumulative Split(byte amount);

        new float Weight { get; }
        byte AmountToComplete { get; }
    }
}
