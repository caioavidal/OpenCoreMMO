using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Items.Types.Useables
{
    public interface IUseableOnItem : IUseableOn2, IItem
    {
        /// <summary>
        /// Useable by creatures on items (ground, weapon, stairs..)
        /// </summary>
        /// <param name="usedBy">player whose item is being used</param>
        /// <param name="item">item which will receive action</param>
        public bool Use(ICreature usedBy, IItem item);
    }
 
}
