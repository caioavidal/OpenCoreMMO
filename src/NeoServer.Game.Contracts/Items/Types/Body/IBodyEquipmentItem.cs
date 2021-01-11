using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Players;
using System.Collections.Immutable;
using System.Text;

namespace NeoServer.Game.Contracts.Items.Types
{
    public interface IInventoryItem : IItemRequirement, IItem
    {
        public Slot Slot => Metadata.BodyPosition;
    }
    public interface IBodyEquipmentItem : IMoveableThing, IPickupable, IInventoryItem
    {
        bool Pickupable => true;
      
        ushort MinimumLevelRequired => Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.MinimumLevel);
        public ImmutableDictionary<SkillType, byte> SkillBonus => Metadata.Attributes.SkillBonus.ToImmutableDictionary();
        public WeaponType WeaponType => Metadata.WeaponType;

        protected string RequirementText
        {
            get
            {
                var stringBuilder = new StringBuilder();
                var sufix = "\nIt can only be wielded properly by";

                for (int i = 0; i < Vocations?.Length; i++)
                {
                    //stringBuilder.Append($"{VocationTypeParser.Parse(Vocations[i]).ToLower()}s");
                    //if (i + 1 < Vocations.Length)
                    //{
                    //    stringBuilder.Append(", ");
                    //}
                }
                if (MinLevel > 0)
                {
                    stringBuilder.Append($" of level {MinLevel} or higher");
                }

                return $"{sufix} {stringBuilder}";
            }
        }
    }
}
