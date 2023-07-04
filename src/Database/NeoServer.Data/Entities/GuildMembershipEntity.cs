namespace NeoServer.Data.Entities;

public sealed class GuildMembershipEntity
{
    public int PlayerId { get; set; }
    public int GuildId { get; set; }
    public int RankId { get; set; }
    public string Nick { get; set; }

    public PlayerEntity Player { get; set; }
    public GuildEntity Guild { get; set; }
    public GuildRankEntity Rank { get; set; }
}