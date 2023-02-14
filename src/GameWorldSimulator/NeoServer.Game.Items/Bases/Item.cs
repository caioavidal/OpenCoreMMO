using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Factories.AttributeFactory;

namespace NeoServer.Game.Items.Bases;

public class Item : StaticItem, IHasDecay
{
    public Item(IItemType metadata, Location location) : base(metadata, location)
    {
        Decayable = DecayableFactory.Create(this);

        if (Decayable is not null) Decayable.OnDecayed += Decayed;
    }

    public IDecayable Decayable { get; }

    private void Decayed(ushort to)
    {
        Transform(null);
    }
}