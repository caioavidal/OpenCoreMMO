using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Items.Types.Useables
{
    public interface IUseableOnCreature: IUseableOn2, IItem
    {
        /// <summary>
        /// Useable by players on creatures
        /// </summary>
        /// <param name="usedBy">player whose item is being used</param>
        public void Use(IPlayer usedBy, ICreature creature);
    }
}
