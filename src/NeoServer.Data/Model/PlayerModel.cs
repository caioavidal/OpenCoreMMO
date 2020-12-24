using NeoServer.Data.Model;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using System.Collections.Generic;
namespace NeoServer.Server.Model.Players
{
  

    public class PlayerModel : IPlayerModel
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string CharacterName { get; set; }

        public AccountModel Account { get; set; }
        public ChaseMode ChaseMode { get; set; }
        public float Capacity { get; set; }
        public ushort Level { get; set; }
        public ushort HealthPoints { get; set; }
        public ushort MaxHealthPoints { get; set; }
        public VocationType Vocation { get; set; }
        public Gender Gender { get; set; }
        public bool Online { get; set; }
        public ushort Mana { get; set; }
        public ushort MaxMana { get; set; }
        public FightMode FightMode { get; }
        public byte SoulPoints { get; set; }
        public byte MaxSoulPoints { get; set; }
        public IDictionary<SkillType, ISkill> Skills { get; set; }

        public IOutfit Outfit { get; set; }

        public ushort StaminaMinutes { get; set; }
        public byte AmmoAmount { get; set; }
        public Dictionary<Slot, ushort> Inventory { get; set; }
        public IEnumerable<IItemModel> Items { get; set; } = new List<ItemModel>();
        public ushort Speed { get; set; }

        public Location Location { get; set; }

        public bool IsMounted()
        {
            return false;
        }
    }

    public class ItemModel:IItemModel
    {
        public ushort ServerId { get; set; }
        public byte Amount { get; set; }
        public IEnumerable<IItemModel> Items { get; set; } = new List<ItemModel>();
    }
}