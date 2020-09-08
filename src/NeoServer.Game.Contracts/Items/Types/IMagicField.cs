using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Game.Contracts.Items
{
    public interface IMagicField
    {
        Location Location { get; }
        IItemType Metadata { get; }
        MagicFieldType Type { get; }
    }
}