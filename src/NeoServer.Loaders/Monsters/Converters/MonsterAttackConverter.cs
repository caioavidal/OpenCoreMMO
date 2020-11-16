﻿using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Contracts.Combat.Attacks;
using NeoServer.Game.Enums.Item;
using NeoServer.Server.Helpers.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Loaders.Monsters.Converters
{
    class MonsterAttackConverter
    {
        public static IMonsterCombatAttack[] Convert(MonsterData.MonsterMetadata data)
        {
            var attacks = new List<IMonsterCombatAttack>();

            foreach (var attack in data.Attacks)
            {
                attack.TryGetValue("name", out string attackName);
                attack.TryGetValue("attack", out byte attackValue);
                attack.TryGetValue("skill", out byte skill);

                attack.TryGetValue("min", out decimal min);
                attack.TryGetValue("max", out decimal max);
                attack.TryGetValue("chance", out byte chance);
                attack.TryGetValue("interval", out ushort interval);
                attack.TryGetValue("length", out byte length);
                attack.TryGetValue("radius", out byte radius);
                attack.TryGetValue("spread", out byte spread);
                attack.TryGetValue("target", out byte target);
                attack.TryGetValue("range", out byte range);

                attack.TryGetValue<JArray>("attributes", out var attributes);
                var shootEffect = attributes?.FirstOrDefault(a => a.Value<string>("key") == "shootEffect")?.Value<string>("value");
                var areaEffect = attributes?.FirstOrDefault(a => a.Value<string>("key") == "areaEffect")?.Value<string>("value");


                var combatAttack = new MonsterCombatAttack()
                {
                    Chance = chance > 100 || chance <= 0 ? (byte)100 : chance,
                    Interval = interval,
                    Length = length,
                    Radius = radius,
                    MaxDamage = (ushort)Math.Abs(max),
                    MinDamage = (ushort)Math.Abs(min),
                    Spread = spread,
                    Target = target,
                    DamageType = ParseDamageType(attackName),
                    //ShootType = ParseShootType(shootEffect),
                    AreaEffect = ParseAreaEffect(areaEffect),

                };

                if (combatAttack.IsMelee)
                {
                    combatAttack.MinDamage = 0;
                    combatAttack.MaxDamage = MeleeCombatAttack.CalculateMaxDamage(skill, attackValue);
                }
                if (attackName == "physical")
                {
                    combatAttack.CombatAttack = new DistanceCombatAttack(range, ParseShootType(shootEffect));
                }

                attacks.Add(combatAttack);


                //    attackName
                //    attack.TryGetValue<JArray>("attributes", out var attributes);

                //    if (attack.ContainsKey("range"))
                //    {
                //        attack.TryGetValue("range", out byte range);
                //        attack.TryGetValue("radius", out byte radius);

                //        var shootEffect = attributes?.FirstOrDefault(a => a.Value<string>("key") == "shootEffect")?.Value<string>("value");


                //        if (attackName == "manadrain")
                //        {

                //            attacks.Add(new ManaDrainCombatAttack(new CombatAttackOption
                //            {
                //                Chance = chance,
                //                Range = range,
                //                MinDamage = (ushort)Math.Abs(min),
                //                MaxDamage = (ushort)Math.Abs(max)
                //            }));
                //        }
                //        else if (attackName == "firefield")
                //        {
                //            attacks.Add(new FieldCombatAttack(new CombatAttackOption
                //            {
                //                Chance = chance,
                //                Radius = radius,
                //                Range = range,
                //                MinDamage = (ushort)Math.Abs(min),
                //                MaxDamage = (ushort)Math.Abs(max),
                //                ShootType = ParseShootType(shootEffect)
                //            }));
                //        }
                //        else if (attack.ContainsKey("radius"))
                //        {

                //            attacks.Add(new DistanceAreaCombatAttack(ParseDamageType(attackName), new CombatAttackOption
                //            {
                //                Chance = chance,
                //                Range = range,
                //                MinDamage = (ushort)Math.Abs(min),
                //                MaxDamage = (ushort)Math.Abs(max),
                //                Radius = radius,
                //                ShootType = ParseShootType(shootEffect)
                //            }));
                //        }
                //        else
                //        {
                //            attacks.Add(new DistanceCombatAttack(ParseDamageType(attackName), new CombatAttackOption
                //            {
                //                Chance = chance,
                //                Range = range,
                //                MinDamage = (ushort)Math.Abs(min),
                //                MaxDamage = (ushort)Math.Abs(max),
                //                ShootType = ParseShootType(shootEffect)
                //            }));
                //        }
                //    }
                //    else if (attack.ContainsKey("spread"))
                //    {
                //        attack.TryGetValue("length", out byte length);
                //        attack.TryGetValue("spread", out byte spread);


                //        attacks.Add(new SpreadCombatAttack(ParseDamageType(attackName), new CombatAttackOption
                //        {
                //            Chance = chance,
                //            MinDamage = (ushort)Math.Abs(min),
                //            MaxDamage = (ushort)Math.Abs(max),
                //            Length = length,
                //            Spread = spread
                //        }));

                //    }
                //    else if (ParseDamageType(attackName?.ToString()) == DamageType.Melee)
                //    {
                //        attacks.Add(new MeleeCombatAttack(attackValue, skill));
                //    }
                //}
            }
            return attacks.ToArray();
        }

        private static DamageType ParseDamageType(string type)
        {
            return type switch
            {
                "melee" => DamageType.Melee,
                "physical" => DamageType.Physical,
                "energy" => DamageType.Energy,
                "fire" => DamageType.Fire,
                "manadrain" => DamageType.ManaDrain,
                _ => DamageType.Melee
            };
        }
        private static EffectT ParseAreaEffect(string type)
        {
            return type switch
            {
                "blueshimmer" => EffectT.GlitterBlue,
                "redshimmer" => EffectT.GlitterRed,
                _ => EffectT.None
            };
        }
        private static ShootType ParseShootType(string type)
        {
            return type switch
            {
                "bolt" => ShootType.Bolt,
                "spear" => ShootType.Spear,
                "star" => ShootType.ThrowingStar,
                "energy" => ShootType.Energy,
                "fire" => ShootType.Fire,
                _ => ShootType.None
            };
        }
    }
}