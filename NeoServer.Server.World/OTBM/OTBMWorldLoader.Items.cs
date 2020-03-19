//using NeoServer.Server.Model.Items;
//using NeoServer.Server.World.OTB;
//using System;

//namespace NeoServer.Server.World
//{
//	public  class OTBMWorldLoader {

//		private  Item ParseItemData(OTBParsingStream stream) {
//			var newItemId = GetItemIdAndConvertPvpFieldsToPermanentFields(stream);
			
//			var item = ItemFactory.Create(newItemId);
//			if (item == null) {
//				//_logger.Warn($"{nameof(ItemFactory)} was unable to create a item with id: {newItemId}."); TODO
//				return null;
//			}

//			if(!stream.IsOver && item.Count > 0) {
//				//var itemAttribute = (OTBMWorldItemAttribute)stream.ReadByte();
//				//itemAttribute = (OTBMWorldItemAttribute)stream.ReadByte();
//				//itemAttribute = (OTBMWorldItemAttribute)stream.ReadByte();
//				//itemAttribute = (OTBMWorldItemAttribute)stream.ReadByte();
//				//while (!stream.IsOver && itemAttribute != OTBMWorldItemAttribute.None && itemAttribute != OTBMWorldItemAttribute.None2) {
//				//	switch (itemAttribute) {
//				//		case OTBMWorldItemAttribute.Count:
//				//		case OTBMWorldItemAttribute.RuneCharges:
//				//		var count = stream.ReadByte();
//				//		item.SetAmount(count);
//				//		break;

//				//		case OTBMWorldItemAttribute.ActionId:
//				//		var actionId = stream.ReadUInt16();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.ActionId)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.UniqueId:
//				//		var uniqueId = stream.ReadUInt16();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.UniqueId)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.Text:
//				//		var text = stream.ReadString();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Text)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.WrittenDate:
//				//		var writtenDate = stream.ReadUInt32();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.WrittenDate)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.WrittenBy:
//				//		var writtenBy = stream.ReadString();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.WrittenBy)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.Description:
//				//		var anotherDescription = stream.ReadString();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Description)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.Charges:
//				//		var charges = stream.ReadUInt16();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Charges)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.Duration:
//				//		var duration = stream.ReadUInt32();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Duration)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.DecayingState:
//				//		var decayingState = stream.ReadByte();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.DecayingState)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.Name:
//				//		var name = stream.ReadString();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Name)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.Article:
//				//		var article = stream.ReadString();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Article)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.PluralName:
//				//		var pluralName = stream.ReadString();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Article)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.Weight:
//				//		var weight = stream.ReadUInt32();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Weight)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.Attack:
//				//		var attack = stream.ReadUInt32();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Attack)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.Defense:
//				//		var defense = stream.ReadUInt32();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Defense)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.ExtraDefense:
//				//		var extraDefense = stream.ReadUInt32();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.ExtraDefense)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.Armor:
//				//		var armor = stream.ReadUInt32();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.Armor)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.HitChance:
//				//		var hitChance = stream.ReadUInt32();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.HitChance)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.ShootRange:
//				//		var shootRange = stream.ReadUInt32();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.ShootRange)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.DepotId:
//				//		var depotId = stream.ReadUInt32();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.DepotId)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.HouseDorId:
//				//		var hourDoorId = stream.ReadUInt32();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.HouseDorId)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.SleeperGUID:
//				//		var sleeperGUID = stream.ReadUInt32();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.SleeperGUID)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.SleepStart:
//				//		var sleepStart = stream.ReadUInt32();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.SleepStart)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.TeleportDestination:
//				//		var teleportDestination = stream.ReadUInt32();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.TeleportDestination)} not implemented.");
//				//		break;

//				//		case OTBMWorldItemAttribute.ContainerItems:
//				//		throw new InvalidOperationException("TFS threw an exception here, so we're throwing too.");

//				//		case OTBMWorldItemAttribute.CustomAttributes:
//				//		var size = stream.ReadUInt64();
//				//		_logger.Warn($"OTBM {nameof(OTBMWorldItemAttribute.CustomAttributes)} not implemented.");
//				//		for (UInt64 i = 0; i < size; i++) {
//				//			var key = stream.ReadString();
//				//			var pos = (OTBMCustomAttributeValueType)stream.ReadByte();
//				//			object value;

//				//			switch (pos) {
//				//				case OTBMCustomAttributeValueType.String:
//				//				value = stream.ReadString();
//				//				break;

//				//				case OTBMCustomAttributeValueType.Int64:
//				//				value = stream.ReadUInt64();
//				//				break;

//				//				case OTBMCustomAttributeValueType.Double:
//				//				value = stream.ReadDouble();
//				//				break;

//				//				case OTBMCustomAttributeValueType.Bool:
//				//				value = stream.ReadBool();
//				//				break;

//				//				default:
//				//				value = new object();
//				//				break;
//				//			}
//				//		}
//				//		break;

//				//		case OTBMWorldItemAttribute.AnotherDescription:
//				//		throw new InvalidOperationException("TFS didn't implement this.");

//				//		case OTBMWorldItemAttribute.ExtensionFile:
//				//		throw new InvalidOperationException("TFS didn't implement this.");

//				//		case OTBMWorldItemAttribute.TileFlags:
//				//		throw new InvalidOperationException("TFS didn't implement this.");

//				//		case OTBMWorldItemAttribute.Item:
//				//		throw new InvalidOperationException("TFS didn't implement this.");

//				//		case OTBMWorldItemAttribute.ExtensionFileForSpawns:
//				//		throw new InvalidOperationException("TFS didn't implement this.");

//				//		case OTBMWorldItemAttribute.ExtensionFileForHouses:
//				//		throw new InvalidOperationException("TFS didn't implement this.");

//				//		default:
//				//		throw new InvalidOperationException($"Unknown {nameof(OTBMWorldItemAttribute)}.");
//				//	}
//				//}
//			}
			
//			return item;
//		}

//		private static ushort GetItemIdAndConvertPvpFieldsToPermanentFields(OTBParsingStream stream) {
//			var originalItemId = stream.ReadUInt16();
//			var newItemId = originalItemId;

//			switch (originalItemId) {
//				case (UInt16)OTBMWorldItemId.FireFieldPvpLarge:
//				newItemId = (UInt16)OTBMWorldItemId.FireFieldPersistentLarge;
//				break;

//				case (UInt16)OTBMWorldItemId.FireFieldPvpMedium:
//				newItemId = (UInt16)OTBMWorldItemId.FireFieldPersistentMedium;
//				break;

//				case (UInt16)OTBMWorldItemId.FireFieldPvpSmall:
//				newItemId = (UInt16)OTBMWorldItemId.FireFieldPersistentSmall;
//				break;

//				case (UInt16)OTBMWorldItemId.EnergyFieldPvp:
//				newItemId = (UInt16)OTBMWorldItemId.EnergyFieldPersistent;
//				break;

//				case (UInt16)OTBMWorldItemId.PoisonFieldPvp:
//				newItemId = (UInt16)OTBMWorldItemId.PoisonFieldPersistent;
//				break;

//				case (UInt16)OTBMWorldItemId.MagicWall:
//				newItemId = (UInt16)OTBMWorldItemId.MagicWallPersistent;
//				break;

//				case (UInt16)OTBMWorldItemId.WildGrowth:
//				newItemId = (UInt16)OTBMWorldItemId.WildGrowthPersistent;
//				break;

//				default:
//				break;
//			}

//			//if (newItemId != originalItemId)
//				//_logger.Warn($"Converted {(OTBMWorldItemId)originalItemId} to {(OTBMWorldItemId)newItemId}."); TODO

//			return newItemId;
//		}
//	}
//}
