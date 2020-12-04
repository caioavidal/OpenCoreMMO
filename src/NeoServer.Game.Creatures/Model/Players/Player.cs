using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Creatures.Model;
using NeoServer.Game.Creatures.Model.Bases;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.Creatures.Spells;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Common.Talks;
using NeoServer.Server.Helpers;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Conditions;
using NeoServer.Game.Creatures.Vocations;

namespace NeoServer.Server.Model.Players
{
    public class Player : CombatActor, IPlayer
    {
        public Player(uint id, string characterName, ChaseMode chaseMode, uint capacity, ushort healthPoints, ushort maxHealthPoints, VocationType vocation,
            Gender gender, bool online, ushort mana, ushort maxMana, FightMode fightMode, byte soulPoints, IDictionary<SkillType, ISkill> skills, ushort staminaMinutes,
            IOutfit outfit, IDictionary<Slot, Tuple<IPickupable, ushort>> inventory, ushort speed,
            Location location, PathFinder pathFinder)
             : base(new CreatureType(characterName, string.Empty, maxHealthPoints, speed, new Dictionary<LookType, ushort> { { LookType.Corpse, 3058 } }), pathFinder, outfit, healthPoints)
        {
            Id = id;
            CharacterName = characterName;
            ChaseMode = chaseMode;
            TotalCapacity = capacity;
            Inventory = new PlayerInventory(this, inventory);
            VocationType = vocation;
            Gender = gender;
            Online = online;
            Mana = mana;
            MaxMana = maxMana;
            FightMode = fightMode;
            MaxSoulPoints = Vocation.SoulMax;
            SoulPoints = soulPoints;
            Skills = skills;
            StaminaMinutes = staminaMinutes;
            Outfit = outfit;

            Location = location;

            Containers = new PlayerContainerList(this);

            KnownCreatures = new Dictionary<uint, long>();//todo
            VipList = new Dictionary<string, bool>(); //todo


            foreach (var skill in Skills.Values)
            {
                skill.OnAdvance += OnLevelAdvance;
            }
        }

        public event PlayerLevelAdvance OnLevelAdvanced;
        public event OperationFail OnOperationFailed;
        public event CancelWalk OnCancelledWalk;

        public event ReduceMana OnManaChanged;
        public event CannotUseSpell OnCannotUseSpell;
        public event LookAt OnLookedAt;
        public event UseSpell OnUsedSpell;

        public void OnLevelAdvance(SkillType type, int fromLevel, int toLevel)
        {
            var levelDiff = toLevel - fromLevel;
            MaxHealthPoints += (uint)(levelDiff * Vocation.GainHp);
            MaxMana += (ushort)(levelDiff * Vocation.GainMana);
            TotalCapacity += (uint)(levelDiff * Vocation.GainCap);
            ResetHealthPoints();
            ResetMana();
            ChangeSpeed(Speed);
            OnLevelAdvanced?.Invoke(this, type, fromLevel, toLevel);
        }

        private uint IdleTime;
        public string CharacterName { get; private set; }

        public Account Account { get; private set; }
        public override IOutfit Outfit { get; protected set; }

        public new uint Corpse => 4240;
        public IDictionary<SkillType, ISkill> Skills { get; private set; }
        public override ushort Speed => (ushort)(220 + Level);
        public IPlayerContainerList Containers { get; }
        public bool HasDepotOpened => Containers.HasAnyDepotOpened;
        public Dictionary<uint, long> KnownCreatures { get; }
        public Dictionary<string, bool> VipList { get; }
        public IVocation Vocation => VocationStore.TryGetValue(VocationType, out var vocation) ? vocation : null;
        public ChaseMode ChaseMode { get; private set; }
        public uint TotalCapacity { get; private set; }
        public ushort Level => Skills[SkillType.Level].Level;
        public VocationType VocationType { get; private set; }
        public Gender Gender { get; private set; }
        public bool Online { get; private set; }
        public ushort Mana { get; private set; }
        public ushort MaxMana { get; private set; }
        public FightMode FightMode { get; private set; }

        private byte soulPoints;
        public byte SoulPoints
        {
            get => soulPoints; 
            private set => soulPoints = value > MaxSoulPoints ? MaxSoulPoints : value;
        }

        public byte MaxSoulPoints { get; private set; }
        public IInventory Inventory { get; set; }

        public ushort StaminaMinutes { get; private set; }

        public uint Experience
        {
            get
            {
                if (Skills.TryGetValue(SkillType.Level, out ISkill skill))
                {
                    return (uint)skill.Count;
                }
                return 0;
            }

        }
        public void ResetMana() => Mana = MaxMana;
        public byte LevelPercent => GetSkillPercent(SkillType.Level);

        public bool IsMounted()
        {
            return false;
        }

        public void IncreaseSkillCounter(SkillType skill, uint value)
        {
            if (!Skills.ContainsKey(skill)) return;

            Skills[skill].IncreaseCounter(value);
        }

        public string GetDescription(bool isYourself)
        {
            if (isYourself)
            {
                return $"You are {VocationType}";
            }
            return "";
        }

        public void ResetIdleTime()
        {
            IdleTime = 0;
        }

        public override void GainExperience(uint exp)
        {
            if (exp == 0)
            {
                return;
            }

            IncreaseSkillCounter(SkillType.Level, exp);
            base.GainExperience(exp);
        }

        public byte AccessLevel { get; set; } // TODO: implement.

        public bool CannotLogout => !(Tile?.ProtectionZone ?? false) && InFight;

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
                            Y = (ushort)(Location.Y - 1),
                            Z = Location.Z
                        };
                    case Direction.East:
                        return new Location
                        {
                            X = (ushort)(Location.X + 1),
                            Y = Location.Y,
                            Z = Location.Z
                        };
                    case Direction.West:
                        return new Location
                        {
                            X = (ushort)(Location.X - 1),
                            Y = Location.Y,
                            Z = Location.Z
                        };
                    case Direction.South:
                        return new Location
                        {
                            X = Location.X,
                            Y = (ushort)(Location.Y + 1),
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
        public int DefenseFactor => FightMode switch
        {
            FightMode.Attack => 5,
            FightMode.Balanced => 7,
            FightMode.Defense => 10,
            _ => 7
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

        public ushort CalculateAttackPower(float attackRate, ushort attack) => (ushort)(attackRate * DamageFactor * attack * Skills[SkillInUse].Level + (Level / 5));

        public override ushort AttackPower
        {
            get
            {
                return (ushort)(0.085f * DamageFactor * Inventory.TotalAttack * Skills[SkillInUse].Level + MinimumAttackPower);
            }
        }
        public uint Id { get; }
        public override ushort MinimumAttackPower => (ushort)(Level / 5);

        public override ushort ArmorRating => Inventory.TotalArmor;

        public override ushort DefensePower => Inventory.TotalDefense;

        public override byte AutoAttackRange => Math.Max((byte)1, Inventory.AttackRange);

        public byte SecureMode { get; private set; }
        public float CarryStrength => TotalCapacity - Inventory.TotalWeight;
        public bool IsPacified => Conditions.ContainsKey(ConditionType.Pacified);
        public override bool UsingDistanceWeapon => Inventory.Weapon is IDistanceWeaponItem;

        public string Guild { get; }

        public bool Recovering => true;

        public byte GetSkillInfo(SkillType skill) => (byte)Skills[skill].Level;
        public byte GetSkillPercent(SkillType skill) => (byte)Skills[skill].Percentage;
        public bool KnowsCreatureWithId(uint creatureId) => KnownCreatures.ContainsKey(creatureId);
        public bool CanMoveThing(Location location) => Location.GetSqmDistance(location) <= MapConstants.MAX_DISTANCE_MOVE_THING;

        public void AddKnownCreature(uint creatureId) => KnownCreatures.TryAdd(creatureId, DateTime.Now.Ticks);

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

        public override void OnMoved(IDynamicTile fromTile, IDynamicTile toTile)
        {
            TogglePacifiedCondition(fromTile, toTile);
            Containers.CloseDistantContainers();
            base.OnMoved(fromTile, toTile);
        }


        public override void SetAsInFight()
        {
            if (IsPacified) return;

            if (HasCondition(ConditionType.InFight, out var condition))
            {
                condition.Start(this);
                return;
            }
            AddCondition(new Condition(ConditionType.InFight, 60000));

            base.SetAsInFight();
        }

        private void TogglePacifiedCondition(IDynamicTile fromTile, IDynamicTile toTile)
        {
            if (fromTile.ProtectionZone is false && toTile.ProtectionZone is true)
            {
                RemoveCondition(ConditionType.InFight);
                AddCondition(new Condition(ConditionType.Pacified, 0));
            }

            if (fromTile.ProtectionZone is true && toTile.ProtectionZone is false) RemoveCondition(ConditionType.Pacified);
        }

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
            FollowCreature = mode == ChaseMode.Follow;
            if (FollowCreature)
            {
                // StartFollowing(AutoAttackTargetId);
                return;
            }

            StopFollowing();

        }

        public void SetSecureMode(byte mode)
        {
            SecureMode = mode;
        }

        public void CancelWalk()
        {
            StopWalking();
            OnCancelledWalk(this);
        }

        public override int ShieldDefend(int attack)
        {
            return (int)(attack - Inventory.TotalDefense * Skills[SkillType.Shielding].Level * (DefenseFactor / 100d) - (attack / 100d) * ArmorRating);
        }

        public override int ArmorDefend(int damage)
        {
            if (ArmorRating > 3)
            {
                var min = ArmorRating / 2;
                var max = (ArmorRating / 2) * 2 - 1;
                damage -= (ushort)ServerRandom.Random.NextInRange(min, max);
            }
            else if (ArmorRating > 0)
            {
                --damage;
            }
            return damage;
        }
        public void ReceiveManaAttack(ICreature enemy, ushort damage)
        {
            ConsumeMana(damage);
        }
        public override bool OnAttack(ICombatActor enemy, out CombatAttackType combat)
        {
            combat = new CombatAttackType();
            return Inventory.Weapon?.Use(this, enemy, out combat) ?? false;
        }

        public override void Say(string message, TalkType talkType)
        {
            if (SpellList.Spells.TryGetValue(message.Trim(), out var spell))
            {
                if (!spell.Invoke(this, out var error))
                {
                    OnCannotUseSpell?.Invoke(this, spell, error);
                    return;
                }

                Cooldowns.Start(CooldownType.Spell, 1000); //todo: 1000 should be a const
                                                           // OnUsedSpell?.Invoke(this, spell); //todo remove this event
            }
            base.Say(message, talkType);
        }

        public bool HasEnoughMana(ushort mana) => Mana >= mana;
        public void ConsumeMana(ushort mana)
        {
            if (mana == 0) return;
            if (!HasEnoughMana(mana)) return;

            Mana -= mana;
            OnManaChanged?.Invoke(this);
        }
        public bool HasEnoughLevel(ushort level) => Level >= level;

        public override IItem CreateItem(ushort itemId, byte amount)
        {
            var item = base.CreateItem(itemId, amount);
            if (!Inventory.BackpackSlot.TryAddItem(item).Success)
            {
                var thing = item as IThing;
                Tile.AddThing(thing);
            }
            return item;
        }
        public void LookAt(ITile tile)
        {
            var isClose = Location.IsNextTo(tile.Location);
            if (tile.TopCreatureOnStack is null && tile.TopItemOnStack is null) return;

            IThing thing = tile.TopCreatureOnStack is null ? tile.TopItemOnStack : tile.TopCreatureOnStack;
            OnLookedAt?.Invoke(this, thing, isClose);
        }
        public void LookAt(byte containerId, sbyte containerSlot)
        {
            if (Containers[containerId][containerSlot] is not IThing thing) return;
            OnLookedAt?.Invoke(this, thing, true);
        }
        public void LookAt(Slot slot)
        {
            if (Inventory[slot] is not IThing thing) return;
            OnLookedAt?.Invoke(this, thing, true);
        }

        public override CombatDamage OnImmunityDefense(CombatDamage damage)
        {
            return damage; //todo
        }

        public bool Logout()
        {
            if (CannotLogout)
            {
                OnOperationFailed?.Invoke(CreatureId, "You may not logout during or immediately after a fight");
                return false;
            }

            StopAttack();
            StopFollowing();
            StopWalking();
            return true;

        }
        public override bool CanBlock(DamageType damage)
        {
            if (!Inventory.HasShield) return false;
            return base.CanBlock(damage);
        }
        public void HealMana(ushort increasing)
        {
            if (Mana <= 0) return;

            if (Mana == MaxMana) return;

            Mana = Mana + increasing >= MaxMana ? MaxMana : (ushort)(Mana + increasing);
            OnManaChanged?.Invoke(this);
        }

        public void Recover()
        {
            if (Cooldowns.Expired(CooldownType.HealthRecovery)) Heal(Vocation.GainHpAmount);
            if (Cooldowns.Expired(CooldownType.ManaRecovery)) HealMana(Vocation.GainManaAmount);

            Cooldowns.Start(CooldownType.HealthRecovery, Vocation.GainHpTicks * 1000);
            Cooldowns.Start(CooldownType.ManaRecovery, Vocation.GainManaTicks * 1000);
        }
    }
}

