using NeoServer.Server.Model.Creatures.Contracts;
using NeoServer.Server.Model.Items.Contracts;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.World.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Model.Items
{
    public class Item : Thing, IItem
    {
        public event ItemHolderChangeEvent OnHolderChanged;

        public event ItemAmountChangeEvent OnAmountChanged;

        public IItemType Type { get; }

        public override ushort ThingId => Type.ClientId;

        public override byte Count => Amount;

        public bool ChangesOnUse => Type.Flags.Contains(ItemFlag.ChangeUse);

        public ushort ChangeOnUseTo
        {
            get
            {
                if (!Type.Flags.Contains(ItemFlag.ChangeUse))
                {
                    throw new InvalidOperationException($"Attempted to ChangeOnUse an item which doesn't have a target: {ThingId}");
                }

                return Convert.ToUInt16(Attributes[ItemAttribute.ChangeTarget]);
            }
        }

        public override bool CanBeMoved => !Type.Flags.Contains(ItemFlag.Unmove);

        public bool HasCollision => Type.Flags.Contains(ItemFlag.CollisionEvent);

        public bool HasSeparation => Type.Flags.Contains(ItemFlag.SeparationEvent);

        public override string InspectionText // TODO: implement correctly.
            => $"{Type.Name}{(string.IsNullOrWhiteSpace(Type.Description) ? string.Empty : "\n" + Type.Description)}";

        public override string CloseInspectionText => InspectionText;

        public uint HolderId => holder;

        public new Location Location
        {
            get
            {
                if (HolderId != 0)
                {
                    return CarryLocation;
                }

                return base.Location;
            }
        }

        public Location CarryLocation { get; private set; }

        public Dictionary<ItemAttribute, IConvertible> Attributes { get; }

        public bool IsCumulative => Type.Flags.Contains(ItemFlag.Cumulative);

        public byte Amount
        {
            get
            {
                if (Attributes.ContainsKey(ItemAttribute.Amount))
                {
                    return (byte)Math.Min(100, Convert.ToInt32(Attributes[ItemAttribute.Amount]));
                }
                return 0x01;
            }

            set
            {
                Attributes[ItemAttribute.Amount] = value;
            }
        }

        public bool IsPathBlocking(byte avoidType = 0)
        {
            var blocking = Type.Flags.Contains(ItemFlag.Unpass);

            if (blocking)
            {
                return true;
            }

            blocking |= Type.Flags.Contains(ItemFlag.Avoid) && (avoidType == 0 || Convert.ToByte(Attributes[ItemAttribute.AvoidDamageTypes]) == avoidType);

            return blocking;
        }

        public virtual void AddContent(IEnumerable<object> content)
        {
            Console.WriteLine($"Item.AddContent: Item with id {Type.TypeId} is not a Container, ignoring content.");
        }

        public bool IsContainer => Type.Flags.Contains(ItemFlag.Container) || Type.Flags.Contains(ItemFlag.Chest); // TODO: chest actually means a quest chest...

        public bool IsDressable => Type.Flags.Contains(ItemFlag.Clothes);

        public byte DressPosition => Attributes.ContainsKey(ItemAttribute.BodyPosition) ? Convert.ToByte(Attributes[ItemAttribute.BodyPosition]) : (byte)Slot.WhereEver;

        public bool IsGround => Type.Flags.Contains(ItemFlag.Bank);

        public byte MovementPenalty
        {
            get
            {
                if (!IsGround || !Attributes.ContainsKey(ItemAttribute.Waypoints))
                {
                    return 0;
                }

                return Convert.ToByte(Attributes[ItemAttribute.Waypoints]);
            }
        }

        public bool IsTop1 => Type.Flags.Contains(ItemFlag.Top) || Type.Flags.Contains(ItemFlag.Clip) || Type.Flags.Contains(ItemFlag.Hang);

        public bool IsTop2 => Type.Flags.Contains(ItemFlag.Bottom);

        public bool CanBeDressed => Type.Flags.Contains(ItemFlag.Clothes);

        public bool IsLiquidPool => Type.Flags.Contains(ItemFlag.LiquidPool);

        public bool IsLiquidSource => Type.Flags.Contains(ItemFlag.LiquidSource);

        public bool IsLiquidContainer => Type.Flags.Contains(ItemFlag.LiquidContainer);

        public byte LiquidType
        {
            get
            {
                if (!Type.Flags.Contains(ItemFlag.LiquidSource))
                {
                    return 0x00;
                }
                return Convert.ToByte(Attributes[ItemAttribute.SourceLiquidType]);
            }

            set
            {
                Attributes[ItemAttribute.SourceLiquidType] = value;
            }
        }

        public byte Attack
        {
            get
            {
                if (Type.Flags.Contains(ItemFlag.Weapon))
                {
                    return Convert.ToByte(Attributes[ItemAttribute.WeaponAttackValue]);
                }

                if (Type.Flags.Contains(ItemFlag.Throw))
                {
                    return Convert.ToByte(Attributes[ItemAttribute.ThrowAttackValue]);
                }

                return 0x00;
            }
        }

        public byte Defense
        {
            get
            {
                if (Type.Flags.Contains(ItemFlag.Shield))
                {
                    return Convert.ToByte(Attributes[ItemAttribute.ShieldDefendValue]);
                }

                if (Type.Flags.Contains(ItemFlag.Weapon))
                {
                    return Convert.ToByte(Attributes[ItemAttribute.WeaponAttackValue]);
                }

                if (Type.Flags.Contains(ItemFlag.Throw))
                {
                    return Convert.ToByte(Attributes[ItemAttribute.ThrowDefendValue]);
                }

                return 0x00;
            }
        }

        public byte Armor
        {
            get
            {
                if (Type.Flags.Contains(ItemFlag.Armor))
                {
                    return Convert.ToByte(Attributes[ItemAttribute.ArmorValue]);
                }

                return 0x00;
            }
        }

        public int Range
        {
            get
            {
                if (Type.Flags.Contains(ItemFlag.Throw))
                {
                    return Convert.ToByte(Attributes[ItemAttribute.ThrowRange]);
                }

                if (Type.Flags.Contains(ItemFlag.Bow))
                {
                    return Convert.ToByte(Attributes[ItemAttribute.BowRange]);
                }

                return 0x01;
            }
        }

        public bool BlocksThrow => Type.Flags.Contains(ItemFlag.Unthrow);

        public bool BlocksPass => Type.Flags.Contains(ItemFlag.Unpass);

        public bool BlocksLay => Type.Flags.Contains(ItemFlag.Unlay);

        public decimal Weight => (Type.Flags.Contains(ItemFlag.Take) ? Convert.ToDecimal(Attributes[ItemAttribute.Weight]) / 100 : default(decimal)) * Amount;

        public IContainer Parent { get; private set; }

        private uint holder;

        public Item(ItemType type)
        {
            Type = type;
            // UniqueId = Guid.NewGuid().ToString().Substring(0, 8);

            // make a copy of the type we are based on...
            // Name = Type.Name;
            // Description = Type.Description;
            // Flags = new HashSet<ItemFlag>(Type.Flags);
            Attributes = new Dictionary<ItemAttribute, IConvertible>(Type.DefaultAttributes);
        }

        public void AddAttributes(IList<CipAttribute> attributes)
        {
            foreach (var attribute in attributes)
            {
                if ("Content".Equals(attribute.Name))
                {
                    // recursive add
                    AddContent(attribute.Value as IEnumerable<CipElement>);
                }
                else
                {
                    // these are safe to add as Attributes of the item.
                    ItemAttribute itemAttr;

                    if (!Enum.TryParse(attribute.Name, out itemAttr))
                    {
                        if (!ServerConfiguration.SupressInvalidItemWarnings)
                        {
                            Console.WriteLine($"Item.AddContent: Unexpected attribute with name {attribute.Name} on item {Type.Name}, igoring.");
                        }

                        continue;
                    }

                    try
                    {
                        Attributes.Add(itemAttr, attribute.Value as IConvertible);
                    }
                    catch
                    {
                        Console.WriteLine($"Item.AddContent: Unexpected attribute {attribute.Name} with illegal value {attribute.Value} on item {Type.Name}, igoring.");
                    }
                }
            }
        }

        public void SetHolder(ICreature holder, Location holdingLoc = default(Location))
        {
            var oldHolder = this.holder;
            this.holder = holder?.CreatureId ?? 0;
            CarryLocation = holdingLoc;

            OnHolderChanged?.Invoke(this, oldHolder);
        }

        public void SetAmount(byte amount)
        {
            var oldAmount = Amount;

            Amount = Math.Min((byte)100, amount);

            OnAmountChanged?.Invoke(this, oldAmount);
        }

        public void SetParent(IContainer parentContainer)
        {
            Parent = parentContainer;
        }

        public virtual bool Join(IItem otherItem)
        {
            if (!IsCumulative || otherItem?.Type.TypeId != Type.TypeId)
            {
                return false;
            }

            var totalAmount = Amount + otherItem.Amount;

            SetAmount((byte)Math.Min(totalAmount, 100));
            otherItem.SetAmount((byte)Math.Max(totalAmount - 100, 0));

            return otherItem.Amount == 0;
        }

        public virtual bool Separate(byte amount, out IItem splitItem)
        {
            splitItem = null;

            if (amount > Amount)
            {
                return false;
            }

            SetAmount((byte)Math.Max(Amount - amount, 0));

            if (Amount > 0)
            {
                splitItem = ItemFactory.Create(Type.TypeId);
                splitItem.SetAmount(amount);
            }

            return true;
        }
    }
}
