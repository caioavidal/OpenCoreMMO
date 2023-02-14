namespace NeoServer.Data.Model;

public class GuildRankModel
{
    public int Id { get; set; }
    public int GuildId { get; set; }
    public string Name { get; set; }
    public int Level { get; set; }
    public virtual GuildModel Guild { get; set; }
}