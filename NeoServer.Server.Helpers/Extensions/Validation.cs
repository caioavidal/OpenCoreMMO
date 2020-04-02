using System;

public static class Validation
{
    public static void ThrowIfNull(this object value, string exception = null)
    {
        if (value == null)
        {
            throw new NullReferenceException(exception ?? nameof(value));
        }

    }
    public static void ThrowIfLessThanZero(this int value, string exception = null)
    {
        if (value < 0)
        {
            throw new ArgumentException(exception ?? $"{nameof(value)} can't be less than 0");
        }

    }
    public static void ThrowIfLessThan(this int value, int length, string exception = null)
    {
        if (value < length)
        {
            throw new ArgumentOutOfRangeException(exception ?? $"{nameof(value)} can't be less than {length}");
        }

    }
}