using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Inspection;
using NeoServer.Game.Common.Location;

namespace NeoServer.Game.Common.Contracts.Items;

public interface IThing
{
    Location.Structs.Location Location { get; set; }
    string Name { get; }

    public byte Amount => 1;

    string GetLookText(IInspectionTextBuilder inspectionTextBuilder, IPlayer player, bool isClose = false);

    public bool IsCloseTo(IThing thing)
    {
        if (Location.Type is not LocationType.Ground && 
            this is IMovableItem movableItem)
        {
            return movableItem.Owner?.Location.IsNextTo(thing.Location) ?? false;
        }
        return Location.IsNextTo(thing.Location);   
    }
}