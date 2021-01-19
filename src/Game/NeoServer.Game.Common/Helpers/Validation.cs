using System;
using System.Collections.Generic;

public static class Validation
{
    public static void ThrowIfNull(this object value, string exception = null)
    {
        if (value is null)
        {
            throw new NullReferenceException(exception ?? nameof(value));
        }

    }
    public static void ThrowIfLessThanZero(this int value, string exception = null)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(exception ?? $"{nameof(value)} can't be less than 0");
        }

    }
    public static void ThrowIfLessThan<T>(this T value, T secondValue, string exception = null)
    {
        if (Comparer<T>.Default.Compare(value, secondValue) < 0)
        {
            throw new ArgumentOutOfRangeException(exception ?? $"{nameof(value)} can't be less than {secondValue}");
        }

    }
    public static void ThrowIfBiggerThan<T>(this T value, T secondValue, string exception = null)
    {
        if (Comparer<T>.Default.Compare(value, secondValue) > 0)
        {
            throw new ArgumentOutOfRangeException(exception ?? $"{nameof(value)} can't be less than {secondValue}");
        }

    }
    public static void ThrowIfNotEqualsTo<T>(this T value, T secondValue)
    {
        if (Comparer<T>.Default.Compare(value, secondValue) != 0)
        {
            throw new ArgumentException($"{nameof(value)} is not equal to {nameof(secondValue)}");
        }
    }
}