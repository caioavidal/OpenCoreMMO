using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World.Tiles;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.World.Map.Tiles
{
    public struct OperationResult : IOperationResult
    {
        public List<(IThing, Operation, byte)> Operations { get; private set; }
        public void Add(Operation operation, IThing thing, byte stackPosition = 0)
        {
            Operations = Operations ?? new();
            Operations.Add((thing, operation, stackPosition));
        }

        public OperationResult(Operation operation, IThing thing, byte stackPosition = 0)
        {
            Operations = new();
            Operations.Add((thing, operation, stackPosition));
        }

        public bool HasAnyOperation => Operations?.Any() ?? false;
    }
}
