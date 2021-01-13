using NeoServer.Game.Common;

namespace NeoServer.OTB.Parsers
{
    public class ItemAttributeTranslationMap
    {
        /// <summary>
        /// Translate attribute name to enum
        /// </summary>
        public static ItemAttribute TranslateAttributeName(string attrName, out bool success)
        {
            success = true;
            switch (attrName.ToLower())
            {
                case "type": return ItemAttribute.Type;
                case "description": return ItemAttribute.Description;
                case "runespellname": return ItemAttribute.RuneSpellName;
                case "weight": return ItemAttribute.Weight;
                case "showcount": return ItemAttribute.ShowCount;
                case "armor": return ItemAttribute.ArmorValue;
                case "defense": return ItemAttribute.WeaponDefendValue;
                case "extradef": return ItemAttribute.ExtraDefense;
                case "attack": return ItemAttribute.Attack;
                case "rotateto": return ItemAttribute.RotateTarget;
                case "moveable": return ItemAttribute.Moveable;
                case "movable": return ItemAttribute.Moveable;
                case "blockprojectile": return ItemAttribute.BlockProjectTile;
                case "allowpickupable": return ItemAttribute.Pickupable;
                case "pickupable": return ItemAttribute.Pickupable;
                case "forceserialize": return ItemAttribute.ForceSerialize;
                case "floorchange": return ItemAttribute.FloorChange;
                case "corpsetype": return ItemAttribute.CorpseType;
                case "containersize": return ItemAttribute.Capacity;
                case "fluidsource": return ItemAttribute.SourceLiquidType;
                case "readable": return ItemAttribute.Readable;
                case "writeable": return ItemAttribute.Writeable;
                case "maxtextlen": return ItemAttribute.MaxLength;
                case "writeonceitemid": return ItemAttribute.WriteOnceItemId;
                case "weapontype": return ItemAttribute.WeaponType;
                case "slottype": return ItemAttribute.BodyPosition;
                case "ammotype": return ItemAttribute.AmmoType;
                case "shoottype": return ItemAttribute.ShootType;
                case "effect": return ItemAttribute.Effect;
                case "range": return ItemAttribute.Range;
                case "stopduration": return ItemAttribute.TotalExpireTime;
                case "decayto": return ItemAttribute.ExpireTarget;
                case "transformequipto": return ItemAttribute.TransformEquipTo;
                case "transformdeequipto": return ItemAttribute.TransformDequipTo;
                case "duration": return ItemAttribute.Duration;
                case "showduration": return ItemAttribute.ShowDuration;
                case "charges": return ItemAttribute.Charges;
                case "showcharges": return ItemAttribute.ShowCharges;
                case "showattributes": return ItemAttribute.ShowAttributes;
                case "hitchance": return ItemAttribute.HitChance;
                case "maxhitchance": return ItemAttribute.MaxHitChange;
                case "invisible": return ItemAttribute.Invisible;
                case "speed": return ItemAttribute.Speed;
                case "healthgain": return ItemAttribute.HealthGain;
                case "healthticks": return ItemAttribute.HealTicks;
                case "managain": return ItemAttribute.ManaGain;
                case "manaticks": return ItemAttribute.ManaTicks;
                case "manashield": return ItemAttribute.ManaShield;
                case "skillsword": return ItemAttribute.SkillSword;
                case "skillaxe": return ItemAttribute.SkillAxe;
                case "skillclub": return ItemAttribute.SkillClub;
                case "skilldist": return ItemAttribute.SkillDistance;
                case "skillfish": return ItemAttribute.SkillFishing;
                case "skillshield": return ItemAttribute.SkillShield;
                case "skillfist": return ItemAttribute.SkillFist;
                case "maxhitpoints": return ItemAttribute.MaxHitpoints;
                case "maxhitpointspercent": return ItemAttribute.MaxHitpointsPercent;
                case "maxmanapoints": return ItemAttribute.MaxManapoints;
                case "maxmanapointspercent": return ItemAttribute.MaxManapointsPercent;
                case "magicpoints": return ItemAttribute.MagicPoints;
                case "magiclevelpoints": return ItemAttribute.MagicPoints;
                case "magicpointspercent": return ItemAttribute.MagicPointsPercent;
                case "criticalhitchance": return ItemAttribute.CriticalHitChance;
                case "criticalhitamount": return ItemAttribute.CriticalHitAmount;
                case "lifeleechchance": return ItemAttribute.LifeLeechChance;
                case "lifeleechamount": return ItemAttribute.LifeLeechAmount;
                case "manaleechchance": return ItemAttribute.ManaLeechChange;
                case "manaleechamount": return ItemAttribute.ManaLeechAmout;
                case "fieldabsorbpercentenergy": return ItemAttribute.FieldAbsorbPercentEnergy;
                case "fieldabsorbpercentfire": return ItemAttribute.FieldAbsorbEercentFire;
                case "fieldabsorbpercentpoison": return ItemAttribute.FieldAbsorbPercentPoison;
                case "fieldabsorbpercentearth": return ItemAttribute.FieldAbsorbPercentPoison;
                case "absorbpercentall": return ItemAttribute.AbsorbPercentAll;
                case "absorbpercentallelements": return ItemAttribute.AbsorbPercentAll;
                case "absorbpercentelements": return ItemAttribute.AbsorbPercentElements;
                case "absorbpercentmagic": return ItemAttribute.AbsorbPercentMagic;
                case "absorbpercentenergy": return ItemAttribute.AbsorbPercentEnergy;
                case "absorbpercentfire": return ItemAttribute.AbsorbPercentFire;
                case "absorbpercentpoison": return ItemAttribute.AbsorbPercentPoison;
                case "absorbpercentearth": return ItemAttribute.AbsorbPercentPoison;
                case "absorbpercentice": return ItemAttribute.AbsorbPercentIce;
                case "absorbpercentholy": return ItemAttribute.AbsorbPercentHoly;
                case "absorbpercentdeath": return ItemAttribute.AbsorbPercentDeath;
                case "absorbpercentlifedrain": return ItemAttribute.AbsorbPercentLifeDrain;
                case "absorbpercentmanadrain": return ItemAttribute.AbsorbPercentManaDrain;
                case "absorbpercentdrown": return ItemAttribute.AbsorbPercentDrown;
                case "absorbpercentphysical": return ItemAttribute.AbsorbPercentPhysical;
                case "absorbpercenthealing": return ItemAttribute.AbsorbPercentHealing;
                case "absorbpercentundefined": return ItemAttribute.AbsorbPercenUndefined; //remove
                case "suppressdrunk": return ItemAttribute.SuppressDrunk;
                case "suppressenergy": return ItemAttribute.SuppressEnergy;
                case "suppressfire": return ItemAttribute.SuppressFire;
                case "suppresspoison": return ItemAttribute.SuppressPoison;
                case "suppressdrown": return ItemAttribute.SuppressDrown;
                case "suppressphysical": return ItemAttribute.SuppressPhysical;
                case "suppressfreeze": return ItemAttribute.SuppressFreeze;
                case "suppressdazzle": return ItemAttribute.SuppressDazzle;
                case "suppresscurse": return ItemAttribute.SuppressCurse;
                case "field": return ItemAttribute.Field;
                case "replaceable": return ItemAttribute.Replaceable;
                case "partnerdirection": return ItemAttribute.PartnerDirection;
                case "leveldoor": return ItemAttribute.LevelDoor;
                case "maletransformto": return ItemAttribute.MaleTransformTo;
                case "malesleeper": return ItemAttribute.MaleTransformTo;
                case "femaletransformto": return ItemAttribute.FemaleTransformTo;
                case "femalesleeper": return ItemAttribute.FemaleTransformTo;
                case "transformto": return ItemAttribute.TransformTo;
                case "destroyto": return ItemAttribute.DestroyTo;
                case "elementice": return ItemAttribute.ElementIce;
                case "elementearth": return ItemAttribute.ElementEarth;
                case "elementfire": return ItemAttribute.ElementFire;
                case "elementenergy": return ItemAttribute.ElementEnergy;
                case "walkstack": return ItemAttribute.WalkStack;
                case "blocking": return ItemAttribute.Blocking;
                case "allowdistread": return ItemAttribute.AllowDistRead;
                case "minlevel": return ItemAttribute.MinimumLevel;
                case "teleport": return ItemAttribute.UseOn;
                case "useon": return ItemAttribute.UseOn;
                case "healing": return ItemAttribute.Healing;
                case "min": return ItemAttribute.Min;
                case "max": return ItemAttribute.Max;
                case "sentence": return ItemAttribute.Sentence;
                case "vocations": return ItemAttribute.Vocation;
                case "regeneration": return ItemAttribute.Regeneration;
                case "needtarget": return ItemAttribute.NeedTarget;
                case "formula": return ItemAttribute.Formula;
                case "damage": return ItemAttribute.Damage;
                case "area": return ItemAttribute.Area;
                case "ticks": return ItemAttribute.Ticks;
                case "count": return ItemAttribute.Count;
                default:
                    success = false;
                    return ItemAttribute.None; // Just return the first
            }
        }

        public static bool TranslateFlagName(string flagName, out ItemFlag flag)
        {
            flag = ItemFlag.Useable;
            switch (flagName)
            {
                case "useable":
                    flag = ItemFlag.Useable;
                    break;
                default:
                    return false;
            }

            return true;
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
