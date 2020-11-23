using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Common.Location.Structs
{
    public struct TileOperationResult
    {

        public IList<Operation> Operations { get; private set; }

        public void Add(Operation operation)
        {
            Operations = Operations ?? new List<Operation>(2);
            Operations.Add(operation);
        }

        public bool HasNoneOperation => Operations?.Contains(Operation.None) ?? false;
    }
}
