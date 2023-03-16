using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Inspection;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Factories.AttributeFactory;

namespace NeoServer.Game.Items.Bases;

public abstract class BaseItem : IItem
{
    protected BaseItem(IItemType metadata, Location location)
    {
        Location = location;
        Metadata = metadata;

        Decay = DecayableFactory.CreateIfItemIsDecayable(this);
    }

    public void MarkAsDeleted() => IsDeleted = true;
    public bool IsDeleted { get; private set; }

    public void SetActionId(ushort actionId) => ActionId = actionId;
    public void SetUniqueId(uint uniqueId) => UniqueId = uniqueId;
    public ushort ActionId { get; private set; }
    public uint UniqueId { get; private set; }

    public IItemType Metadata { get; private set; }
    public void UpdateMetadata(IItemType newMetadata) => Metadata = newMetadata;

    public Location Location { get; set; }
    public void SetNewLocation(Location location)
    {
        if (!((IItem)this).CanBeMoved) return;
        Location = location;
    }

    public virtual string GetLookText(IInspectionTextBuilder inspectionTextBuilder, IPlayer player,
        bool isClose = false) =>
        inspectionTextBuilder is null
            ? $"You see {Metadata.Article} {Metadata.Name}."
            : inspectionTextBuilder.Build(this, player, isClose);

    public string FullName => Metadata.FullName;
    public byte Amount { get; set; } = 1;

    public virtual void Use(IPlayer usedBy)
    {
        //do nothing
    }

    public override string ToString()
    {
        var plural = Metadata.Plural ?? $"{Metadata.Name}s";
        return Amount > 1 ? $"{Amount} {plural}" : Metadata.FullName;
    }
    
    public virtual float Weight => Metadata.Weight;

    private IThing _owner;
    public void SetOwner(IThing owner)
    {
        Owner = owner;
    }
    public IThing Owner
    {
        get => _owner is IContainer container ? container.RootParent : _owner;
        private set => _owner = value;
    }

    #region Decay
    public IDecayable Decay { get; protected set; }
    #endregion
}