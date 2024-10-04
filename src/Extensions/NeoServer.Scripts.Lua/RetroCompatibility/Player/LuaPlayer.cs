using System.Collections.Generic;
using NeoServer.Game.Common.Combat;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Scripts.Lua.RetroCompatibility.Player;

public class LuaPlayer : Game.Creature.Player.Player
{
    public LuaPlayer(uint id, string characterName, ChaseMode chaseMode, uint capacity, uint healthPoints, uint maxHealthPoints, IVocation vocation, Gender gender, bool online, ushort mana, ushort maxMana, FightMode fightMode, byte soulPoints, byte soulMax, IDictionary<SkillType, ISkill> skills, ushort staminaMinutes, IOutfit outfit, ushort speed, Location location, IMapTool mapTool, ITown town) : base(id, characterName, chaseMode, capacity, healthPoints, maxHealthPoints, vocation, gender, online, mana, maxMana, fightMode, soulPoints, soulMax, skills, staminaMinutes, outfit, speed, location, mapTool, town)
    {
    }

    public Skull getSkull() => (Skull)Skull;
}