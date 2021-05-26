using Microsoft.Extensions.Caching.Memory;
using NeoServer.Game.Contracts.Items.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Commands.Player.UseItem
{
    public class HotKeyCache
    {
        private IMemoryCache _cache;

        public HotKeyCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Add(uint playerId, ushort clientId, IContainer container, byte slotIndex)
        {
            _cache.Set((playerId, clientId), new HotkeyItemLocation(container, slotIndex), TimeSpan.FromSeconds(60));
        }

        public HotkeyItemLocation Get(uint playerId, ushort clientId) => _cache.Get((playerId, clientId)) as HotkeyItemLocation;

        public record HotkeyItemLocation(IContainer Container, byte SlotIndex);
    }
}
