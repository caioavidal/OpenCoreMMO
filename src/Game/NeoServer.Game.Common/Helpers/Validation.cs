using System.Collections.Generic;

public static class Validation
{
    public static bool IsNull(this object value) => value is null;
    public static bool IsNotNull(this object value) => value is not null;

    public static bool IsLessThanZero(this int value) => value < 0;
    public static bool IsLessThan<T>(this T value, T secondValue) => Comparer<T>.Default.Compare(value, secondValue) < 0;
    public static bool ThrowIfBiggerThan<T>(this T value, T secondValue) => Comparer<T>.Default.Compare(value, secondValue) > 0;
    public static bool ThrowIfNotEqualsTo<T>(this T value, T secondValue) => Comparer<T>.Default.Compare(value, secondValue) != 0;
}

public class Guard
{
    public static bool AnyNull(params object[] values)
    {
        foreach (var value in values)
        {
            if (value.IsNull()) return true;
        }
        return false;
    }
}