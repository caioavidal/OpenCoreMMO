using NeoServer.OTB.Parsers;
using NeoServer.OTBM.Enums;
using System;
using System.Collections.Generic;

namespace NeoServer.OTBM.Structure
{
    public struct ItemNodeAttributeValue
    {
        public ItemNodeAttribute AttributeName { get; set; }
        public IConvertible Value { get; set; }
        public IEnumerable<CustomAttribute> CustomAttributes { get; set; }

        public ItemNodeAttributeValue(ItemNodeAttribute attribute, OTBParsingStream stream)
        {
            Value = null;
            AttributeName = ItemNodeAttribute.None;
            CustomAttributes = new List<CustomAttribute>();

            AttributeName = attribute;

            switch (attribute)
            {
                case ItemNodeAttribute.Count:
                case ItemNodeAttribute.RuneCharges:
                    Value = stream.ReadByte();
                    break;
                case ItemNodeAttribute.ActionId:
                case ItemNodeAttribute.UniqueId:
                case ItemNodeAttribute.Charges:
                case ItemNodeAttribute.ExtraDefense:
                case ItemNodeAttribute.DepotId:
                    Value = stream.ReadUInt16();
                    break;
                case ItemNodeAttribute.Text:
                case ItemNodeAttribute.WrittenBy:
                case ItemNodeAttribute.Description:
                case ItemNodeAttribute.Name:
                case ItemNodeAttribute.Article:
                case ItemNodeAttribute.PluralName:
                    Value = stream.ReadString();
                    break;
                case ItemNodeAttribute.WrittenDate:
                case ItemNodeAttribute.Weight:
                case ItemNodeAttribute.Attack:
                case ItemNodeAttribute.Defense:
                case ItemNodeAttribute.Armor:
                case ItemNodeAttribute.HitChance:
                case ItemNodeAttribute.ShootRange:
                case ItemNodeAttribute.DecayTo:
                case ItemNodeAttribute.SleeperGUID:
                case ItemNodeAttribute.SleepStart:
                case ItemNodeAttribute.ContainerItems:
                    Value = stream.ReadUInt32();
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

                case ItemNodeAttribute.HouseDoorId:
                    Value = stream.ReadByte();
                    break;
                case ItemNodeAttribute.TeleportDestination:
                    stream.Skip(5); //todo
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
