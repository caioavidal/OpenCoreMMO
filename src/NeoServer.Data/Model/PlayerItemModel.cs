using NeoServer.Server.Model.Players;

namespace NeoServer.Data.Model
{
    public partial class PlayerItemModel
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public byte[] Attributes { get; set; }
        public short Count { get; set; }
        public short Itemtype { get; set; }
        public int Pid { get; set; }
        public int Sid { get; set; }

        public virtual PlayerModel Player { get; set; }
    }
}
