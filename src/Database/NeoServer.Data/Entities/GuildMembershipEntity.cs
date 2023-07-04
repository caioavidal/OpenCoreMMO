namespace NeoServer.Data.Entities;

public class GuildMembershipEntity
{
    public int PlayerId { get; set; }
    public int GuildId { get; set; }
    public int RankId { get; set; }
    public string Nick { get; set; }

    public virtual PlayerEntity Player { get; set; }
    public virtual GuildEntity Guild { get; set; }
    public virtual GuildRankEntity Rank { get; set; }
}