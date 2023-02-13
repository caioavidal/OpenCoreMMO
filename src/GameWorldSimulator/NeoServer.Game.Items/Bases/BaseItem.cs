using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Inspection;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Bases;

public abstract class BaseItem : IItem
{
    protected BaseItem(IItemType metadata, Location location)
    {
        Location = location;
        Metadata = metadata;
    }

    public void SetActionId(ushort actionId)
    {
        ActionId = actionId;
    }

    public void SetUniqueId(uint uniqueId)
    {
        UniqueId = uniqueId;
    }

    public ushort ActionId { get; private set; }
    public uint UniqueId { get; private set; }

    public IItemType Metadata { get; protected set; }
    public Location Location { get; set; }

    public virtual string GetLookText(IInspectionTextBuilder inspectionTextBuilder, IPlayer player,
        bool isClose = false)
    {
        return inspectionTextBuilder is null
            ? $"You see {Metadata.Article} {Metadata.Name}."
            : inspectionTextBuilder.Build(this, player, isClose);
    }

    public bool IsPickupable => Metadata.HasFlag(ItemFlag.Pickupable);
    public string FullName => Metadata.FullName;
    public byte Amount { get; set; } = 1;

    public void Transform(IPlayer by)
    {
        OnTransform?.Invoke(by, this, Metadata.Attributes.GetTransformationItem());
    }

    public void Transform(IPlayer by, ushort to)
    {
        OnTransform?.Invoke(by, this, to);
    }

    public event Transform OnTransform;

    public virtual void Use(IPlayer usedBy)
    {
    }

    public override string ToString()
    {
        var plural = Metadata.Plural ?? $"{Metadata.Name}s";
        return Amount > 1 ? $"{Amount} {plural}" : Metadata.FullName;
    }
}