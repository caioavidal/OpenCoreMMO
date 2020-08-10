using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Game.Creatures.Model;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Item;
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
        public Player(string characterName, ChaseMode chaseMode, float capacity, ushort healthPoints, ushort maxHealthPoints, VocationType vocation,
            Gender gender, bool online, ushort mana, ushort maxMana, FightMode fightMode, byte soulPoints, uint maxSoulPoints, IDictionary<SkillType, ISkill> skills, ushort staminaMinutes,
            IOutfit outfit, IDictionary<Slot, Tuple<IPickupable, ushort>> inventory, ushort speed,
            Location location)
             : base(new CreatureType(characterName, string.Empty, maxHealthPoints, speed, new Dictionary<LookType, ushort> { { LookType.Corpse, 4240 } }), outfit, healthPoints)
        {
            CharacterName = characterName;
            ChaseMode = chaseMode;
            CarryStrength = capacity;
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

        public string CharacterName { get; private set; }

        public Account Account { get; private set; }
        public override IOutfit Outfit { get; protected set; }

        public new uint Corpse => 4240;
        public IDictionary<SkillType, ISkill> Skills { get; private set; }

        public IPlayerContainerList Containers { get; }

        public Dictionary<uint, long> KnownCreatures { get; }
        public Dictionary<string, bool> VipList { get; }



        public ChaseMode ChaseMode { get; private set; }
        public uint Capacity { get; private set; }
        public ushort Level => Skills[SkillType.Level].Level;
        public ushort MaxHealthPoints { get; private set; }
        public VocationType Vocation { get; private set; }
        public Gender Gender { get; private set; }
        public bool Online { get; private set; }
        public ushort Mana { get; private set; }
        public ushort MaxMana { get; private set; }
        public FightMode FightMode { get; private set; }
        public byte SoulPoints { get; private set; }
        public uint MaxSoulPoints { get; private set; }

        public IInventory Inventory { get; set; }

        public ushort StaminaMinutes { get; private set; }

        public uint Experience => (uint)Skills[SkillType.Level].Count;
        public byte LevelPercent => GetSkillPercent(SkillType.Level);

        public bool IsMounted()
        {
            return false;
        }

        public void IncreaseSkillCounter(SkillType skill, ushort value)
        {
            if (!Skills.ContainsKey(skill))
            {
                // TODO: proper logging.
                Console.WriteLine($"CreatureId {Name} does not have the skill {skill} in it's skill set.");
            }

            Skills[skill].IncreaseCounter(value);
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

        public float DamageFactor => FightMode switch
        {
            FightMode.Attack => 1,
            FightMode.Balanced => 0.75f,
            FightMode.Defense => 0.5f,
            _ => 0.75f
        };

        public SkillType SkillInUse
        {
            get
            {
                if (Inventory.Weapon is IWeapon weapon)
                {
                    return weapon.Type switch
                    {
                        WeaponType.Club => SkillType.Club,
                        WeaponType.Sword => SkillType.Sword,
                        WeaponType.Axe => SkillType.Axe,
                        WeaponType.Ammunition => SkillType.Distance,
                        WeaponType.Distance => SkillType.Distance,
                        _ => SkillType.Fist
                    };
                }
                return SkillType.Fist;
            }
        }




        public override ushort AttackPower
        {
            get
            {

                return (ushort)(0.085f * DamageFactor * Inventory.TotalAttack * Skills[SkillInUse].Level + (Level / 5));
            }
        }

        public override ushort MinimumAttackPower => (ushort)(Level / 5);

        public override ushort ArmorRating => Inventory.TotalArmor;

        public override ushort DefensePower => Inventory.TotalDefense;

        public override byte AutoAttackRange => Math.Max((byte)1, Inventory.AttackRange);

        public byte SecureMode { get; private set; }
        public float CarryStrength { get; }

        public override bool UsingDistanceWeapon => Inventory.Weapon is IDistanceWeaponItem;

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

        public override int ShieldDefend(int attack)
        {
            throw new NotImplementedException();
        }

        public override int ArmorDefend(int attack)
        {
            throw new NotImplementedException();
        }
    }
}



