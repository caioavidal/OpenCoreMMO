using NeoServer.Server.Model.Players;

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
