using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Types
{
    public delegate void ItemReduce(ICumulative item, byte amount);

    public interface ICumulative : IPickupable
    {
        public byte Amount { get; set; }

        bool HasMany => Amount > 1;
        new float Weight { get; }
        byte AmountToComplete { get; }
        string Subject => HasMany ? "They weigh" : "It weighs";
        string IItem.LookText => HasMany ? $"{Amount} {Plural ?? $"{Name}s"}" : $"{Metadata.Article} {Name}";
        string IThing.InspectionText => LookText;
        string IThing.CloseInspectionText => $"{InspectionText}.\n{Subject} {Weight} oz";

        event ItemReduce OnReduced;

        bool TryJoin(ref ICumulative item);
        float CalculateWeight(byte amount);
        ICumulative Clone(byte amount);
        void Increase(byte amount);
        ICumulative Split(byte amount);
        void ClearSubscribers();

        public static bool IsApplicable(IItemType type)
        {
            return type.Flags.Contains(ItemFlag.Stackable);
        }
    }
}