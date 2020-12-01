using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures
{
    public record LootItem(ushort ItemId, byte Amount, uint Chance, ILootItem[] Items) : ILootItem;
}
