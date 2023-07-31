using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Creatures.Players;

namespace NeoServer.Game.Common.Contracts.Creatures;

public interface IOutfit
{
    ushort LookType { get; set; }
    byte Head { get; set; }
    byte Body { get; set; }
    byte Legs { get; set; }
    byte Feet { get; set; }
    byte Addon { get; set; }

    public Gender Type { get; protected set; }
    public string Name { get;  }
    public bool Premium { get; protected set; }
    public bool Unlocked { get; }
    public bool Enabled { get; protected set; }
    ushort LookItemTypeId { get; set; }

    void Change(ushort lookType, byte head, byte body, byte legs, byte feet, byte addon, ushort outfitLookItemTypeId);
    IOutfit Clone();
    IOutfit SetName(string name);

    public IOutfit SetPremium(bool premium)
    {
        Premium = premium;
        return this;
    }

    public IOutfit SetEnabled(bool enabled)
    {
        Enabled = enabled;
        return this;
    }

    public IOutfit SetGender(Gender type)
    {
        Type = type;
        return this;
    }

    IOutfit SetUnlocked(bool unlocked);
    IOutfit LookAsAnItem(IItem item);
}