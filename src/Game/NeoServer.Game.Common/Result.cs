namespace NeoServer.Game.Common
{
    public readonly ref struct Result
    {
        public Result(InvalidOperation error)
        {
            Error = error;
        }

        public InvalidOperation Error { get; }
        public bool IsSuccess => Error == InvalidOperation.None;
        public static Result Success => new(InvalidOperation.None);
        public static Result NotPossible => new(InvalidOperation.NotPossible);
        public static Result Fail(InvalidOperation invalidOperation) => new(invalidOperation);

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

        public Result ResultValue => new(Error);

        public T Value { get; }
        public InvalidOperation Error { get; }
        public bool IsSuccess => Error == InvalidOperation.None;
        public static Result<T> Success => new(InvalidOperation.None);

        public static Result<T> NotPossible => new(InvalidOperation.NotPossible);
    }
}