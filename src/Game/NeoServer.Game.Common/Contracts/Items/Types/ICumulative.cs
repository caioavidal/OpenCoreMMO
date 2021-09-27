using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Types
{
    public delegate void ItemReduce(ICumulative item, byte amount);

    public interface ICumulative : IPickupable
    {
        public new byte Amount { get; set; }

        new float Weight { get; }
        byte AmountToComplete { get; }

        event ItemReduce OnReduced;

        bool TryJoin(ref ICumulative item);
        float CalculateWeight(byte amount);
        ICumulative Clone(byte amount);
        ICumulative Split(byte amount);
        void ClearSubscribers();

        public static bool IsApplicable(IItemType type)
        {
            return type.Flags.Contains(ItemFlag.Stackable);
        }
    }
}