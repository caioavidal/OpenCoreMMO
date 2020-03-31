using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creature.Model;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Model;

namespace NeoServer.Game.Creatures.Model
{
    public abstract class Creature : Thing, ICreature, ICombatActor
    {
        private static readonly object _idLock = new object();
        private static uint _idCounter = 1;

        private readonly object _enqueueWalkLock;

        protected Creature(
            uint id,
            string name,
            string article,
            uint maxHitpoints,
            uint maxManapoints,
            ushort corpse = 0,
            ushort hitpoints = 0,
            ushort manapoints = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (maxHitpoints == 0)
            {
                throw new ArgumentException($"{nameof(maxHitpoints)} must be positive.");
            }

            _enqueueWalkLock = new object();

            CreatureId = id;
            Name = name;
            Article = article;
            MaxHitpoints = maxHitpoints;
            Hitpoints = Math.Min(MaxHitpoints, hitpoints == 0 ? MaxHitpoints : hitpoints);
            MaxManapoints = maxManapoints;
            Manapoints = Math.Min(MaxManapoints, manapoints);
            Corpse = corpse;

            Cooldowns = new Dictionary<CooldownType, Tuple<DateTime, TimeSpan>>
            {
                { CooldownType.Move, new Tuple<DateTime, TimeSpan>(DateTime.Now, TimeSpan.Zero) },
                { CooldownType.Action, new Tuple<DateTime, TimeSpan>(DateTime.Now, TimeSpan.Zero) },
                { CooldownType.Combat, new Tuple<DateTime, TimeSpan>(DateTime.Now, TimeSpan.Zero) },
                { CooldownType.Talk, new Tuple<DateTime, TimeSpan>(DateTime.Now, TimeSpan.Zero) }
            };

            Outfit = new Outfit
            {
                Id = 0,
                LookType = 0
            };

            WalkingQueue = new ConcurrentQueue<Tuple<byte, Direction>>();

            // Subscribe any attack-impacting conditions here
            OnThingChanged += CheckAutoAttack;         // Are we in range with our target now/still?
            OnThingChanged += CheckPendingActions;                    // Are we in range with our pending action?
            // OnTargetChanged += CheckAutoAttack;          // Are we attacking someone new / not attacking anymore?
            // OnInventoryChanged += Mind.AttackConditionsChanged;        // Equipped / DeEquiped something?
            Skills = new Dictionary<SkillType, ISkill>();
            Hostiles = new HashSet<uint>();
            Friendly = new HashSet<uint>();
        }


        
        public event OnAttackTargetChange OnTargetChanged;

        public override ushort ThingId => CreatureThingId;

        public override byte Count => 0x01;

        public override string InspectionText => $"{Article} {Name}";

        public override string CloseInspectionText => InspectionText;

        public uint ActorId => CreatureId;

        public uint CreatureId { get; }

        public string Article { get; }

        public string Name { get; }

        public ushort Corpse { get; }

        public uint Hitpoints { get; }

        public uint MaxHitpoints { get; }

        public uint Manapoints { get; }

        public uint MaxManapoints { get; }

        public decimal CarryStrength { get; protected set; }

        public abstract IOutfit Outfit { get; protected set; }

        public Direction Direction { get; protected set; }

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


        public byte LightBrightness { get; protected set; }

        public byte LightColor { get; protected set; }

        public ushort Speed { get; protected set; } = 220;

        public uint Flags { get; private set; }

        public BloodType Blood { get; protected set; } // TODO: implement.

        public abstract ushort AttackPower { get; }

        public abstract ushort ArmorRating { get; }

        public abstract ushort DefensePower { get; }

        public uint AutoAttackTargetId { get; protected set; }

        public abstract byte AutoAttackRange { get; }

        public byte AutoAttackCredits { get; }

        public byte AutoDefenseCredits { get; }

        public decimal BaseAttackSpeed { get; }

        public decimal BaseDefenseSpeed { get; }

        public TimeSpan CombatCooldownTimeRemaining => throw new NotImplementedException(); //TODO
                                                                                            //CalculateRemainingCooldownTime(CooldownType.Combat, Game.Instance.CombatSynchronizationTime);

        public DateTime LastAttackTime => Cooldowns[CooldownType.Combat].Item1;

        public TimeSpan LastAttackCost => Cooldowns[CooldownType.Combat].Item2;

        public IDictionary<SkillType, ISkill> Skills { get; }

        public IDictionary<CooldownType, Tuple<DateTime, TimeSpan>> Cooldowns { get; }

        // public IList<Condition> Conditions { get; protected set; } // TODO: implement.
        public bool IsInvisible { get; protected set; } // TODO: implement.

        public bool CanSeeInvisible { get; } // TODO: implement.

        public byte Skull { get; protected set; } // TODO: implement.

        public byte Shield { get; protected set; } // TODO: implement.

        public ConcurrentQueue<Tuple<byte, Direction>> WalkingQueue { get; }

        public byte NextStepId { get; set; }

        public HashSet<uint> Hostiles { get; }

        public HashSet<uint> Friendly { get; }

        public abstract IInventory Inventory { get; protected set; }

        public static uint GetNewId()
        {
            lock (_idLock)
            {
                return _idCounter++; // we got 2^32 ids to give per game run... enough!
            }
        }

        protected virtual void CheckPendingActions(IThing thingChanged, IThingStateChangedEventArgs eventAgrs) { }

        // ~CreatureId()
        // {
        //    OnLocationChanged -= CheckAutoAttack;         // Are we in range with our target now/still?
        //    OnLocationChanged -= CheckPendingActions;                  // Are we in range with any of our pending actions?
        //    //OnTargetChanged -= CheckAutoAttack;           // Are we attacking someone new / not attacking anymore?
        //    //OnInventoryChanged -= Mind.AttackConditionsChanged;      // Equipped / DeEquiped something?
        // }
        public void IncreaseSkillCounter(SkillType skill, ushort value)
        {
            if (!Skills.ContainsKey(skill))
            {
                // TODO: proper logging.
                Console.WriteLine($"CreatureId {Name} does not have the skill {skill} in it's skill set.");
            }

            Skills[skill].IncreaseCounter(value);
        }

        public bool HasFlag(CreatureFlag flag)
        {
            var flagValue = (uint)flag;

            return (Flags & flagValue) == flagValue;
        }

        public void SetFlag(CreatureFlag flag)
        {
            Flags |= (uint)flag;
        }

        public void UnsetFlag(CreatureFlag flag)
        {
            Flags &= ~(uint)flag;
        }

        public bool CanSee(ICreature otherCreature)
        {
            return !otherCreature.IsInvisible || CanSeeInvisible;
        }

        public bool CanSee(Location pos)
        {
            if (Location.Z <= 7)
            {
                // we are on ground level or above (7 -> 0)
                // view is from 7 -> 0
                if (pos.Z > 7)
                {
                    return false;
                }
            }
            else if (Location.Z >= 8)
            {
                // we are underground (8 -> 15)
                // view is +/- 2 from the floor we stand on
                if (Math.Abs(Location.Z - pos.Z) > 2)
                {
                    return false;
                }
            }

            var offsetZ = Location.Z - pos.Z;

            if (pos.X >= Location.X - 8 + offsetZ && pos.X <= Location.X + 9 + offsetZ &&
                pos.Y >= Location.Y - 6 + offsetZ && pos.Y <= Location.Y + 7 + offsetZ)
            {
                return true;
            }

            return false;
        }

        public void TurnToDirection(Direction direction)
        {
            Direction = direction;
        }

        public void SetAttackTarget(uint targetId)
        {
            throw new NotImplementedException(); //TODO
            //if (targetId == CreatureId || AutoAttackTargetId == targetId)
            //{
            //    // if we want to attack ourselves or if the current target is already the one we want... no change needed.
            //    return;
            //}

            //// save the previus target to report
            //var oldTargetId = AutoAttackTargetId;

            //if (targetId == 0)
            //{
            //    // clearing our target.
            //    if (AutoAttackTargetId != 0)
            //    {
            //        var attackTarget = Game.Instance.GetCreatureWithId(AutoAttackTargetId);

            //        if (attackTarget != null)
            //        {
            //            attackTarget.OnThingChanged -= CheckAutoAttack;
            //        }

            //        AutoAttackTargetId = 0;
            //    }
            //}
            //else
            //{
            //    // TODO: verify against Hostiles.Union(Friendly).
            //    // if (creature != null)
            //    // {
            //    AutoAttackTargetId = targetId;

            //    var attackTarget = Game.Instance.GetCreatureWithId(AutoAttackTargetId);

            //    if (attackTarget != null)
            //    {
            //        attackTarget.OnThingChanged += CheckAutoAttack;
            //    }

            //    // }
            //    // else
            //    // {
            //    //    Console.WriteLine("Taget creature not found in attacker\'s view.");
            //    // }
            //}

            //// report the change to our subscribers.
            //OnTargetChanged?.Invoke(oldTargetId, targetId);
            //CheckAutoAttack(this, new ThingStateChangedEventArgs() { PropertyChanged = nameof(location) });
        }

        public void UpdateLastAttack(TimeSpan exahust)
        {
            throw new NotImplementedException(); //TODO

            //Cooldowns[CooldownType.Combat] = new Tuple<DateTime, TimeSpan>(Game.Instance.CombatSynchronizationTime, exahust);
        }

        public void CheckAutoAttack(IThing thingChanged, IThingStateChangedEventArgs eventAgrs)
        {

            if (AutoAttackTargetId == 0)
            {
                return;
            }

            throw new NotImplementedException();
            //var attackTarget = Game.Instance.GetCreatureWithId(AutoAttackTargetId); //todo

            //if (attackTarget == null || (thingChanged != this && thingChanged != attackTarget) || eventAgrs.PropertyChanged != nameof(Thing.Location))
            //{
            //    return;
            //}

            //var locationDiff = Location - attackTarget.Location;
            //var inRange = CanSee(attackTarget) && locationDiff.Z == 0 && locationDiff.MaxValueIn2D <= AutoAttackRange;

            //if (inRange)
            //{
            //    //Game.Instance.SignalAttackReady(); //todo
            //}
        }

        public void StopWalking()
        {
            lock (_enqueueWalkLock)
            {
                WalkingQueue.Clear(); // reset the actual queue
                UpdateLastStepInfo(0);
            }
        }

        public void AutoWalk(params Direction[] directions)
        {
            throw new NotImplementedException(); //TODO

            //lock (_enqueueWalkLock)
            //{
            //    if (WalkingQueue.Count > 0)
            //    {
            //        StopWalking();
            //    }

            //    var nextStepId = NextStepId;

            //    foreach (var direction in directions)
            //    {
            //        WalkingQueue.Enqueue(new Tuple<byte, Direction>((byte)(nextStepId++ % byte.MaxValue), direction));
            //    }

            //    Game.Instance.SignalWalkAvailable();
            //}
        }

        public TimeSpan CalculateRemainingCooldownTime(CooldownType type, DateTime currentTime)
        {
            TimeSpan timeDiff = TimeSpan.Zero;

            try
            {
                timeDiff = Cooldowns[type].Item1 + Cooldowns[type].Item2 - currentTime;
            }
            catch
            {
                Console.WriteLine($"WARN: cooldown type {type} not found in {Name}'s cooldowns.");
            }

            return timeDiff > TimeSpan.Zero ? timeDiff : TimeSpan.Zero;
        }

        public void UpdateLastStepInfo(byte lastStepId, bool wasDiagonal = true)
        {
            var tilePenalty = Tile?.Ground?.MovementPenalty;
            var totalPenalty = (tilePenalty ?? 200) * (wasDiagonal ? 2 : 1);

            Cooldowns[CooldownType.Move] = new Tuple<DateTime, TimeSpan>(DateTime.Now, TimeSpan.FromMilliseconds(1000 * totalPenalty / (double)Math.Max(1, (int)Speed)));

            NextStepId = (byte)(lastStepId + 1);
        }
    }
}
