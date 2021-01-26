using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Data.Model
{
    public class AccountVipListModel
    {
        public int AccountId { get; set; }
        public int PlayerId { get; set; }
        public string Description { get; set; }
        public virtual PlayerModel Player { get; set; }
    }
}
