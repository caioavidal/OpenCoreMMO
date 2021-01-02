using NeoServer.Server.Model.Players;

namespace NeoServer.Data.Model
{
    public partial class PlayerInventoryItemModel
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int ServerId { get; set; }
        public int SlotId { get; set; }
        public short Amount { get; set; }

        public virtual PlayerModel Player { get; set; }
    }
}
