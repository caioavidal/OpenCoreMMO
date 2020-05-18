using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Items.Items
{
    public class CumulativeItem : ICumulativeItem, IItem
    {
        public CumulativeItem(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes)
        {
            Metadata = type;
            Amount = 1;
            if (attributes != null && attributes.TryGetValue(Enums.ItemAttribute.Count, out var amount))
            {
                Amount = Math.Min((byte)100, (byte)amount);
            }

            ClientId = type.ClientId;
            Weight = type.Attributes.GetAttribute<float>(Enums.ItemAttribute.Weight);
            this.location = location;
        }
        public CumulativeItem(IItemType type, Location location, byte amount)
        {
            Metadata = type;

            Amount = Math.Min((byte)100, amount);

            ClientId = type.ClientId;
            Weight = type.Attributes.GetAttribute<float>(Enums.ItemAttribute.Weight);
            this.location = location;
        }

        public byte Amount { get; set; }

        public ushort ClientId { get; }

        private float weight;
        public float Weight { get
            {
                 return weight * Amount;
            }
            set
            {
                weight = value;
            }
        }

        private Location location;
        public Location Location => location;

        public IItemType Metadata { get; }

        public Span<byte> GetRaw()
        {
            Span<byte> cache = stackalloc byte[3];
            var idBytes = BitConverter.GetBytes(ClientId);

            cache[0] = idBytes[0];
            cache[1] = idBytes[1];
            cache[2] = Amount;

            return cache.ToArray();
        }
        public static bool IsApplicable(IItemType type) => type.Flags.Contains(Enums.ItemFlag.Stackable);

        public void SetNewLocation(Location location)
        {
            this.location = location;
        }


        public ICumulativeItem Split(byte amount)
        {
            Amount -= amount;

            var splitedItem = (ICumulativeItem)MemberwiseClone();
            splitedItem.Amount = amount;

            return splitedItem;
        }

        public bool TryJoin(ref ICumulativeItem item)
        {

            if (item?.Metadata?.ClientId == null)
            {

                return false;
            }

            if (item.Metadata.ClientId != item.Metadata.ClientId)
            {
                return false;
            }

            var remainingAmount = Amount + item.Amount;

            if (remainingAmount <= 100)
            {
                Amount = (byte)remainingAmount;
                item = null; //item to join does not exist anymore
                return true;
            }

            Amount = 100;

            item.Amount = (byte)(remainingAmount - Amount);

            return true;

        }
    }
}
