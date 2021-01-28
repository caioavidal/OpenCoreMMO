using NeoServer.Server.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Data.Model
{
    public class GuildMembershipModel
    {
        public int PlayerId { get; set; }
        public int GuildId { get; set; }
        public int RankId { get; set; }
        public string Nick { get; set; }

        public virtual PlayerModel Player { get; set; }
        public virtual GuildModel Guild { get; set; }
        public virtual GuildRankModel Rank { get; set; }
    }
}
