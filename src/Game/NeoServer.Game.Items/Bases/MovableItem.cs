using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Bases;

public class MovableItem : BaseItem, IMovableThing, IMovableItem
{
    private IThing _owner;

    public MovableItem(IItemType type, Location location) : base(type, location)
    {
    }

    public virtual float Weight => Metadata.Weight;

    public void SetOwner(IThing owner)
    {
        Owner = owner;
    }

    public IThing Owner
    {
        get => _owner is IContainer container ? container.RootParent : _owner;
        private set => _owner = value;
    }

    public virtual void OnMoved(IThing to)
    {
    }

    public IMovableThing Clone()
    {
        var clone = (IMovableThing)MemberwiseClone();
        return clone;
    }
}