using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Items;

namespace NeoServer.Game.Items.Items
{
    public struct MagicField : IItem, IMagicField
    {
        public MagicField(IItemType type, Location location)
        {
            Metadata = type;
            Location = location;
        }

        public Location Location { get; set; }

        public MagicFieldType Type => ParseFieldType(Metadata.Attributes.GetAttribute(ItemAttribute.Field));

        public IItemType Metadata { get; }

        public static bool IsApplicable(IItemType type) => type.Attributes.GetAttribute(ItemAttribute.Type) == "magicfield";

        private MagicFieldType ParseFieldType(string type) => type switch
        {
            "fire" => MagicFieldType.Fire,
            _ => MagicFieldType.None
        };

    }
}

