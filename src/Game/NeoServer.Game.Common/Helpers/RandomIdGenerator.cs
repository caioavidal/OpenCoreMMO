using System;
using NeoServer.Game.Contracts.Creatures;

public class RandomIdGenerator
{
    private static readonly Random _random = new();
    private static readonly object _lock = new();

    public static uint Generate()
    {
        lock (_lock)
        {
            return (uint) _random.Next(0, int.MaxValue);
        }
    }

    public static ushort Generate(ushort maxValue)
    {
        lock (_lock)
        {
            return (ushort) _random.Next(0, maxValue);
        }
    }
}

public class RandomCreatureIdGenerator
{
    //private static Random _random = new Random();
    private static readonly object _lock = new();
    private static uint lastPlayerId = 0x10000000;
    private static uint lastMonsterId = 0x40000000;
    private static uint lastNpcId = 0x80000000;

    public static uint Generate(ICreature creature)
    {
        lock (_lock)
        {
            if (creature is IPlayer)
                return lastPlayerId++;
            if (creature is IMonster)
                return lastMonsterId++;
            if (creature is INpc)
                return lastNpcId++;
        }

        return 0;
    }
}