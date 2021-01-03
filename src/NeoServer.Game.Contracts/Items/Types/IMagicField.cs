using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Contracts.Items
{
    public interface IMagicField
    {
        Location Location { get; }
        IItemType Metadata { get; }
        MagicFieldType Type { get; }
    }
}