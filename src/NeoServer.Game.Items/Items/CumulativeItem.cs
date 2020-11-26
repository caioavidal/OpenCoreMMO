using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Items.Items
{
    public class CumulativeItem : MoveableItem, ICumulativeItem, IItem
    {
        public CumulativeItem(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(type, location)
        {
            Amount = 1;
            if (attributes != null && attributes.TryGetValue(Common.ItemAttribute.Count, out var amount))
            {
                Amount = Math.Min((byte)100, (byte)amount);
            }
        }
        public CumulativeItem(IItemType type, Location location, byte amount) : base(type, location) => Amount = Math.Min((byte)100, amount);

        public byte Amount { get; set; }

        public new float Weight => CalculateWeight(Amount);

        public float CalculateWeight(byte amount) => Metadata.Weight * amount;

        public Span<byte> GetRaw()
        {
            Span<byte> cache = stackalloc byte[3];
            var idBytes = BitConverter.GetBytes(Metadata.ClientId);

            cache[0] = idBytes[0];
            cache[1] = idBytes[1];
            cache[2] = Amount;

            return cache.ToArray();
        }
        public static bool IsApplicable(IItemType type) => type.Flags.Contains(Common.ItemFlag.Stackable);

        public ICumulativeItem Clone(byte amount)
        {
            var clone = (ICumulativeItem)MemberwiseClone();
            clone.Amount = amount;
            return clone;
        }

        /// <summary>
        /// Reduce amount from item
        /// </summary>
        /// <param name="amount">Amount to be reduced</param>
        public void Reduce(byte amount)
        {
            amount = (byte)(amount > 100 ? 100 : amount);
            Amount -= amount;
        }

        /// <summary>
        /// Split item in two parts
        /// </summary>
        /// <param name="amount">Amount to be reduced</param>
        public ICumulativeItem Split(byte amount)
        {
            Reduce(amount);
            return Clone(amount);
        }

        public void Increase(byte amount) => Amount = (byte)(amount + Amount > 100 ? 100 : amount + Amount);
       

        public byte AmountToComplete => (byte)(100 - Amount);

        public bool TryJoin(ref ICumulativeItem item)
        {

            if (item?.Metadata?.ClientId == null)
            {

                return false;
            }

            if (item.Metadata.ClientId != Metadata.ClientId)
            {
                return false;
            }

            var totalAmount = Amount + item.Amount;

            if (totalAmount <= 100)
            {
                Amount = (byte)totalAmount;
                item = null; 
                return true;
            }

            Amount = 100;

            item.Amount = (byte)(totalAmount - Amount);

            return true;

        }
    }
}
