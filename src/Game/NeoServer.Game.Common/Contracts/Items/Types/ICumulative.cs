namespace NeoServer.Game.Contracts.Items.Types
{
    public delegate void ItemReduce(ICumulative item, byte amount);
    public interface ICumulative : IPickupable
    {
        public byte Amount { get; set; }

        event ItemReduce OnReduced;

        bool TryJoin(ref ICumulative item);
        float CalculateWeight(byte amount);
        ICumulative Clone(byte amount);
        void Increase(byte amount);
        ICumulative Split(byte amount);
        void ClearSubscribers();

        bool HasMany => Amount > 1;
        new float Weight { get; }
        byte AmountToComplete { get; }
        string IItem.LookText => HasMany ? $"{Amount} {Plural ?? $"{Name}s"}" : $"{Metadata.Article} {Name}";
        string IThing.InspectionText => LookText;
        string Subject => HasMany ? "They weigh" : "It weighs";
        string IThing.CloseInspectionText => $"{InspectionText}.\n{Subject} {Weight} oz";
    }
}
