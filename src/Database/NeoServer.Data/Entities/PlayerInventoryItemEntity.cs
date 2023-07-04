namespace NeoServer.Data.Entities;

public sealed class PlayerInventoryItemEntity
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public int ServerId { get; set; }
    public int SlotId { get; set; }
    public short Amount { get; set; }

    public PlayerEntity Player { get; set; }
}