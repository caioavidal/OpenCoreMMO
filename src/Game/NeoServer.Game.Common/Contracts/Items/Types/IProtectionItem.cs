using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items.Types
{
    public interface IProtectionItem : IItem
    {
        void DressedIn(IPlayer player);
        void UndressFrom(IPlayer player);
    }
}
