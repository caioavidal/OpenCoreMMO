using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Items
{
    public interface IChargeable
    {
        byte Charges { get; }
        void DecreaseCharges();

    }
}
