using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Creatures.Players;

namespace NeoServer.Game.Creatures.Player;

public class Outfit : IOutfit, IEqualityComparer<Outfit>
{
    public ushort LookType { get; set; }

    public byte Head { get; set; }

    public byte Body { get; set; }

    public byte Legs { get; set; }

    public byte Feet { get; set; }
    public byte Addon { get; set; }

    public ushort LookItemTypeId { get; set; }

    public Gender Type { get; set; }

    public string Name { get; private set; }

    public bool Premium { get; set; }

    public bool Unlocked { get; private set; }

    bool IOutfit.Enabled { get; set; }

    public void Change(ushort lookType, byte head, byte body, byte legs, byte feet, byte addon,
        ushort outfitLookItemTypeId)
    {
        LookType = lookType;
        Head = head;
        Body = body;
        Legs = legs;
        Feet = feet;
        Addon = addon;
        LookItemTypeId = outfitLookItemTypeId;
    }

    public IOutfit Clone()
    {
        return (IOutfit)MemberwiseClone();
    }

    public IOutfit SetName(string name)
    {
        Name = name;
        return this;
    }
    public IOutfit SetUnlocked(bool unlocked)
    {
        Unlocked = unlocked;
        return this;
    }

    public IOutfit LookAsAnItem(IItem item)
    {
        Name = item.Name;
        Unlocked = true;
        Premium = false;
        LookItemTypeId = item.ClientId;
        return this;
    }

    public bool Equals(Outfit x, Outfit y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.LookType == y.LookType && x.Head == y.Head && x.Body == y.Body && x.Legs == y.Legs && x.Feet == y.Feet && x.Addon == y.Addon && x.LookItemTypeId == y.LookItemTypeId && x.Type == y.Type;
    }

    public int GetHashCode(Outfit obj) => HashCode.Combine(obj.LookType, obj.Head, obj.Body, obj.Legs, obj.Feet, obj.Addon, obj.LookItemTypeId, (int)obj.Type);
}