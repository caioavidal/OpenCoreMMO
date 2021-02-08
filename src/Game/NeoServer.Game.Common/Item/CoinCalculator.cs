using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Common.Item
{
    public static class CoinCalculator
    {
        public static IEnumerable<(ushort, byte)> Calculate(uint value)
        {
            return Calculate(value, 10_000);
        }
        private static IEnumerable<(ushort, byte)> Calculate(uint value, int divisor = 10_000, List<(ushort, byte)> coins = null)
        {
            coins = coins ?? new List<(ushort, byte)>(10);

            if (value == 0) return Array.Empty<(ushort, byte)>();

            var money = (value / divisor);

            var coinType = divisor switch { 1 => CoinType.Gold, 100 => CoinType.Platinum, 10_000 => CoinType.Crystal, _ => CoinType.Gold };

            while (money > 0)
            {
                coins.Add(((ushort)coinType, (byte)Math.Min(100, money)));
                money = money - 100;
            }

            uint mod = (uint)(value % divisor);
            if (mod == 0) return coins;

            if (mod >= 100) return Calculate(mod, 100, coins);
            else return Calculate(mod, 1, coins);
        }
    }
}
