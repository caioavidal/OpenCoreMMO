using NeoServer.Game.Contracts.Items;
using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Data.Model
{
    public record PlayerDepotModel (uint PlayerId, List<IItemModel> Items);
}
