namespace NeoServer.Game.Enums
{
    public readonly ref struct Result
    {
        public Result(InvalidOperation error)
        {

            Error = error;
        }

        public InvalidOperation Error { get; }
        public readonly bool Success => Error == InvalidOperation.None;
    }
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
