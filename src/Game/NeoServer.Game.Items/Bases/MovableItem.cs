using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Bases;

public abstract class MovableItem : BaseItem, IMovableThing, IMovableItem
{
    protected MovableItem(IItemType type, Location location) : base(type, location)
    {
    }

    public float Weight => Metadata.Weight;

    public virtual void OnMoved(IThing to)
    {
        if (to is IContainer container)
        {
            SetContainer(container);
            return;
        }
        
        SetContainer(null);
    }

    public void SetContainer(IContainer container) => Container = container;
    public IContainer Container { get; private set; }
    public IMovableThing Clone()
    {
        var clone = (IMovableThing)MemberwiseClone();
        return clone;
    }
}