using System.Collections.Generic;
using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Items;

namespace NeoServer.Game.Contracts.World.Tiles
{
    public interface IOperationResult
    {
        List<(IThing, Operation, byte)> Operations { get; }
        bool HasAnyOperation { get; }

        void Add(Operation operation, IThing thing, byte stackPosition = 0);
    }
}