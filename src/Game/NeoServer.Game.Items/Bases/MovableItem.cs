using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Bases;

public class MovableItem : BaseItem, IMovableThing, IMovableItem
{

    public MovableItem(IItemType type, Location location) : base(type, location)
    {
    }

    public float Weight => Metadata.Weight;

    public virtual void OnMoved(IThing to) { }

    public void SetOwner(IThing owner) => Owner = owner;

    private IThing _owner;

    public IThing Owner
    {
        get => _owner is IContainer container ? container.RootParent : _owner;
        private set => _owner = value;
    }

    public IMovableThing Clone()
    {
        var clone = (IMovableThing)MemberwiseClone();
        return clone;
    }
}