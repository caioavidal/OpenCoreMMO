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
    public delegate void Die(ICombatActor creature, IThing by);
    public delegate void GainExperience(ICreature creature, uint exp);
    public delegate void StartWalk(IWalkableCreature creature);
    public delegate void Heal(ICombatActor creature, ushort amount);
    public delegate void Say(ICreature creature, SpeechType type, string message, ICreature receiver = null);
    public delegate void AddCondition(ICreature creature, ICondition condition);
    public delegate void RemoveCondition(ICreature creature, ICondition condition);
    public delegate void ChangeOutfit(ICreature creature, IOutfit outfit);

    public interface ICreature : IMoveableThing
    {
        /// <summary>
        /// Fires when creature is removed from game
        /// </summary>
        event RemoveCreature OnCreatureRemoved;
        /// <summary>
        /// Fires when creature says something
        /// </summary>
        event Say OnSay;
     
        /// <summary>
        /// Fires when creature changes outfit
        /// </summary>
        event ChangeOutfit OnChangedOutfit;
        /// <summary>
        /// Creature's Blood Type. Ex: Slime, blood, fire ...
        /// </summary>
        BloodType BloodType { get; }
        /// <summary>
        /// Checks if creature can see invisible creatures
        /// </summary>
        bool CanSeeInvisible { get; }
        /// <summary>
        /// Translates directions to only South ,North, East or West direction;
        /// NorthEast and SouthEast to: East;
        /// NorthWest and SouthWest to West 
        /// </summary>
        Direction SafeDirection { get; }
        /// <summary>
        /// Corpse Type Id
        /// </summary>
        ushort CorpseType { get; }
        /// <summary>
        /// Random Creature Id
        /// </summary>
        uint CreatureId { get; }
        /// <summary>
        /// Player Direction: North, South, East and West
        /// </summary>
        Direction Direction { get; }
        /// <summary>
        /// Checks if Creature is invisible
        /// </summary>
        bool IsInvisible { get; }
        /// <summary>
        /// Creature's light level
        /// </summary>
        byte LightBrightness { get; }
        /// <summary>
        /// Creature's light color
        /// </summary>
        byte LightColor { get; }
        /// <summary>
        /// Creature's outfit
        /// </summary>
        IOutfit Outfit { get; }
        /// <summary>
        /// Creature's Emblem
        /// </summary>
        byte Emblem { get; }
        /// <summary>
        /// Indicates Skull showed on creature
        /// </summary>
        byte Skull { get; }
        /// <summary>
        /// HP
        /// </summary>
        uint HealthPoints { get; }
        /// <summary>
        /// Maximum HP
        /// </summary>
        uint MaxHealthPoints { get; }
        /// <summary>
        /// Indicates if HP is displayed
        /// </summary>
        bool IsHealthHidden { get; }
        /// <summary>
        /// Corpse instance
        /// </summary>
        IThing Corpse { get; set; }
        /// <summary>
        /// Last outfit creature used
        /// </summary>
        IOutfit LastOutfit { get; }
        /// <summary>
        /// Tile which creature is on
        /// </summary>
        IDynamicTile Tile { get; }
        /// <summary>
        /// Checks if creature can be seen by others
        /// </summary>
        bool CanBeSeen { get; }
   
        /// <summary>
        /// Checks if creature can see other creature
        /// </summary>                
        bool CanSee(ICreature otherCreature);
        /// <summary>
        /// Checks if creature can see location
        /// </summary>
        /// <returns></returns>
        bool CanSee(Location pos);
        /// <summary>
        /// Change creature outfit
        /// </summary>
        void ChangeOutfit(ushort lookType, ushort id, byte head, byte body, byte legs, byte feet, byte addon);
        /// <summary>
        /// Set old outfit to current
        /// </summary>
        void BackToOldOutfit();
     
        void OnCreatureAppear(Location location, ICylinderSpectator[] spectators);
    
        /// <summary>
        /// Says a message
        /// </summary>
        void Say(string message, SpeechType talkType, ICreature receiver = null);
        
  
        /// <summary>
        /// Sets new outfit and store current as last outfit
        /// </summary>
        void SetTemporaryOutfit(ushort lookType, ushort id, byte head, byte body, byte legs, byte feet, byte addon);
    }
}
