namespace NeoServer.Data.Model;

public class PlayerQuestModel
{
    public int PlayerId { get; set; }
    public PlayerModel Player { get; set; }
    public string Group { get; set; }
    public string GroupKey { get; set; }
    public int ActionId { get; set; }
    public int UniqueId { get; set; }
    public string Name { get; set; }
    public bool Done { get; set; }
}