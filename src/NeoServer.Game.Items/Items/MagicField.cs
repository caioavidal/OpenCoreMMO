using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Game.Items.Items
{
    public readonly struct MagicField : IItem, IMagicField
    {
        public MagicField(IItemType type, Location location)
        {
            Metadata = type;
            Location = location;
        }

        public Location Location { get; }

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

