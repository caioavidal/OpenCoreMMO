using NeoServer.Server.Model.Items.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NeoServer.Server.Model.Items
{
    public class ItemType : IItemType
    {
        public ushort TypeId { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public ISet<ItemFlag> Flags { get; }

        public IDictionary<ItemAttribute, IConvertible> DefaultAttributes { get; }

        public bool Locked { get; private set; }

        public ushort ClientId { get; private set; }

        public ItemType()
        {
            TypeId = 0;
            Name = string.Empty;
            Description = string.Empty;
            Flags = new HashSet<ItemFlag>();
            DefaultAttributes = new Dictionary<ItemAttribute, IConvertible>();
            Locked = false;
        }

        public void LockChanges()
        {
            Locked = true;
        }

        public void SetId(ushort typeId)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            TypeId = typeId;
        }

        public void SetClientId(ushort clientId)
        {
            ClientId = clientId;
        }

        public void SetName(string name)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            Name = name;
        }

        public void SetDescription(string description)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            Description = description.Trim('"');
        }

        public void SetFlag(ItemFlag flag)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            Flags.Add(flag);
        }

        public void SetAttribute(ItemAttribute attribute, int attributeValue)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            DefaultAttributes[attribute] = attributeValue;
        }

        public void SetAttribute(string attributeName, int attributeValue)
        {
            if (Locked)
            {
                throw new InvalidOperationException("This ItemType is locked and cannot be altered.");
            }

            if (!Enum.TryParse(attributeName, out ItemAttribute attribute))
            {
                throw new InvalidDataException($"Attempted to set an unknown Item attribute [{attributeName}].");
            }

            DefaultAttributes[attribute] = attributeValue;
        }

        public void ParseOTFlags(UInt32 flags)
        {
            if (HasOTFlag(flags, 1 << 0)) // blockSolid
                SetFlag(ItemFlag.CollisionEvent);

            if (HasOTFlag(flags, 1 << 1)) // blockProjectile
                SetFlag(ItemFlag.Unthrow);

            if (HasOTFlag(flags, 1 << 2)) // blockPathFind
                SetFlag(ItemFlag.Unpass);

            if (HasOTFlag(flags, 1 << 3)) // hasElevation
                SetFlag(ItemFlag.Height);

            if (HasOTFlag(flags, 1 << 4)) // isUsable
                SetFlag(ItemFlag.UseEvent);

            if (HasOTFlag(flags, 1 << 5)) // isPickupable
                SetFlag(ItemFlag.Take);

            if (HasOTFlag(flags, 1 << 6)) // isMoveable
                SetFlag(ItemFlag.Unmove);

            if (HasOTFlag(flags, 1 << 7)) // isStackable
                SetFlag(ItemFlag.Cumulative);

            //if (HasFlag(flags, 1 << 8)) // floorChangeDown -- unused

            //if (HasFlag(flags, 1 << 9)) // floorChangeNorth -- unused

            //if (HasFlag(flags, 1 << 10)) // floorChangeEast -- unused

            //if (HasFlag(flags, 1 << 11)) // floorChangeSouth -- unused

            //if (HasFlag(flags, 1 << 12)) // floorChangeWest -- unused

            if (HasOTFlag(flags, 1 << 13)) // alwaysOnTop
                SetFlag(ItemFlag.Top);

            if (HasOTFlag(flags, 1 << 14)) // isReadable
                SetFlag(ItemFlag.Text);

            if (HasOTFlag(flags, 1 << 15)) // isRotatable
                SetFlag(ItemFlag.Rotate);

            if (HasOTFlag(flags, 1 << 16)) // isHangable
                SetFlag(ItemFlag.Hang);

            if (HasOTFlag(flags, 1 << 17)) // isVertical
                SetFlag(ItemFlag.HookEast);

            if (HasOTFlag(flags, 1 << 18)) // isHorizontal
                SetFlag(ItemFlag.HookSouth);

            //if (HasFlag(flags, 1 << 19)) // cannotDecay -- unused

            if (HasOTFlag(flags, 1 << 20)) // allowDistRead
                SetFlag(ItemFlag.DistUse);

            //if (HasFlag(flags, 1 << 21)) // unused -- unused

            //if (HasFlag(flags, 1 << 22)) // isAnimation -- unused

            if (HasOTFlag(flags, 1 << 23)) // lookTrough
                SetFlag(ItemFlag.Top);
            else
                SetFlag(ItemFlag.Bottom);

            if (HasOTFlag(flags, 1 << 25)) // fullTile
                SetFlag(ItemFlag.Bank);

            if (HasOTFlag(flags, 1 << 26)) // forceUse
                SetFlag(ItemFlag.ForceUse);
        }

        private bool HasOTFlag(UInt32 flags, UInt32 flag)
        {
            return (flags & flag) != 0;
        }

        public bool ParseOTWeaponType(string type)
        {
            var value = OpenTibiaTranslationMap.TranslateMeeleWeaponTypeName(type, out bool success);
            if (success)
                SetAttribute(ItemAttribute.WeaponType, value);
            else
            {
                var flag = OpenTibiaTranslationMap.TranslateToItemFlag(type, out success);
                if (success)
                    SetFlag(flag);
                else
                    return false;
            }

            return true;
        }

    }
}
