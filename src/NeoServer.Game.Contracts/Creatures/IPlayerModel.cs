using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface IPlayerModel
    {
        IAccountModel Account { get; set; }
        float Capacity { get; set; }
        string CharacterName { get; set; }
        ChaseMode ChaseMode { get; set; }
        FightMode FightMode { get; }
        Gender Gender { get; set; }
        ushort HealthPoints { get; set; }
        int Id { get; }
        Dictionary<Slot, ushort> Inventory { get; set; }
        IEnumerable<IItemModel> Items { get; set; }
        ushort Level { get; set; }
        Location Location { get; set; }
        ushort Mana { get; set; }
        ushort MaxHealthPoints { get; set; }
        ushort MaxMana { get; set; }
        byte MaxSoulPoints { get; set; }
        bool Online { get; set; }
        IOutfit Outfit { get; set; }
        IDictionary<SkillType, ISkill> Skills { get; set; }
        byte SoulPoints { get; set; }
        ushort Speed { get; set; }
        ushort StaminaMinutes { get; set; }
        VocationType Vocation { get; set; }

        bool IsMounted();
    }
}
