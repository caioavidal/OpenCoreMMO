using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Common
{
    public struct OperationResult<T>
    {
        public List<(T, Operation, byte)> Operations { get; private set; }

        public void Add(Operation operation, T thing, byte position = 0)
        {
            Operations ??= new List<(T, Operation, byte)>();
            Operations.Add((thing, operation, position));
        }

        public OperationResult(Operation operation, T thing, byte position = 0)
        {
            Operations = new List<(T, Operation, byte)>();
            Operations.Add((thing, operation, position));
        }

        public OperationResult(T value)
        {
            Operations = new List<(T, Operation, byte)>();
            Operations.Add((value, Operation.None, 0));
        }

        public bool HasAnyOperation => Operations?.Any() ?? false;
    }
}