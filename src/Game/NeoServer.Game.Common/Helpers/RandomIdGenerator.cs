using System;

public class RandomIdGenerator
{
    private static Random _random = new Random();
    private static object _lock = new object();
    public static uint Generate()
    {
        lock (_lock)
        {
            return (uint)_random.Next(0, int.MaxValue);
        }
    }
    public static ushort Generate(ushort maxValue)
    {
        lock (_lock)
        {
            return (ushort)_random.Next(0, maxValue);
        }
    }
}

