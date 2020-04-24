using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Game.Contracts.Items
{
    public interface IThing
    {
        Location Location { get; }

        string Name { get; }
        string InspectionText { get; }
        string CloseInspectionText { get; }
    }
}