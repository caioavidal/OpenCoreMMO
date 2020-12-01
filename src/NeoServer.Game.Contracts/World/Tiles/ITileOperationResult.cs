using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.World.Tiles
{
    public interface ITileOperationResult
    {
        List<(IThing,Operation)> Operations { get; }
        bool HasAnyOperation { get; }

        void Add(Operation operation, IThing thing);
    }
}
