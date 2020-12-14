using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts
{
    public interface IProbability
    {
        byte Chance { get; init; }
    }
}
