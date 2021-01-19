using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Common
{
    public struct OperationResult<T>
    {
        public List<(T, Operation, byte)> Operations { get; private set; }
        public void Add(Operation operation, T thing, byte position = 0)
        {
            Operations = Operations ?? new();
            Operations.Add((thing, operation, position));
        }

        public OperationResult(Operation operation, T thing, byte position = 0)
        {
            Operations = new();
            Operations.Add((thing, operation, position));
        }

        public bool HasAnyOperation => Operations?.Any() ?? false;
    }
}
