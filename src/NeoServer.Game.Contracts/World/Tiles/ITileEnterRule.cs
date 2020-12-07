using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.World.Tiles
{
    public interface ITileEnterRule
    {
        Func<ITile, bool> CanEnter { get; }
    }
}
