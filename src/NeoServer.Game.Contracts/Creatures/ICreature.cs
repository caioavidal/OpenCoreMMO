using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Creatures.Enums;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void OnCreatureStateChange();
    public delegate void OnTurnedToDirection(IWalkableCreature creature, Direction direction);
    public delegate void RemoveCreature(ICreature creature);
    public delegate void StopWalk(IWalkableCreature creature);
    public delegate void Die(ICombatActor creature);
    public delegate void GainExperience(ICreature creature, uint exp);
    public delegate void StartWalk(IWalkableCreature creature);
    public delegate void Heal(ICombatActor creature, ushort amount);
    public delegate void Say(ICreature creature, TalkType type, string message);
    public delegate void AddCondition(ICreature creature, ICondition condition);
    public delegate void RemoveCondition(ICreature creature, ICondition condition);
    public delegate void ChangeOutfit(ICreature creature, IOutfit outfit);

    public interface ICreature : IMoveableThing
    {
        BloodType Blood { get; }
        bool CanSeeInvisible { get; }
        Direction ClientSafeDirection { get; }
        IDictionary<ConditionType, ICondition> Conditions { get; set; }
        ushort CorpseType { get; }
        uint CreatureId { get; }
        Direction Direction { get; }
        uint Flags { get; }
        HashSet<uint> Friendly { get; }
        HashSet<uint> Hostiles { get; }
        bool IsInvisible { get; }
        bool IsRemoved { get; }
        byte LightBrightness { get; }
        byte LightColor { get; }
        IOutfit Outfit { get; }
        byte Shield { get; }
        byte Skull { get; }
        uint HealthPoints { get; }
        uint MaxHealthPoints { get; }
        bool IsHealthHidden { get; }
        IThing Corpse { get; set; }
        IOutfit OldOutfit { get; }
        IDynamicTile Tile { get; set; }
        bool CanBeSeen { get; }

        event RemoveCreature OnCreatureRemoved;
        event GainExperience OnGainedExperience;
        event Say OnSay;
        event RemoveCondition OnRemovedCondition;
        event ChangeOutfit OnChangedOutfit;
        event AddCondition OnAddedCondition;

        void AddCondition(ICondition condition);
        bool CanSee(Contracts.Creatures.ICreature otherCreature);
        bool CanSee(Location pos);
        void ChangeOutfit(ushort lookType, ushort id, byte head, byte body, byte legs, byte feet, byte addon);
        IItem CreateItem(ushort itemId, byte amount);
        void DisableTemporaryOutfit();
        void GainExperience(uint exp);
        bool HasCondition(ConditionType type, out ICondition condition);
        bool HasCondition(ConditionType type);
        bool HasFlag(CreatureFlag flag);
        void OnCreatureAppear(Location location, ICylinderSpectator[] spectators);
        void RemoveCondition(ICondition condition);
        void Say(string message, TalkType talkType);
        void SetAsRemoved();
        void SetDirection(Direction direction);
        void SetFlag(CreatureFlag flag);
        void SetTemporaryOutfit(ushort lookType, ushort id, byte head, byte body, byte legs, byte feet, byte addon);
        void UnsetFlag(CreatureFlag flag);
    }
}
