using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Common.Contracts.World.Tiles;

public interface IOperationResult
{
    List<(IThing, Operation, byte)> Operations { get; }
    bool HasAnyOperation { get; }

    void Add(Operation operation, IThing thing, byte stackPosition = 0);
}