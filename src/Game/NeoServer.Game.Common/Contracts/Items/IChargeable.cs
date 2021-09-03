using System;

namespace NeoServer.Game.Common.Contracts.Items
{
    public interface IChargeable
    {
        ushort Charges { get; }
        bool NoCharges => Charges == 0;
        void DecreaseCharges();
    }
}