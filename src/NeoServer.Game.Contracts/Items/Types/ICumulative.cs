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
        bool HasMany => Amount > 1;
        new float Weight { get; }
        byte AmountToComplete { get; }
        string IThing.InspectionText => HasMany ? $"{Amount} {Plural ?? $"{Name}s"}" : $"{Metadata.Article} {Name}";
        string Subject => HasMany ? "They weigh" : "It weighs";
        string IThing.CloseInspectionText => $"{InspectionText}.\n{Subject} {Weight} oz";

    }
}
