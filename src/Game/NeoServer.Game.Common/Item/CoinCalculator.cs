using NeoServer.Game.Contracts.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Common.Item
{
    public static class CoinCalculator
    {
        public static IEnumerable<(ushort, byte)> Calculate(IDictionary<ushort, IItemType> coinTypes, ulong value)
        {
            return Calculate(value, coinTypes.ToDictionary(x => x.Key, x => x.Value?.Attributes?.GetAttribute<uint>(ItemAttribute.Worth) ?? 0));
        }
        private static IEnumerable<(ushort, byte)> Calculate(ulong value, IDictionary<ushort, uint> coinTypes, List<(ushort, byte)> coins = null)
        {
            var coinType = coinTypes.Aggregate((l, r) => l.Value > r.Value ? l : r);

            coins = coins ?? new List<(ushort, byte)>(10);

            if (value == 0) return Array.Empty<(ushort, byte)>();

            var (coinId, worth) = coinType;

            long money = (long)(value / worth);

            while (money > 0)
            {
                coins.Add((coinId, (byte)Math.Min(100, money)));
                money = money - 100;
            }

            uint mod = (uint)(value % worth);
            if (mod == 0) return coins;

            coinTypes.Remove(coinId);

            return Calculate(mod, coinTypes, coins);
        }
    }
}
