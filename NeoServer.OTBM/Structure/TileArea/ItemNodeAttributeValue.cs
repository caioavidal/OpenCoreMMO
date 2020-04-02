using System;
using System.Collections.Generic;
using NeoServer.OTBM.Enums;
using NeoServer.OTBM.Helpers;

namespace NeoServer.OTBM.Structure
{
    public class ItemNodeAttributeValue
    {
        public ItemNodeAttribute AttributeName { get; set; }
        public IConvertible Value { get; set; }
        public IEnumerable<CustomAttribute> CustomAttributes { get; set; }

        public ItemNodeAttributeValue(ItemNodeAttribute attribute, OTBParsingStream stream)
        {
            AttributeName = attribute;

            switch (attribute)
            {
                case ItemNodeAttribute.Count:
                case ItemNodeAttribute.RuneCharges:
                    Value = stream.ReadByte();
                    break;
                case ItemNodeAttribute.ActionId:
                    Value = stream.ReadUInt16();
                    break;
                case ItemNodeAttribute.UniqueId:
                    Value = stream.ReadUInt16();
                    break;
                case ItemNodeAttribute.Text:
                    Value = stream.ReadString();
                    break;
                case ItemNodeAttribute.WrittenDate:
                    Value = stream.ReadUInt32();
                    break;
                case ItemNodeAttribute.WrittenBy:
                    Value = stream.ReadString();
                    break;
                case ItemNodeAttribute.Description:
                    Value = stream.ReadString();
                    break;
                case ItemNodeAttribute.Charges:
                    Value = stream.ReadUInt16();
                    break;
                case ItemNodeAttribute.Duration:
                    Value = Math.Max(0, stream.ReadUInt32());
                    break;
                case ItemNodeAttribute.DecayingState:
                    Value = stream.ReadByte();

                    if ((ItemDecayingState)Value != ItemDecayingState.False)
                    {
                        Value = ItemDecayingState.Pending;
                    }
                    break;
                case ItemNodeAttribute.Name:
                    Value = stream.ReadString();
                    break;
                case ItemNodeAttribute.Article:
                    Value = stream.ReadString();
                    break;
                case ItemNodeAttribute.PluralName:
                    Value = stream.ReadString();
                    break;
                case ItemNodeAttribute.Weight:
                    Value = stream.ReadUInt32();
                    break;
                case ItemNodeAttribute.Attack:
                    Value = stream.ReadUInt32();
                    break;
                case ItemNodeAttribute.Defense:
                    Value = stream.ReadUInt32();
                    break;
                case ItemNodeAttribute.ExtraDefense:
                    Value = stream.ReadUInt16();
                    break;
                case ItemNodeAttribute.Armor:
                    Value = stream.ReadUInt32();
                    break;
                case ItemNodeAttribute.HitChance:
                    Value = stream.ReadUInt32();
                    break;
                case ItemNodeAttribute.ShootRange:
                    Value = stream.ReadUInt32();
                    break;
                case ItemNodeAttribute.DecayTo:
                    Value = stream.ReadUInt32();
                    break;
                case ItemNodeAttribute.DepotId:
                    Value = stream.ReadUInt16();
                    break;
                case ItemNodeAttribute.HouseDoorId:
                    Value = stream.ReadByte();
                    break;
                case ItemNodeAttribute.SleeperGUID:
                    Value = stream.ReadUInt32();
                    break;
                case ItemNodeAttribute.SleepStart:
                    Value = stream.ReadUInt32();
                    break;
                case ItemNodeAttribute.TeleportDestination:
                    stream.Skip(5); //todo
                    break;
                case ItemNodeAttribute.ContainerItems:
                    break;

                case ItemNodeAttribute.CustomAttributes:
                    throw new NotImplementedException(); //todo
                    var size = stream.ReadUInt64();
                    for (ulong i = 0; i < size; i++)
                    {
                        CustomAttributes = new List<CustomAttribute>();

                        ((List<CustomAttribute>)Value).Add(new CustomAttribute(stream));
                    }
                    break;
            }
        }
    }
}
