using NeoServer.Game.Enums.Location;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Enums
{
    public readonly ref struct Result<T>
    {
        public Result(T result, InvalidOperation error = InvalidOperation.None)
        {
            Value = result;
            Error = error;
        }
        public Result(InvalidOperation error)
        {
            Value = default;
            Error = error;
        }

        public T Value { get; }
        public InvalidOperation Error { get; }
        public readonly bool Success => Error == InvalidOperation.None;
    }
}
