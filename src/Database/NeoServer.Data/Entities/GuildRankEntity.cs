namespace NeoServer.Data.Entities;

public class GuildRankEntity
{
    public int Id { get; set; }
    public int GuildId { get; set; }
    public string Name { get; set; }
    public int Level { get; set; }
    public virtual GuildEntity Guild { get; set; }
}