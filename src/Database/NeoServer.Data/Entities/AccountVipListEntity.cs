namespace NeoServer.Data.Entities;

public class AccountVipListEntity
{
    public int AccountId { get; set; }
    public int PlayerId { get; set; }
    public string Description { get; set; }
    public virtual PlayerEntity Player { get; set; }
}