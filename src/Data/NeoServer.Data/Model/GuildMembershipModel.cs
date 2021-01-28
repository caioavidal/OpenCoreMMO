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

        public PlayerModel Player { get; set; }
        public GuildModel Guild { get; set; }
    }
}
