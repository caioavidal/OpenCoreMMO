namespace NeoServer.Game.Common
{
    public readonly ref struct Result
    {
        public Result(InvalidOperation error)
        {

            Error = error;
        }

        public InvalidOperation Error { get; }
        public readonly bool IsSuccess => Error == InvalidOperation.None;
        public static Result Success => new Result(InvalidOperation.None);
        public static Result NotPossible => new Result(InvalidOperation.NotPossible);
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
        public Result ResultValue => new Result(Error);

        public T Value { get; }
        public InvalidOperation Error { get; }
        public readonly bool IsSuccess => Error == InvalidOperation.None;
        public static Result<T> Success => new Result<T>(InvalidOperation.None);

        public static Result<T> NotPossible => new Result<T>(InvalidOperation.NotPossible);

    }
}
