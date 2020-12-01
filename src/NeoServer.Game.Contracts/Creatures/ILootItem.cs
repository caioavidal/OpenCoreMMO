using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface ILootItem
    {
        ushort ItemId { get;  }
        byte Amount { get;  }
        uint Chance { get; }
        ILootItem[] Items { get; }
    }
}
