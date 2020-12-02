using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Common.Players;
using NeoServer.Server.Model.Players.Contracts;
using System.Collections.Immutable;
using System.Text;

namespace NeoServer.Game.Contracts.Items.Types
{
    public interface IInventoryItem : IItem
    {
        public Slot Slot => Metadata.BodyPosition;
    }
    public interface IBodyEquipmentItem : IMoveableThing, IPickupable, IInventoryItem
    {
        bool Pickupable => true;
        IItemRequirement[] Requirements => Metadata.Requirements;
        bool CanWear(IPlayer player)
        {
            if (Requirements is null || Requirements.Length == 0) return true;

            foreach (var requirement in Requirements)
            {
                if (requirement.Vocation == VocationType.All && requirement.MinLevel < player.Level) return true;
                if (requirement.Vocation == player.Vocation && requirement.MinLevel < player.Level) return true;
            }
            return false;
        }
        ushort MinimumLevelRequired => Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.MinimumLevel);
        public ImmutableDictionary<SkillType, byte> SkillBonus => Metadata.Attributes.SkillBonus.ToImmutableDictionary();
        public WeaponType WeaponType => Metadata.WeaponType;

        protected string RequirementText
        {
            get
            {
                if (Requirements is null || Requirements.Length == 0) return string.Empty;

                var stringBuilder = new StringBuilder();
                var sufix = "\nIt can only be wielded properly by";

                var minLevel = 0;
                foreach (var requirement in Requirements)
                {
                    if (requirement.MinLevel == 0) return string.Empty;

                    if (requirement.Vocation == VocationType.All)
                    {
                        stringBuilder.Clear();
                        stringBuilder.Append($"players of level {requirement.MinLevel} or higher");
                        return $"{sufix} {stringBuilder}";
                    }

                    stringBuilder.Append($"{VocationTypeParser.Parse(requirement.Vocation).ToLower()}s, ");
                    minLevel = requirement.MinLevel;
                }

                stringBuilder.Remove(stringBuilder.Length - 2, 2);
                stringBuilder.Append($" of level {minLevel} or higher");

                return $"{sufix} {stringBuilder}";
            }
        }
    }
}
