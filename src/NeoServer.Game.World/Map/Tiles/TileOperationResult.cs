using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World.Tiles;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.World.Map.Tiles
{
    public struct TileOperationResult: IOperationResult
    {
        public List<(IThing,Operation)> Operations { get; private set; }
        public void Add(Operation operation, IThing thing)
        {
            Operations = Operations ?? new();
            Operations.Add((thing,operation));
        }

        public bool HasAnyOperation => Operations?.Any() ?? false;
    }
}
