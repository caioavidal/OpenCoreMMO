
using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Common.Players;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Effects.Parsers;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Items
{
    public class ItemType : IItemType
    {
        /// <summary>
        /// Server Id
        /// </summary>
        public ushort TypeId { get; private set; }

        /// <summary>
        /// Item's name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Item's description
        /// </summary>
        public string Description => Attributes.GetAttribute(ItemAttribute.Description);

        public ISet<ItemFlag> Flags { get; }

        public IItemAttributeList Attributes { get; }
        public IItemAttributeList OnUse { get; private set; }
        public bool Locked { get; private set; }

        public ushort ClientId { get; private set; }
        public ushort TransformTo => Attributes.GetTransformationItem();

        public ItemTypeAttribute TypeAttribute { get; private set; }

        public ItemGroup Group { get; private set; }
        public ushort WareId { get; private set; }
        public LightBlock LightBlock { get; private set; }
        public ushort Speed => Attributes.GetAttribute<ushort>(ItemAttribute.Speed);
        public string Article { get; private set; }
        public string Plural { get; private set; }

        public float Weight => Attributes.GetAttribute<float>(ItemAttribute.Weight);

        void ThrowIfLocked()
        {
            if (Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }
        }
        public void SetGroup(byte type)
        {
            ThrowIfLocked();
            Group = (ItemGroup)type;

        }

        public void SetType(byte type)
        {
            ThrowIfLocked();
            switch ((ItemGroup)type)
            {
                case ItemGroup.GroundContainer:
                    TypeAttribute = ItemTypeAttribute.ITEM_TYPE_CONTAINER;
                    break;
                case ItemGroup.ITEM_GROUP_DOOR:
                    //not used
                    TypeAttribute = ItemTypeAttribute.ITEM_TYPE_DOOR;
                    break;
                case ItemGroup.ITEM_GROUP_MAGICFIELD:
                    //not used
                    TypeAttribute = ItemTypeAttribute.ITEM_TYPE_MAGICFIELD;
                    break;
                case ItemGroup.ITEM_GROUP_TELEPORT:
                    //not used
                    TypeAttribute = ItemTypeAttribute.ITEM_TYPE_TELEPORT;
                    break;
                case ItemGroup.None:
                case ItemGroup.Ground:
                case ItemGroup.Splash:
                case ItemGroup.ITEM_GROUP_FLUID:
                case ItemGroup.ITEM_GROUP_CHARGES:
                case ItemGroup.ITEM_GROUP_DEPRECATED:
                    break;
                default:
                    break;
            }
        }

        public void SetSpeed(ushort speed)
        {
            Attributes.SetAttribute(ItemAttribute.Speed, speed);
            ThrowIfLocked();
        }

        public ItemType()
        {
            TypeId = 0;
            Name = string.Empty;
            Flags = new HashSet<ItemFlag>();
            Attributes = new ItemAttributeList();
            Locked = false;
        }

        public void SetLight(LightBlock lightBlock)
        {
            ThrowIfLocked();
            LightBlock = lightBlock;
        }

        public void LockChanges()
        {
            Locked = true;
        }

        public void SetWareId(ushort wareId)
        {
            ThrowIfLocked();
            WareId = wareId;
        }

        public void SetId(ushort typeId)
        {
            ThrowIfLocked();

            TypeId = typeId;
        }

        public void SetClientId(ushort clientId)
        {
            ClientId = clientId;
        }

        public void SetName(string name)
        {
            ThrowIfLocked();

            Name = name;
        }

        public void SetRequirements(IItemRequirement[] requirements)
        {
            ThrowIfLocked();
        }
        public void SetOnUse()
        {
            ThrowIfLocked();
            if (OnUse is not null) return;
            OnUse = new ItemAttributeList();
        }
        public void SetFlag(ItemFlag flag)
        {
            ThrowIfLocked();

            Flags.Add(flag);
        }

        public void ParseOTFlags(uint flags)
        {
            if (HasOTFlag(flags, 1 << 0)) // blockSolid
                SetFlag(ItemFlag.BlockSolid);

            if (HasOTFlag(flags, 1 << 1)) // blockProjectile
                SetFlag(ItemFlag.BlockProjectTile);

            if (HasOTFlag(flags, 1 << 2)) // blockPathFind
                SetFlag(ItemFlag.BlockPathFind);

            if (HasOTFlag(flags, 1 << 3)) // hasElevation
                SetFlag(ItemFlag.HasHeight);

            if (HasOTFlag(flags, 1 << 4)) // isUsable
                SetFlag(ItemFlag.Useable);

            if (HasOTFlag(flags, 1 << 5)) // isPickupable
                SetFlag(ItemFlag.Pickupable);

            if (HasOTFlag(flags, 1 << 6)) // isMoveable
                SetFlag(ItemFlag.Moveable);

            if (HasOTFlag(flags, 1 << 7)) // isStackable
                SetFlag(ItemFlag.Stackable);

            if (HasOTFlag(flags, 1 << 13)) // alwaysOnTop
                SetFlag(ItemFlag.AlwaysOnTop);

            if (HasOTFlag(flags, 1 << 14)) // isReadable
                SetFlag(ItemFlag.Readable);

            if (HasOTFlag(flags, 1 << 15)) // isRotatable
                SetFlag(ItemFlag.Rotatable);

            if (HasOTFlag(flags, 1 << 16)) // isHangable
                SetFlag(ItemFlag.Hangable);

            if (HasOTFlag(flags, 1 << 17)) // isVertical
                SetFlag(ItemFlag.Vertical);

            if (HasOTFlag(flags, 1 << 18)) // isHorizontal
                SetFlag(ItemFlag.Horizontal);

            //if (HasFlag(flags, 1 << 19)) // cannotDecay -- unused

            if (HasOTFlag(flags, 1 << 20)) // allowDistRead
                SetFlag(ItemFlag.AllowDistRead);

            //if (HasFlag(flags, 1 << 21)) // unused -- unused

            //if (HasFlag(flags, 1 << 22)) // isAnimation -- unused

            if (HasOTFlag(flags, 1 << 23)) // lookTrough
                SetFlag(ItemFlag.LookTrough);

            if (HasOTFlag(flags, 1 << 26)) // forceUse
                SetFlag(ItemFlag.ForceUse);
        }

        private bool HasOTFlag(UInt32 flags, UInt32 flag)
        {
            return (flags & flag) != 0;
        }

        public void SetArticle(string article)
        {
            Article = article;
        }

        public void SetPlural(string plural)
        {
            Plural = plural;
        }

        public bool HasFlag(ItemFlag flag) => Flags.Contains(flag);

        public AmmoType AmmoType => Attributes?.GetAttribute(ItemAttribute.AmmoType) switch
        {
            "bolt" => AmmoType.Bolt,
            "arrow" => AmmoType.Arrow,
            _ => AmmoType.None
        };

        public Slot BodyPosition => SlotTypeParser.Parse(Attributes?.GetAttribute(ItemAttribute.BodyPosition));
        public ShootType ShootType => ShootTypeParser.Parse(Attributes?.GetAttribute(ItemAttribute.ShootType));
        public WeaponType WeaponType => WeaponTypeParser.Parse(Attributes?.GetAttribute(ItemAttribute.WeaponType));
        public DamageType DamageType => DamageTypeParser.Parse(Attributes?.GetAttribute(ItemAttribute.Damage));
        public EffectT EffectT => EffectParser.Parse(Attributes?.GetAttribute(ItemAttribute.Effect));


    }
}
