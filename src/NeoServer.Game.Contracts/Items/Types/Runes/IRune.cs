using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures.Structs;
using NeoServer.Game.Common.Helpers;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Items.Types.Runes
{
    public interface IRune : IItemRequirement, IPickupable, IFormula
    {
        public CooldownTime Cooldown { get; }
     
    }
}
