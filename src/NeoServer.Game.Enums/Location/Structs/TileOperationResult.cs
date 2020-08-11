using System.Collections.Generic;

namespace NeoServer.Game.Enums.Location.Structs
{
    public struct TileOperationResult
    {

        public List<Operation> Operations { get; private set; }

        public void Add(Operation operation)
        {
            Operations = Operations ?? new List<Operation>(2);
            Operations.Add(operation);
        }
    }
}
