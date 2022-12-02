namespace NeoServer.Game.Common.Item;

public class QuestData
{
    public (ushort, uint) Key => (ActionId, UniqueId);
    public ushort ActionId { get; set; }
    public uint UniqueId { get; set; }
    public string Script { get; set; }
}