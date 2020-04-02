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
}