using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.Items
{
    public interface IDressable
    {
        void DressedIn(IPlayer player);
        void UndressFrom(IPlayer player);
    }
}
