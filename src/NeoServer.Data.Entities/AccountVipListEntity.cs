namespace NeoServer.Data.Entities;

public sealed class AccountVipListEntity
{
    public int AccountId { get; set; }
    public int PlayerId { get; set; }
    public string Description { get; set; }
    public PlayerEntity Player { get; set; }
}