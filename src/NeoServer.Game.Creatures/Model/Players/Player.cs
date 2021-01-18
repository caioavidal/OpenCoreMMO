using NeoServer.Game.Common;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Conditions;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Game.Contracts.Items.Types.Useables;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Creatures.Model;
using NeoServer.Game.Creatures.Model.Bases;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.Creatures.Spells;
using NeoServer.Game.Creatures.Vocations;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Server.Model.Players
{
    public class Player : CombatActor, IPlayer
    {
        public Player(uint id, string characterName, ChaseMode chaseMode, uint capacity, ushort healthPoints, ushort maxHealthPoints, byte vocation,
            Gender gender, bool online, ushort mana, ushort maxMana, FightMode fightMode, byte soulPoints, byte soulMax, IDictionary<SkillType, ISkill> skills, ushort staminaMinutes,
            IOutfit outfit, IDictionary<Slot, Tuple<IPickupable, ushort>> inventory, ushort speed,
            Location location, IPathAccess pathAccess)
             : base(new CreatureType(characterName, string.Empty, maxHealthPoints, speed, new Dictionary<LookType, ushort> { { LookType.Corpse, 3058 } }), pathAccess, outfit, healthPoints)
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
            MaxSoulPoints = soulMax;
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
                skill.OnIncreaseSkillPoints += (skill) => OnGainedSkillPoint?.Invoke(this, skill);
            }
        }

        public event PlayerLevelAdvance OnLevelAdvanced;
        public event PlayerGainSkillPoint OnGainedSkillPoint;

        public event OperationFail OnOperationFailed;
        public event CancelWalk OnCancelledWalk;

        public event ReduceMana OnStatusChanged;
        public event CannotUseSpell OnCannotUseSpell;
        public event LookAt OnLookedAt;
        public event UseSpell OnUsedSpell;
        public event UseItem OnUsedItem;
        public event LogIn OnLoggedIn;
        public event LogOut OnLoggedOut;


        public void OnLevelAdvance(SkillType type, int fromLevel, int toLevel)
        {
            if (type == SkillType.Level)
            {
                var levelDiff = toLevel - fromLevel;
                MaxHealthPoints += (uint)(levelDiff * Vocation.GainHp);
                MaxMana += (ushort)(levelDiff * Vocation.GainMana);
                TotalCapacity += (uint)(levelDiff * Vocation.GainCap);
                ResetHealthPoints();
                ResetMana();
                ChangeSpeed(Speed);
            }
            OnLevelAdvanced?.Invoke(this, type, fromLevel, toLevel);
        }

        private uint IdleTime;
        public string CharacterName { get; private set; }
        public override IOutfit Outfit { get; protected set; }
        public IDictionary<SkillType, ISkill> Skills { get; private set; }
        public override ushort Speed => (ushort)(220 + (2 * (Level - 1))); //todo: remove hard code base speed
        public IPlayerContainerList Containers { get; }
        public bool HasDepotOpened => Containers.HasAnyDepotOpened;
        public Dictionary<uint, long> KnownCreatures { get; }
        public Dictionary<string, bool> VipList { get; }
        public IVocation Vocation => VocationStore.TryGetValue(VocationType, out var vocation) ? vocation : null;
        public ChaseMode ChaseMode { get; private set; }
        public uint TotalCapacity { get; private set; }
        public ushort Level => Skills[SkillType.Level].Level;
        public byte VocationType { get; private set; }
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
        public void ResetMana() => HealMana(MaxMana);
        public byte LevelPercent => GetSkillPercent(SkillType.Level);
        public void IncreaseSkillCounter(SkillType skill, uint value)
        {
            if (!Skills.ContainsKey(skill)) return;

            Skills[skill].IncreaseCounter(value);
        }

        public void ResetIdleTime() => IdleTime = 0;

        public override void GainExperience(uint exp)
        {
            if (exp == 0)
            {
                return;
            }

            IncreaseSkillCounter(SkillType.Level, exp);
            base.GainExperience(exp);
        }

        public bool CannotLogout => !(Tile?.ProtectionZone ?? false) && InFight;
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

        public uint Id { get; }
        public override ushort MinimumAttackPower => (ushort)(Level / 5);

        public override ushort ArmorRating => Inventory.TotalArmor;
        public byte SecureMode { get; private set; }
        public float CarryStrength => TotalCapacity - Inventory.TotalWeight;
        public bool IsPacified => Conditions.ContainsKey(ConditionType.Pacified);
        public override bool UsingDistanceWeapon => Inventory.Weapon is IDistanceWeaponItem;

        public string Guild { get; }
        public bool Recovering { get; private set; }

        public byte GetSkillInfo(SkillType skill) => (byte)Skills[skill].Level;
        public byte GetSkillPercent(SkillType skill) => (byte)Skills[skill].Percentage;
        public bool KnowsCreatureWithId(uint creatureId) => KnownCreatures.ContainsKey(creatureId);
        public bool CanMoveThing(Location location) => Location.GetSqmDistance(location) <= MapConstants.MAX_DISTANCE_MOVE_THING;


        public void AddKnownCreature(uint creatureId) => KnownCreatures.TryAdd(creatureId, DateTime.Now.Ticks);
        const int KnownCreatureLimit = 250; //todo: for version 8.60

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

        public override void OnMoved(IDynamicTile fromTile, IDynamicTile toTile, ICylinderSpectator[] spectators)
        {
            TogglePacifiedCondition(fromTile, toTile);
            Containers.CloseDistantContainers();
            base.OnMoved(fromTile, toTile, spectators);
        }
        public override bool CanSee(ICreature otherCreature)
        {
            return !otherCreature.IsInvisible || (otherCreature is IPlayer && otherCreature.CanBeSeen) || (CanSeeInvisible);
        }
        public override void TurnInvisible()
        {
            SetTemporaryOutfit(0, 0, 0, 0, 0, 0, 0);
            base.TurnInvisible();
        }
        public override void TurnVisible()
        {
            DisableTemporaryOutfit();
            base.TurnVisible();
        }
        public override void SetAsEnemy(ICreature creature)
        {
            if (creature is not IMonster) return;
            SetAsInFight();
        }
        public void SetAsInFight()
        {
            if (IsPacified) return;

            if (HasCondition(ConditionType.InFight, out var condition))
            {
                condition.Start(this);
                return;
            }
            AddCondition(new Condition(ConditionType.InFight, 60000));
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
            if (FollowCreature && AutoAttackTarget is not null)
            {
                StartFollowing(AutoAttackTarget as IWalkableCreature, PathSearchParams);
                return;
            }

            StopFollowing();
        }

        public void SetSecureMode(byte mode) => SecureMode = mode;

        public void CancelWalk()
        {
            StopWalking();
            OnCancelledWalk(this);
        }

        public override int ShieldDefend(int attack)
        {
            var resultDamage = (int)(attack - Inventory.TotalDefense * Skills[SkillType.Shielding].Level * (DefenseFactor / 100d) - (attack / 100d) * ArmorRating);
            if (resultDamage <= 0)
            {
                IncreaseSkillCounter(SkillType.Shielding, 1);
            }
            return resultDamage;
        }

        public override int ArmorDefend(int damage)
        {
            if (ArmorRating > 3)
            {
                var min = ArmorRating / 2;
                var max = (ArmorRating / 2) * 2 - 1;
                damage -= (ushort)GameRandom.Random.NextInRange(min, max);
            }
            else if (ArmorRating > 0)
            {
                --damage;
            }
            return damage;
        }
        public override bool OnAttack(ICombatActor enemy, out CombatAttackType combat)
        {
            combat = new CombatAttackType();
            var canUse = Inventory.Weapon?.Use(this, enemy, out combat) ?? false;

            if (canUse)
            {
                IncreaseSkillCounter(SkillInUse, 1);
            }

            return canUse;
        }
        public void SendMessageTo(IPlayer player)
        {   
        }

        public virtual bool CastSpell(string message)
        {
            if (SpellList.TryGet(message.Trim(), out var spell))
            {
                if (!spell.Invoke(this, message, out var error))
                {
                    OnCannotUseSpell?.Invoke(this, spell, error);
                    return true;
                }

                var talkType = TalkType.MonsterSay;

                Cooldowns.Start(CooldownType.Spell, 1000); //todo: 1000 should be a const

                if (spell.IncreaseSkill) IncreaseSkillCounter(SkillType.Magic, spell.Mana);

                if (!spell.ShouldSay) return true;

                base.Say(message, talkType);

                return true;
            }

            return false;
        }
        public override void Say(string message, TalkType talkType)
        {
            base.Say(message, talkType);
        }

        public bool HasEnoughMana(ushort mana) => Mana >= mana;
        public void ConsumeMana(ushort mana)
        {
            if (mana == 0) return;
            if (!HasEnoughMana(mana)) return;

            Mana -= mana;
            OnStatusChanged?.Invoke(this);
        }
        public bool HasEnoughLevel(ushort level) => Level >= level;

        public override IItem CreateItem(ushort itemId, byte amount)
        {
            var item = base.CreateItem(itemId, amount);
            if (!Inventory.BackpackSlot.AddItem(item).IsSuccess)
            {
                Tile.AddItem(item);
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

        public bool Logout(bool forced = false)
        {
            if (CannotLogout && forced == false)
            {
                OnOperationFailed?.Invoke(CreatureId, "You may not logout during or immediately after a fight");
                return false;
            }

            StopAttack();
            StopFollowing();
            StopWalking();
            Containers.CloseAll();

            OnLoggedOut?.Invoke(this);
            return true;
        }
        public bool Login()
        {
            StopAttack();
            StopFollowing();
            StopWalking();

            KnownCreatures.Clear();

            OnLoggedIn?.Invoke(this);
            return true;
        }
        public override bool CanBlock(DamageType damage)
        {
            if (!Inventory.HasShield) return false;
            return base.CanBlock(damage);
        }
        public void HealMana(ushort increasing)
        {
            if (increasing <= 0) return;

            if (Mana == MaxMana) return;

            Mana = Mana + increasing >= MaxMana ? MaxMana : (ushort)(Mana + increasing);
            OnStatusChanged?.Invoke(this);
        }
        public void HealSoul(ushort increasing)
        {
            if (increasing <= 0) return;

            if (SoulPoints == MaxSoulPoints) return;

            SoulPoints = SoulPoints + increasing >= MaxSoulPoints ? MaxSoulPoints : (byte)(SoulPoints + increasing);
            OnStatusChanged?.Invoke(this);
        }

        public void Recover()
        {
            if (Cooldowns.Expired(CooldownType.HealthRecovery)) Heal(Vocation.GainHpAmount);
            if (Cooldowns.Expired(CooldownType.ManaRecovery)) HealMana(Vocation.GainManaAmount);
            if (Cooldowns.Expired(CooldownType.SoulRecovery)) HealSoul(1);

            //todo: start these cooldowns when player logs in
            Cooldowns.Start(CooldownType.HealthRecovery, Vocation.GainHpTicks * 1000);
            Cooldowns.Start(CooldownType.ManaRecovery, Vocation.GainManaTicks * 1000);
            Cooldowns.Start(CooldownType.SoulRecovery, Vocation.GainSoulTicks * 1000);
        }

        public override void OnDamage(IThing enemy, CombatDamage damage)
        {
            if (damage.Type == DamageType.ManaDrain) ConsumeMana(damage.Damage);
            else
                ReduceHealth(damage);
        }
        public void Use(IUseable item)
        {
            item.Use(this);
        }
        public void Use(IUseableOn item, ICreature onCreature)
        {
            if (item is IItemRequirement requirement && !requirement.CanBeUsed(this))
            {
                OnOperationFailed?.Invoke(CreatureId, requirement.ValidationError);
                return;
            }
            var result = false;

            if (onCreature is ICombatActor enemy)
            {
                if (item is IUseableAttackOnCreature useableAttackOnCreature) result = Attack(enemy, useableAttackOnCreature);
                else if (item is IUseableOnCreature useableOnCreature) { useableOnCreature.Use(this, onCreature); result = true; }
                else if (item is IUseableOnTile useableOnTile) result = useableOnTile.Use(this, onCreature.Tile);
            }

            if (result) OnUsedItem?.Invoke(this, onCreature, item);
        }
        public void Use(IUseableOn item, IItem onItem)
        {
            if (item is IItemRequirement requirement && !requirement.CanBeUsed(this))
            {
                OnOperationFailed?.Invoke(CreatureId, requirement.ValidationError);
                return;
            }

            if (item is not IUseableOnItem useableOnItem) return;

            useableOnItem.Use(this, onItem);
            OnUsedItem?.Invoke(this, onItem, item);
        }
        public void Use(IUseableOn item, ITile onTile)
        {
            if (item is IItemRequirement requirement && !requirement.CanBeUsed(this))
            {
                OnOperationFailed?.Invoke(CreatureId, requirement.ValidationError);
                return;
            }

            if (onTile.TopItemOnStack is not IItem onItem) return;

            var result = false;

            if (item is IUseableAttackOnTile useableAttackOnTile) result = Attack(onTile, useableAttackOnTile);
            else if (item is IUseableOnTile useableOnTile) result = useableOnTile.Use(this, onTile);
            else if (item is IUseableOnItem useableOnItem) result = useableOnItem.Use(this, onItem);

            if (result) OnUsedItem?.Invoke(this, onItem, item);
        }
        public bool Feed(IFood food)
        {
            if (food is null) return false;

            var regenerationMs = (uint)food.Duration * 1000;
            var maxRegenerationTime = (uint)1200 * 1000;

            if (Conditions.TryGetValue(ConditionType.Regeneration, out var condition))
            {
                if (condition.RemainingTime + regenerationMs >= maxRegenerationTime) //todo: this number should be configurable
                {
                    OnOperationFailed?.Invoke(CreatureId, "You are full");
                    return false;
                }

                condition.Extend(regenerationMs, maxRegenerationTime);
            }
            else
            {
                AddCondition(new Condition(ConditionType.Regeneration, regenerationMs, OnHungry));
            }

            Recovering = true;
            return true;
        }

        public void OnHungry() => Recovering = false;

        public Result MoveItem(IStore source, IStore destination, IItem thing, byte amount, byte fromPosition, byte? toPosition)
        {
            if (thing is not IPickupable) return Result.NotPossible;
            if (thing.Location.Type == LocationType.Ground && !Location.IsNextTo(thing.Location)) return new Result(InvalidOperation.TooFar);

            return source.SendTo(destination, thing, amount, fromPosition, toPosition).ResultValue;
        }

        public override void SetAttackTarget(ICreature target)
        {
            base.SetAttackTarget(target);
            if (target.CreatureId != 0 && ChaseMode == ChaseMode.Follow)
            {
                StartFollowing(target, PathSearchParams);
            }
        }
    }
}