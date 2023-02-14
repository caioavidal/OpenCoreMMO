using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Managers;

public class DecayableItemManager : IDecayableItemManager
{
    private readonly SortedList<DateTime, IDecayable> _items;
    private readonly object _listLock = new();

    public DecayableItemManager()
    {
        _items = new SortedList<DateTime, IDecayable>();
    }

    public void Add(IDecayable decayable)
    {
        lock (_listLock)
        {
            var expiresAt = DateTime.Now.AddSeconds(decayable.Remaining);
            _items.Add(expiresAt, decayable);
        }
    }

    public List<IDecayable> DecayExpiredItems()
    {
        var expiredItems = new List<IDecayable>(5);

        lock (_listLock)
        {
            foreach (var (_, decayable) in _items.ToList())
            {
                if (!decayable.Expired) break;

                decayable.TryDecay();
                expiredItems.Add(decayable);
                _items.RemoveAt(0);
            }
        }

        return expiredItems;
    }
}