using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Creature.Model;
using NeoServer.Game.Creatures.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NeoServer.Game.Creatures.Model
{

    public abstract class Creature : IEquatable<Creature>, ICreature
    {
        public event RemoveCreature OnCreatureRemoved;
        public event GainExperience OnGainedExperience;
        public event AddCondition OnAddedCondition;
        public event RemoveCondition OnRemovedCondition;
        public event ChangeOutfit OnChangedOutfit;

        public event Say OnSay;

        protected readonly ICreatureType CreatureType;

        public Creature(ICreatureType type, IOutfit outfit = null, uint healthPoints = 0)
        {
            if (string.IsNullOrWhiteSpace(type.Name))
            {
                throw new ArgumentNullException(nameof(type.Name));
            }
            MaxHealthPoints = type.MaxHealth;
            HealthPoints = Math.Min(MaxHealthPoints, healthPoints == 0 ? MaxHealthPoints : healthPoints);

            CreatureType = type;

            CreatureId = RandomIdGenerator.Generate();
            Outfit = outfit ?? new Outfit()
            {
                LookType = type.Look[LookType.Type],
                Body = (byte)type.Look[LookType.Body],
                Feet = (byte)type.Look[LookType.Feet],
                Head = (byte)type.Look[LookType.Head],
                Legs = (byte)type.Look[LookType.Legs],
            };
            Hostiles = new HashSet<uint>();
            Friendly = new HashSet<uint>();
            MaxHealthPoints = type.MaxHealth;

        }

        private IDynamicTile tile;
        public IDynamicTile Tile
        {
            get
            {
                return tile;
            }
            set
            {
                tile = value;
                Location = tile.Location;
            }
        }
        public Action<ICreature> NextAction { get; protected set; }
        public uint HealthPoints { get; protected set; }
        public uint MaxHealthPoints { get; protected set; }
        public new string Name => CreatureType.Name;
        public uint CreatureId { get; }
        public ushort CorpseType => CreatureType.Look[LookType.Corpse];
        public IThing Corpse { get; set; }
        public virtual BloodType Blood => BloodType.Blood;
        public virtual bool CanBeSeen => true;
        public abstract IOutfit Outfit { get; protected set; }
        public IOutfit OldOutfit { get; private set; }
        public Direction Direction { get; protected set; }
        public IDictionary<ConditionType, ICondition> Conditions { get; set; } = new Dictionary<ConditionType, ICondition>();
        public Direction ClientSafeDirection
        {
            get
            {
                switch (Direction)
                {
                    case Direction.North:
                    case Direction.East:
                    case Direction.South:
                    case Direction.West:
                        return Direction;
                    case Direction.NorthEast:
                    case Direction.SouthEast:
                        return Direction.East;
                    case Direction.NorthWest:
                    case Direction.SouthWest:
                        return Direction.West;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void ChangeOutfit(ushort lookType, ushort id, byte head, byte body, byte legs, byte feet, byte addon)
        {
            OldOutfit = null;
            Outfit.Change(lookType, id, head, body, legs, feet, addon);

            OnChangedOutfit?.Invoke(this, Outfit);
        }
        public void SetTemporaryOutfit(ushort lookType, ushort id, byte head, byte body, byte legs, byte feet, byte addon)
        {
            OldOutfit = Outfit.Clone();
            Outfit.Change(lookType, id, head, body, legs, feet, addon);
            OnChangedOutfit?.Invoke(this, Outfit);
        }

        public void DisableTemporaryOutfit()
        {
            Outfit = OldOutfit;
            OldOutfit = null;
            OnChangedOutfit?.Invoke(this, Outfit);
        }
        public byte LightBrightness { get; protected set; }
        public byte LightColor { get; protected set; }
        public uint Flags { get; private set; }
        public bool IsInvisible { get; protected set; } // TODO: implement.
        public virtual bool CanSeeInvisible { get; }

        public HashSet<uint> Hostiles { get; }

        public HashSet<uint> Friendly { get; }
        public bool IsRemoved { get; private set; }

        public bool HasFlag(CreatureFlag flag)
        {
            var flagValue = (uint)flag;
            return (Flags & flagValue) == flagValue;
        }

        public void SetFlag(CreatureFlag flag) => Flags |= (uint)flag;
        public void UnsetFlag(CreatureFlag flag) => Flags &= ~(uint)flag;
        public virtual bool CanSee(ICreature otherCreature)
        {
            return !otherCreature.IsInvisible || CanSeeInvisible;
        }
        public void SetAsRemoved() => IsRemoved = true;
        public bool CanSee(Location pos, int viewPortX, int viewPortY)
        {
            if (Location.IsSurface || Location.IsAboveSurface)
            {
                if (pos.IsUnderground) return false;
            }
            else if (Location.IsUnderground)
            {
                if (Math.Abs(Location.Z - pos.Z) > 2) return false;
            }

            var offsetZ = Location.Z - pos.Z;

            if (pos.X >= Location.X - viewPortX + offsetZ && pos.X <= Location.X + viewPortX + offsetZ &&
                pos.Y >= Location.Y - viewPortY + offsetZ && pos.Y <= Location.Y + viewPortY + offsetZ)
            {
                return true;
            }

            return false;
        }

        public virtual void OnCreatureAppear(Location location, ICylinderSpectator[] spectators)
        {
            foreach (var cylinder in spectators)
            {
                var spectator = cylinder.Spectator;
                if (this == (Creature)spectator) continue;

                if (spectator is ICombatActor actor) actor.SetAsEnemy(this);
                if (this is ICombatActor a) a.SetAsEnemy(spectator);
            }
        }

        protected void ExecuteNextAction(ICreature creature)
        {
            NextAction?.Invoke(creature);
            NextAction = null;
        }
        public bool CanSee(Location pos)
        {
            var viewPortX = 9;
            var viewPortY = 7;

            if (Location.IsSurface || Location.IsAboveSurface)
            {
                if (pos.IsUnderground) return false;
            }
            else if (Location.IsUnderground)
            {
                if (Math.Abs(Location.Z - pos.Z) > 2) return false;
            }

            var offsetZ = Location.Z - pos.Z;

            if (pos.X >= Location.X - (viewPortX - 1) + offsetZ && pos.X <= Location.X + viewPortX + offsetZ &&
                pos.Y >= Location.Y - (viewPortY - 1) + offsetZ && pos.Y <= Location.Y + viewPortY + offsetZ)
            {
                return true;
            }

            return false;
        }


        public byte Skull { get; protected set; } // TODO: implement.

        public byte Shield { get; protected set; } // TODO: implement.
        public bool IsHealthHidden { get; protected set; }
        public Location Location { get; set; }

        public void SetDirection(Direction direction) => Direction = direction;

        public virtual void GainExperience(uint exp)
        {
            OnGainedExperience?.Invoke(this, exp);
        }

        public void AddCondition(ICondition condition)
        {
            var result = Conditions.TryAdd(condition.Type, condition);
            condition.Start(this);
            if (result == false) return;

            OnAddedCondition?.Invoke(this, condition);
        }
        public void RemoveCondition(ICondition condition)
        {
            Conditions.Remove(condition.Type);
            OnRemovedCondition?.Invoke(this, condition);
        }
        public void RemoveCondition(ConditionType type)
        {
            if (Conditions.Remove(type, out var condition) is false) return;
            OnRemovedCondition?.Invoke(this, condition);
        }
        public bool HasCondition(ConditionType type, out ICondition condition) => Conditions.TryGetValue(type, out condition);
        public bool HasCondition(ConditionType type) => Conditions.ContainsKey(type);

        public virtual void Say(string message, TalkType talkType)
        {
            OnSay?.Invoke(this, talkType, message);
        }
        public virtual IItem CreateItem(ushort itemId, byte amount)
        {
            throw new NotImplementedException();
            //var item = ItemFactory.Create(itemId, Location, null);
            //if (item is ICumulativeItem cumulativeItem) cumulativeItem.Increase((byte)(amount - 1));
            //return item;
        }

        public override bool Equals(object obj) => obj is ICreature creature && creature.CreatureId == CreatureId;

        public override int GetHashCode() => HashCode.Combine(CreatureId);

        public bool Equals([AllowNull] Creature other)
        {
            return this == other;
        }

        public void OnMoved() { }

        public static bool operator ==(Creature creature1, Creature creature2) => creature1.CreatureId == creature2.CreatureId;
        public static bool operator !=(Creature creature1, Creature creature2) => creature1.CreatureId != creature2.CreatureId;
    }
}
