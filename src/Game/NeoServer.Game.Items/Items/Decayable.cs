using System;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;

namespace NeoServer.Game.Items.Items
{
    public class Decayable : Dressable
    {
        public Decayable(IItemType type, Location location) : base(type, location)
        {
        }

        public ushort Duration => Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.Duration);
        public byte charges => Metadata.Attributes.GetAttribute<byte>(ItemAttribute.Charges);
        public bool Expired => Duration <= 0 && Charges <= 0;

        public byte Charges { get; private set; }

        public void DecreaseCharges()
        {
            if (!Expired) Charges--;
        }

        public void StartDecaying()
        {
            throw new NotImplementedException();
        }
    }
}