namespace NeoServer.Data.Model;

public class PlayerItemModel
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public short Amount { get; set; }
    public int ParentId { get; set; }
    public int ServerId { get; set; }

    public virtual PlayerModel Player { get; set; }
    public ushort? DecayTo { get; set; }
    public uint? DecayDuration { get; set; }
    public uint? DecayElapsed { get; set; }

    public ushort? Charges { get; set; }
}