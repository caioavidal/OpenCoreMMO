﻿namespace NeoServer.Scripts.LuaJIT;

// Enums
public enum LuaDataType : byte
{
    Unknown,
	    Item,
	    Container,
	    Teleport,
	    Player,
	    Monster,
	    Npc,
	    MonsterType,
	    NpcType,
	    Tile,
	    Variant,
	    Position,
	    NetworkMessage,
	    ModalWindow,
	    Guild,
	    Group,
	    Vocation,
	    Town,
	    House,
	    ItemType,
	    Combat,
	    Condition,
	    Charm,
	    Loot,
	    MonsterSpell,
	    Spell,
	    Party,
	    Action,
	    TalkAction,
	    CreatureEvent,
	    MoveEvent,
	    GlobalEvent,
	    Weapon,
	    Imbuement,
	    Mount,
	    ItemClassification,
};

public enum CreatureEventType
{
    CREATURE_EVENT_NONE,
    CREATURE_EVENT_LOGIN,
    CREATURE_EVENT_LOGOUT,
    CREATURE_EVENT_THINK,
    CREATURE_EVENT_PREPAREDEATH,
    CREATURE_EVENT_DEATH,
    CREATURE_EVENT_KILL,
    CREATURE_EVENT_ADVANCE,
    CREATURE_EVENT_MODALWINDOW,
    CREATURE_EVENT_TEXTEDIT,
    CREATURE_EVENT_HEALTHCHANGE,
    CREATURE_EVENT_MANACHANGE,
    // Otclient additional network opcodes.
    CREATURE_EVENT_EXTENDED_OPCODE,
};

public enum MoveEventType
{
    MOVE_EVENT_STEP_IN,
    MOVE_EVENT_STEP_OUT,
    MOVE_EVENT_EQUIP,
    MOVE_EVENT_DEEQUIP,
    MOVE_EVENT_ADD_ITEM,
    MOVE_EVENT_REMOVE_ITEM,
    MOVE_EVENT_ADD_ITEM_ITEMTILE,
    MOVE_EVENT_REMOVE_ITEM_ITEMTILE,

    MOVE_EVENT_LAST,
    MOVE_EVENT_NONE
};

public enum RaidStateType
{
    RAIDSTATE_IDLE,
    RAIDSTATE_EXECUTING,
};

public enum TalkActionResultType
{
    TALKACTION_CONTINUE,
    TALKACTION_BREAK,
    TALKACTION_FAILED,
};

public enum GlobalEventType
{
    GLOBALEVENT_NONE,
    GLOBALEVENT_TIMER,

    GLOBALEVENT_STARTUP,
    GLOBALEVENT_SHUTDOWN,
    GLOBALEVENT_RECORD,
    GLOBALEVENT_PERIODCHANGE,
    GLOBALEVENT_ON_THINK,
};

public enum ModuleTypeType
{
    MODULE_TYPE_RECVBYTE,
    MODULE_TYPE_NONE,
};

public enum LuaVariantType
{
    VARIANT_NONE,

    VARIANT_NUMBER,
    VARIANT_POSITION,
    VARIANT_TARGETPOSITION,
    VARIANT_STRING,
};

public enum ErrorCodeType
{
    LUA_ERROR_PLAYER_NOT_FOUND,
    LUA_ERROR_CREATURE_NOT_FOUND,
    LUA_ERROR_NPC_NOT_FOUND,
    LUA_ERROR_NPC_TYPE_NOT_FOUND,
    LUA_ERROR_MONSTER_NOT_FOUND,
    LUA_ERROR_MONSTER_TYPE_NOT_FOUND,
    LUA_ERROR_ITEM_NOT_FOUND,
    LUA_ERROR_THING_NOT_FOUND,
    LUA_ERROR_TILE_NOT_FOUND,
    LUA_ERROR_HOUSE_NOT_FOUND,
    LUA_ERROR_COMBAT_NOT_FOUND,
    LUA_ERROR_CONDITION_NOT_FOUND,
    LUA_ERROR_AREA_NOT_FOUND,
    LUA_ERROR_CONTAINER_NOT_FOUND,
    LUA_ERROR_VARIANT_NOT_FOUND,
    LUA_ERROR_VARIANT_UNKNOWN,
    LUA_ERROR_SPELL_NOT_FOUND,
    LUA_ERROR_ACTION_NOT_FOUND,
    LUA_ERROR_TALK_ACTION_NOT_FOUND,
    LUA_ERROR_ZONE_NOT_FOUND,
};

public enum TargetSearchTypeType
{
    TARGETSEARCH_DEFAULT,
    TARGETSEARCH_NEAREST,
    TARGETSEARCH_HP,
    TARGETSEARCH_DAMAGE,
    TARGETSEARCH_RANDOM
};

public enum MapMarkType
{
    MAPMARK_TICK = 0,
    MAPMARK_QUESTION = 1,
    MAPMARK_EXCLAMATION = 2,
    MAPMARK_STAR = 3,
    MAPMARK_CROSS = 4,
    MAPMARK_TEMPLE = 5,
    MAPMARK_KISS = 6,
    MAPMARK_SHOVEL = 7,
    MAPMARK_SWORD = 8,
    MAPMARK_FLAG = 9,
    MAPMARK_LOCK = 10,
    MAPMARK_BAG = 11,
    MAPMARK_SKULL = 12,
    MAPMARK_DOLLAR = 13,
    MAPMARK_REDNORTH = 14,
    MAPMARK_REDSOUTH = 15,
    MAPMARK_REDEAST = 16,
    MAPMARK_REDWEST = 17,
    MAPMARK_GREENNORTH = 18,
    MAPMARK_GREENSOUTH = 19,
};

public enum RuleViolationTypeType : byte
{
    REPORT_TYPE_NAME = 0,
    REPORT_TYPE_STATEMENT = 1,
    REPORT_TYPE_BOT = 2
};

public enum RuleViolationReasonsType : byte
{
    REPORT_REASON_NAMEINAPPROPRIATE = 0,
    REPORT_REASON_NAMEPOORFORMATTED = 1,
    REPORT_REASON_NAMEADVERTISING = 2,
    REPORT_REASON_NAMEUNFITTING = 3,
    REPORT_REASON_NAMERULEVIOLATION = 4,
    REPORT_REASON_INSULTINGSTATEMENT = 5,
    REPORT_REASON_SPAMMING = 6,
    REPORT_REASON_ADVERTISINGSTATEMENT = 7,
    REPORT_REASON_UNFITTINGSTATEMENT = 8,
    REPORT_REASON_LANGUAGESTATEMENT = 9,
    REPORT_REASON_DISCLOSURE = 10,
    REPORT_REASON_RULEVIOLATION = 11,
    REPORT_REASON_STATEMENT_BUGABUSE = 12,
    REPORT_REASON_UNOFFICIALSOFTWARE = 13,
    REPORT_REASON_PRETENDING = 14,
    REPORT_REASON_HARASSINGOWNERS = 15,
    REPORT_REASON_FALSEINFO = 16,
    REPORT_REASON_ACCOUNTSHARING = 17,
    REPORT_REASON_STEALINGDATA = 18,
    REPORT_REASON_SERVICEATTACKING = 19,
    REPORT_REASON_SERVICEAGREEMENT = 20
};

public enum BugReportTypeType : byte
{
    BUG_CATEGORY_MAP = 0,
    BUG_CATEGORY_TYPO = 1,
    BUG_CATEGORY_TECHNICAL = 2,
    BUG_CATEGORY_OTHER = 3
};

// Struct
public struct LuaVariant
{
    public LuaVariantType Type = LuaVariantType.VARIANT_NONE;
    public string Text;
    public string InstantName;
    public string RuneName;
    //public Position Pos;
    public uint Number = 0;

    public LuaVariant()
    {
    }
}

public struct LuaTimerEventDesc
{
    public int ScriptId = -1;
    public string ScriptName;
    public int Function = -1;
    public List<int> Parameters;
    public string EventId = string.Empty;

    public LuaTimerEventDesc()
    {
        Parameters = new List<int>();
    }

    public LuaTimerEventDesc(int scriptId, string scriptName, int function, List<int> parameters, string eventId)
    {
        this.ScriptId = scriptId;
        this.ScriptName = scriptName;
        this.Function = function;
        this.Parameters = parameters;
        this.EventId = eventId;
    }
}