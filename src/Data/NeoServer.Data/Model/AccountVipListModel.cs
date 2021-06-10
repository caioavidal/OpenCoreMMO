namespace NeoServer.Data.Model
{
    public class AccountVipListModel
    {
        public int AccountId { get; set; }
        public int PlayerId { get; set; }
        public string Description { get; set; }
        public virtual PlayerModel Player { get; set; }
    }
}