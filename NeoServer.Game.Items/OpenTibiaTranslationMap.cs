using System;
using System.Collections.Generic;
using System.Text;
using NeoServer.Game.Enums;

namespace NeoServer.Server.Model.Items
{
    public class OpenTibiaTranslationMap
    {

        public static ItemAttribute TranslateAttributeName(string attrName, out bool success)
        {
            success = true;
            switch (attrName)
            {
                case "containerSize": return ItemAttribute.Capacity;
                case "weight": return ItemAttribute.Weight;
                case "speed": return ItemAttribute.Waypoints;
                // case "": return ItemAttribute.AvoidDamageTypes;
                case "decayTo": return ItemAttribute.ExpireTarget;
                case "duration": return ItemAttribute.TotalExpireTime;
                case "fluidSource": return ItemAttribute.SourceLiquidType;
                case "transformTo": return ItemAttribute.DisguiseTarget;
                // case "": return ItemAttribute.Brightness;
                // case "": return ItemAttribute.LightColor;
                // case "": return ItemAttribute.Elevation;
                // case "": return ItemAttribute.KeydoorTarget;
                // case "": return ItemAttribute.ChangeTarget;
                // case "": return ItemAttribute.NamedoorTarget;
                // case "": return ItemAttribute.FontSize;
                // case "": return ItemAttribute.QuestdoorTarget;
                // case "": return ItemAttribute.LeveldoorTarget;
                // case "": return ItemAttribute.ThrowRange;
                // case "": return ItemAttribute.ThrowAttackValue;
                // case "": return ItemAttribute.ThrowDefendValue;
                // case "": return ItemAttribute.ThrowMissile;
                // case "": return ItemAttribute.ThrowSpecialEffect;
                // case "": return ItemAttribute.ThrowEffectStrength;
                // case "": return ItemAttribute.ThrowFragility;
                case "rotateTo": return ItemAttribute.RotateTarget;
                case "destroyTo": return ItemAttribute.DestroyTarget;
                // case "": return ItemAttribute.Meaning;
                // case "": return ItemAttribute.InformationType;
                case "maxTextLen": return ItemAttribute.MaxLength;
                // case "": return ItemAttribute.MaxLengthOnce;
                case "slotType": return ItemAttribute.BodyPosition;
                // case "": return ItemAttribute.ShieldDefendValue;
                // case "": return ItemAttribute.ProtectionDamageTypes;
                // case "": return ItemAttribute.DamageReduction;
                // case "": return ItemAttribute.WearoutTarget;
                // case "": return ItemAttribute.TotalUses;
                case "armor": return ItemAttribute.ArmorValue;
                // case "": return ItemAttribute.MinimumLevel;
                // case "": return ItemAttribute.Professions;
                // case "": return ItemAttribute.WandRange;
                // case "": return ItemAttribute.WandManaConsumption;
                // case "": return ItemAttribute.WandAttackStrength;
                // case "": return ItemAttribute.WandAttackVariation;
                // case "": return ItemAttribute.WandDamageType;
                // case "": return ItemAttribute.WandMissile;
                // case "": return ItemAttribute.SkillNumber;
                // case "": return ItemAttribute.SkillModification;
                case "weaponType": return ItemAttribute.WeaponType;
                // case "": return ItemAttribute.WeaponAttackValue;
                case "defense": return ItemAttribute.WeaponDefendValue;
                // case "": return ItemAttribute.Nutrition;
                // case "": return ItemAttribute.BowRange;
                // case "": return ItemAttribute.BowAmmoType;
                // case "": return ItemAttribute.AmmoType;
                // case "": return ItemAttribute.AmmoAttackValue;
                // case "": return ItemAttribute.AmmoMissile;
                // case "": return ItemAttribute.AmmoSpecialEffect;
                // case "": return ItemAttribute.AmmoEffectStrength;
                case "corpseType": return ItemAttribute.CorpseType;
                // case "": return ItemAttribute.AbsTeleportEffect;
                // case "": return ItemAttribute.RelTeleportEffect;
                // case "": return ItemAttribute.RelTeleportDisplacement;
                // case "": return ItemAttribute.Amount;
                // case "": return ItemAttribute.ContainerLiquidType;
                // case "": return ItemAttribute.PoolLiquidType;
                // case "": return ItemAttribute.String;
                // case "": return ItemAttribute.SavedExpireTime;

                default:
                    success = false;
                    return ItemAttribute.AbsTeleportEffect; // Just return the first
            }
        }

        public static int TranslateMeeleWeaponTypeName(string typeName, out bool success)
        {
            success = true;
            switch (typeName)
            {
                case "sword":
                    return 1;

                case "club":
                    return 2;

                case "axe":
                    return 3;

                default:
                    success = false;
                    return 0;
            }
        }

        public static ItemFlag TranslateToItemFlag(string possibleFlag, out bool success)
        {
            success = true;
            switch (possibleFlag)
            {
                case "distance":
                    return ItemFlag.Bow;

                case "shield":
                    return ItemFlag.Shield;

                case "ammunition":
                    return ItemFlag.Ammo;

                case "wand":
                    return ItemFlag.Wand;

                default:
                    success = false;
                    return ItemFlag.Ammo; // Just return the first
            }
        }

        public static int TranslateLiquidType(string type, out bool success)
        {
            success = true;
            switch (type)
            {
                case "water":
                    return 1;

                case "wine":
                    return 2;

                case "beer":
                    return 3;

                case "muddy":
                    return 4;

                case "blood":
                    return 5;

                case "slime":
                    return 6;

                default:
                    success = false;
                    return 0;
            }
        }

        public static int TranslateCorpseType(string type, out bool success)
        {
            success = true;
            switch (type)
            {
                case "blood":
                    return 1;

                case "venom":
                    return 2;

                default:
                    success = false;
                    return 0;
            }
        }

        public static int TranslateSlotType(string type, out bool success)
        {
            success = true;
            switch (type)
            {
                case "two-handed":
                    return 0;

                case "head":
                    return 1;

                case "necklace":
                    return 2;

                case "backpack":
                    return 3;

                case "body":
                    return 4;

                case "right-hand": // Not sure if right-hand comes first than left-hand
                    return 5;

                case "left-hand":
                    return 6;

                case "legs":
                    return 7;

                case "feet":
                    return 8;

                case "ring":
                    return 9;

                default:
                    success = false;
                    return 0;
            }
        }

    }
}
