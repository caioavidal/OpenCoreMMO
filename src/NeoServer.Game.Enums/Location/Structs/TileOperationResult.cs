using NeoServer.Game.Enums;
using System;
using System.Collections.Generic;
using System.Text;

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
