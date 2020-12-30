using NeoServer.Server.Model.Players;

namespace NeoServer.Data.Model
{
    public class PlayerDepotItemModel
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int ServerId { get; set; }
        public byte[] Attributes { get; set; }
        public short Amount { get; set; }
        public short Itemtype { get; set; }
        public int ParentId { get; set; }

        public virtual PlayerModel Player { get; set; }
    }
}
