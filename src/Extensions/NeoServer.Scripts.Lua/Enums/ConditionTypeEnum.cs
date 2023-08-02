using NeoServer.Game.Common.Creatures;
using NLua;

namespace NeoServer.Scripts.Lua.Enums;
public static class ConditionTypeEnum
{
    public static void Register(NLua.Lua lua)
    {
        if (lua["CONDITION_TYPE"] is not LuaTable conditionType) return;

        conditionType["None"] = ConditionType.None;
        conditionType["Poison"] = ConditionType.Poison;
        conditionType["Fire"] = ConditionType.Fire;
        conditionType["Energy"] = ConditionType.Energy;
        conditionType["Bleeding"] = ConditionType.Bleeding;
        conditionType["Haste"] = ConditionType.Haste;
        conditionType["Paralyze"] = ConditionType.Paralyze;
        conditionType["Outfit"] = ConditionType.Outfit;
        conditionType["Invisible"] = ConditionType.Invisible;
        conditionType["Light"] = ConditionType.Light;
        conditionType["ManaShield"] = ConditionType.ManaShield;
        conditionType["InFight"] = ConditionType.InFight;
        conditionType["Drunk"] = ConditionType.Drunk;
        conditionType["ExhaustWeapon"] = ConditionType.ExhaustWeapon;
        conditionType["Regeneration"] = ConditionType.Regeneration;
        conditionType["Soul"] = ConditionType.Soul;
        conditionType["Drown"] = ConditionType.Drown;
        conditionType["Muted"] = ConditionType.Muted;
        conditionType["ChannelMutedTicks"] = ConditionType.ChannelMutedTicks;
        conditionType["YellTicks"] = ConditionType.YellTicks;
        conditionType["Attributes"] = ConditionType.Attributes;
        conditionType["Freezing"] = ConditionType.Freezing;
        conditionType["Dazzled"] = ConditionType.Dazzled;
        conditionType["Cursed"] = ConditionType.Cursed;
        conditionType["ExhaustCombat"] = ConditionType.ExhaustCombat;
        conditionType["ExhaustHeal"] = ConditionType.ExhaustHeal;
        conditionType["Pacified"] = ConditionType.Pacified;
        conditionType["Illusion"] = ConditionType.Illusion;
        conditionType["Hungry"] = ConditionType.Hungry;
    }
}