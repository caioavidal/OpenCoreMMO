using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using System;
using System.Diagnostics.CodeAnalysis;

namespace NeoServer.Game.Creatures.Model.Bases
{
    public abstract class Creature : IEquatable<Creature>, ICreature
    {
        protected readonly ICreatureType CreatureType;

        private IDynamicTile tile;

        public Creature(ICreatureType type, IOutfit outfit = null, uint healthPoints = 0)
        {
            if (string.IsNullOrWhiteSpace(type.Name)) throw new ArgumentNullException(nameof(type.Name));
            MaxHealthPoints = type.MaxHealth;
            HealthPoints = Math.Min(MaxHealthPoints, healthPoints == 0 ? MaxHealthPoints : healthPoints);

            CreatureType = type;

            CreatureId = RandomCreatureIdGenerator.Generate(this);
            Outfit = outfit ?? new Outfit
            {
                LookType = type.Look[LookType.Type],
                Body = (byte) type.Look[LookType.Body],
                Feet = (byte) type.Look[LookType.Feet],
                Head = (byte) type.Look[LookType.Head],
                Legs = (byte) type.Look[LookType.Legs]
            };
            MaxHealthPoints = type.MaxHealth;
        }

        public Action<ICreature> NextAction { get; protected set; }
        public event RemoveCreature OnCreatureRemoved;

        public event ChangeOutfit OnChangedOutfit;

        public event Say OnSay;

        public IDynamicTile Tile
        {
            get => tile;
            protected set
            {
                tile = value;
                Location = tile.Location;
            }
        }

        public uint HealthPoints { get; protected set; }
        public uint MaxHealthPoints { get; protected set; }
        public string Name => CreatureType.Name;
        public string GetLookText(bool isClose = false)
        {
            return $"You see {(isClose ? CloseInspectionText : InspectionText)}";
        }

        protected virtual string InspectionText => $"{Name}.";
        protected virtual string CloseInspectionText => $"{Name}.";

        public uint CreatureId { get; }
        public ushort CorpseType => CreatureType.Look[LookType.Corpse];
        public IThing Corpse { get; set; }
        public virtual BloodType BloodType => BloodType.Blood;
        public abstract bool CanBeSeen { get; }
        public abstract IOutfit Outfit { get; protected set; }
        public IOutfit LastOutfit { get; private set; }
        public Direction Direction { get; protected set; }
        public Direction LastDirection { get; protected set; }

        public Direction SafeDirection
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
                        return LastDirection;
                }
            }
        }

        public void ChangeOutfit(ushort lookType, ushort id, byte head, byte body, byte legs, byte feet, byte addon)
        {
            LastOutfit = null;
            Outfit.Change(lookType, id, head, body, legs, feet, addon);

            OnChangedOutfit?.Invoke(this, Outfit);
        }

        public void SetTemporaryOutfit(ushort lookType, ushort id, byte head, byte body, byte legs, byte feet,
            byte addon)
        {
            LastOutfit = Outfit.Clone();
            Outfit.Change(lookType, id, head, body, legs, feet, addon);
            OnChangedOutfit?.Invoke(this, Outfit);
        }

        public void BackToOldOutfit()
        {
            Outfit = LastOutfit;
            LastOutfit = null;
            OnChangedOutfit?.Invoke(this, Outfit);
        }

        public byte LightBrightness { get; protected set; }
        public byte LightColor { get; protected set; }
        public bool IsInvisible { get; protected set; } // TODO: implement.
        public abstract bool CanSeeInvisible { get; }

        public virtual bool CanSee(ICreature otherCreature)
        {
            return !otherCreature.IsInvisible || CanSeeInvisible;
        }

        public virtual void OnCreatureAppear(Location location, ICylinderSpectator[] spectators)
        {
            foreach (var cylinder in spectators)
            {
                var spectator = cylinder.Spectator;
                if (this == (Creature) spectator) continue;

                if (spectator is ICombatActor actor) actor.SetAsEnemy(this);
                if (this is ICombatActor a) a.SetAsEnemy(spectator);
            }
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
                return true;

            return false;
        }

        public byte Skull { get; protected set; } // TODO: implement.

        public virtual byte Emblem { get; } // TODO: implement.
        public bool IsHealthHidden { get; protected set; }
        public Location Location { get; set; }

        public virtual void Say(string message, SpeechType talkType, ICreature receiver = null)
        {
            if (string.IsNullOrWhiteSpace(message) || talkType == SpeechType.None) return;
            OnSay?.Invoke(this, talkType, message, receiver);
        }

        public void OnMoved()
        {
        }

        public bool Equals([AllowNull] Creature other)
        {
            return this == other;
        }

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
                return true;

            return false;
        }

        protected void ExecuteNextAction(ICreature creature)
        {
            NextAction?.Invoke(creature);
            NextAction = null;
        }

        protected void SetDirection(Direction direction)
        {
            // LastDirection should only remember actual directions, so we're ignoring 'none'.
            LastDirection = Direction == Direction.None ? LastDirection : Direction;
            Direction = direction;
        }

        public override bool Equals(object obj)
        {
            return obj is ICreature creature && creature.CreatureId == CreatureId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CreatureId);
        }

        public static bool operator ==(Creature creature1, Creature creature2)
        {
            return creature1.CreatureId == creature2.CreatureId;
        }

        public static bool operator !=(Creature creature1, Creature creature2)
        {
            return creature1.CreatureId != creature2.CreatureId;
        }
    }
}