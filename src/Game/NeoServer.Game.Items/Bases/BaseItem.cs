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

    public event Transform OnTransform;

    public override string ToString()
    {
        var plural = Metadata.Plural ?? $"{Metadata.Name}s";
        return Amount > 1 ? $"{Amount} {plural}" : Metadata.FullName;
    }
}