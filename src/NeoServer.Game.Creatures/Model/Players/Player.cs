  using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Creatures.Model;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Server.Model.Players
{
    public class Player : Creature, IPlayer
    {
        public Player(uint id, string characterName, ChaseMode chaseMode, uint capacity, ushort healthPoints, ushort maxHealthPoints, VocationType vocation,
            Gender gender, bool online, ushort mana, ushort maxMana, FightMode fightMode, byte soulPoints, uint maxSoulPoints, IDictionary<SkillType, ISkill> skills, ushort staminaMinutes,
            IOutfit outfit, IDictionary<Slot, Tuple<IPickupable, ushort>> inventory, ushort speed,
            Location location)
             : base(id, characterName, string.Empty, maxHealthPoints, maxMana, 4240, healthPoints, mana)
        {
            Id = id;
            CharacterName = characterName;
            ChaseMode = chaseMode;
            CarryStrength = capacity;
            HealthPoints = healthPoints;
            MaxHealthPoints = maxHealthPoints;
            Vocation = vocation;
            Gender = gender;
            Online = online;
            Mana = mana;
            MaxMana = maxMana;
            FightMode = fightMode;
            SoulPoints = soulPoints;
            MaxSoulPoints = maxSoulPoints;
            Skills = skills;
            StaminaMinutes = staminaMinutes;
            Outfit = outfit;
            Speed = speed;

            //Location = location;
            SetNewLocation(location);

            Containers = new PlayerContainerList(this);

            KnownCreatures = new Dictionary<uint, long>();//todo
            VipList = new Dictionary<string, bool>(); //todo

            //OnThingChanged += CheckInventoryContainerProximity;




            Inventory = new PlayerInventory(this, inventory);
        }


        public event CancelWalk OnCancelledWalk;

        private uint IdleTime;

        public uint Id { get; private set; }
        public string CharacterName { get; private set; }

        public Account Account { get; private set; }
        public override IOutfit Outfit { get; protected set; }

        public new uint Corpse => 4240;

        public IPlayerContainerList Containers { get; }

        public Dictionary<uint, long> KnownCreatures { get; }
        public Dictionary<string, bool> VipList { get; }

        public ChaseMode ChaseMode { get; private set; }
        public uint Capacity { get; private set; }
        public ushort Level => Skills[SkillType.Level].Level;
        public ushort HealthPoints { get; private set; }
        public ushort MaxHealthPoints { get; private set; }
        public VocationType Vocation { get; private set; }
        public Gender Gender { get; private set; }
        public bool Online { get; private set; }
        public ushort Mana { get; private set; }
        public ushort MaxMana { get; private set; }
        public FightMode FightMode { get; private set; }
        public byte SoulPoints { get; private set; }
        public uint MaxSoulPoints { get; private set; }
        public new IDictionary<SkillType, ISkill> Skills { get; private set; }

        public sealed override IInventory Inventory { get; protected set; }

        public ushort StaminaMinutes { get; private set; }

        public uint Experience => (uint)Skills[SkillType.Level].Count;
        public byte LevelPercent => GetSkillPercent(SkillType.Level);

        public bool IsMounted()
        {
            return false;
        }

        public string GetDescription(bool isYourself)
        {
            if (isYourself)
            {
                return $"You are {Vocation}";
            }
            return "";
        }

        public void ResetIdleTime()
        {
            IdleTime = 0;
        }

        /////
        ///

        public override string InspectionText => Name;

        public override string CloseInspectionText => InspectionText;
        public byte AccessLevel { get; set; } // TODO: implement.

        public bool CannotLogout => !Tile.ProtectionZone && InFight;

        public bool CanLogout
        {
            get
            {
                //todo inconnection validation

                if (Tile.CannotLogout)
                {
                    return false;
                }
                if (Tile.ProtectionZone)
                {
                    return true;
                }

                return !CannotLogout;
            }
        }

        public Location LocationInFront
        {

            get
            {
                switch (Direction)
                {
                    case Direction.North:
                        return new Location
                        {
                            X = Location.X,
                            Y = Location.Y - 1,
                            Z = Location.Z
                        };
                    case Direction.East:
                        return new Location
                        {
                            X = Location.X + 1,
                            Y = Location.Y,
                            Z = Location.Z
                        };
                    case Direction.West:
                        return new Location
                        {
                            X = Location.X - 1,
                            Y = Location.Y,
                            Z = Location.Z
                        };
                    case Direction.South:
                        return new Location
                        {
                            X = Location.X,
                            Y = Location.Y + 1,
                            Z = Location.Z
                        };
                    default:
                        return Location; // should not happen.
                }
            }
        }

        public override ushort AttackPower => Inventory.TotalAttack;

        public override ushort ArmorRating => Inventory.TotalArmor;

        public override ushort DefensePower => Inventory.TotalDefense;

        public override byte AutoAttackRange => Math.Max((byte)1, Inventory.AttackRange);

        public byte SecureMode { get; private set; }



        public byte GetSkillInfo(SkillType skill) => (byte)Skills[skill].Level;
        public byte GetSkillPercent(SkillType skill) => (byte)Skills[skill].Percentage;
        public bool KnowsCreatureWithId(uint creatureId) => KnownCreatures.ContainsKey(creatureId);
        public bool CanMoveThing(Location location) => Location.GetSqmDistance(location) <= MapConstants.MAX_DISTANCE_MOVE_THING;

        public void AddKnownCreature(uint creatureId)
        {
            try
            {
                KnownCreatures[creatureId] = DateTime.Now.Ticks;
            }
            catch
            {
                // happens when 2 try to add at the same time, which we don't care about.
            }
        }

        const int KnownCreatureLimit = 250; // TODO: not sure of the number for this version... debugs will tell :|
        public uint ChooseToRemoveFromKnownSet()
        {
            // if the buffer is full we need to choose a vitim.
            while (KnownCreatures.Count == KnownCreatureLimit)
            {
                foreach (var candidate in KnownCreatures.OrderBy(kvp => kvp.Value).ToList()) // .ToList() prevents modifiying an enumerating collection in the rare case we hit an exception down there.
                {
                    try
                    {
                        if (KnownCreatures.Remove(candidate.Key))
                        {
                            return candidate.Key;
                        }
                    }
                    catch
                    {
                        // happens when 2 try to remove time, which we don't care too much.
                    }
                }
            }

            return uint.MinValue; // 0
        }

        public void ChangeOutfit(IOutfit outfit) => Outfit = outfit;


        public override bool TryWalkTo(params Direction[] directions)
        {
            ResetIdleTime();
            return base.TryWalkTo(directions);
        }

        public void SetFightMode(FightMode mode)
        {
            FightMode = mode;
        }
        public void SetChaseMode(ChaseMode mode)
        {
            ChaseMode = mode;
        }

        public void SetSecureMode(byte mode)
        {
            SecureMode = mode;
        }

        public void SetDirection(Direction direction)
        {
            Direction = direction;
        }

        public void CancelWalk()
        {
            StopWalking();
            OnCancelledWalk(this);
        }
    }
}



