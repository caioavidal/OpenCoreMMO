using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Contracts.Combat.Attacks;
using NeoServer.Game.Creatures.Combat.Attacks;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Server.Helpers.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NeoServer.Game.Common.Parsers;

namespace NeoServer.Loaders.Monsters.Converters
{
    class MonsterAttackConverter
    {
        public static IMonsterCombatAttack[] Convert(MonsterData data)
        {
            if (data.Attacks is null) return new IMonsterCombatAttack[0];

            var attacks = new List<IMonsterCombatAttack>();

            foreach (var attack in data.Attacks)
            {
                attack.TryGetValue("name", out string attackName);
                attack.TryGetValue("attack", out ushort attackValue);
                attack.TryGetValue("skill", out ushort skill);
                attack.TryGetValue("min", out decimal min);
                attack.TryGetValue("max", out decimal max);
                attack.TryGetValue("chance", out byte chance);
                attack.TryGetValue("interval", out ushort interval);
                attack.TryGetValue("length", out byte length);
                attack.TryGetValue("radius", out byte radius);
                attack.TryGetValue("spread", out byte spread);
                attack.TryGetValue("target", out byte target);
                attack.TryGetValue("range", out byte range);

                attack.TryGetValue<JArray>("attributes", out var attributesArray);
                var attributes = attributesArray?.ToDictionary(k => ((JObject)k).Properties().First().Name, v => v.Values().First().Value<object>());

                attributes.TryGetValue("shootEffect", out string shootEffect);
                attributes.TryGetValue("areaEffect", out string areaEffect);

                var combatAttack = new MonsterCombatAttack()
                {
                    Chance = chance > 100 || chance <= 0 ? (byte)100 : chance,
                    Interval = interval,
                    MaxDamage = (ushort)Math.Abs(max),
                    MinDamage = (ushort)Math.Abs(min),
                    Target = target,
                    DamageType = MonsterAttributeParser.ParseDamageType(attackName),
                };

                if (combatAttack.IsMelee)
                {
                    
                    combatAttack.MinDamage = (ushort)Math.Abs(min);
                    combatAttack.MaxDamage = Math.Abs(max) > 0 ? (ushort)Math.Abs(max) : MeleeCombatAttack.CalculateMaxDamage(skill, attackValue);

                    combatAttack.CombatAttack = new MeleeCombatAttack();

                    if (attack.TryGetValue("fire", out ushort value))
                    {
                        combatAttack.CombatAttack = new MeleeCombatAttack(value,value, ConditionType.Fire, 9000);
                    }
                    else if (attack.TryGetValue("poison", out value))
                    {
                        combatAttack.CombatAttack = new MeleeCombatAttack(value, value, ConditionType.Poison, 4000);
                    }
                    else if (attack.TryGetValue("energy", out value))
                    {
                        combatAttack.CombatAttack = new MeleeCombatAttack(value, value, ConditionType.Energy, 10000);
                    }
                    else if (attack.TryGetValue("drown", out value))
                    {
                        combatAttack.CombatAttack = new MeleeCombatAttack(value, value, ConditionType.Drown, 5000);
                    }
                    else if (attack.TryGetValue("freeze", out value))
                    {
                        combatAttack.CombatAttack = new MeleeCombatAttack(value, value, ConditionType.Freezing, 8000);
                    }
                    else if (attack.TryGetValue("dazzle", out value))
                    {
                        combatAttack.CombatAttack = new MeleeCombatAttack(value, value, ConditionType.Dazzled, 10000);
                    }
                    else if (attack.TryGetValue("curse", out value))
                    {
                        combatAttack.CombatAttack = new MeleeCombatAttack(value, value, ConditionType.Cursed, 4000);
                    }
                    else if (attack.TryGetValue("bleed", out value) || attack.TryGetValue("physical", out value))
                    {
                        combatAttack.CombatAttack = new MeleeCombatAttack(value, value, ConditionType.Bleeding , 4000);
                    }

                    if (attack.TryGetValue("tick", out ushort tick) && combatAttack.CombatAttack is MeleeCombatAttack melee) melee.ConditionInterval = tick;
                }
                if (range > 1 || radius == 1)
                {
                    if (areaEffect != null)
                        combatAttack.DamageType = MonsterAttributeParser.ParseDamageType(areaEffect);
                    combatAttack.CombatAttack = new DistanceCombatAttack(range, ShootTypeParser.Parse(shootEffect));
                }
                if (radius > 1)
                {
                    combatAttack.DamageType = MonsterAttributeParser.ParseDamageType(areaEffect);
                    combatAttack.CombatAttack = new DistanceAreaCombatAttack(range, radius, ShootTypeParser.Parse(shootEffect));
                }
                if (length > 0)
                {
                    combatAttack.DamageType = MonsterAttributeParser.ParseDamageType(areaEffect);
                    combatAttack.CombatAttack = new SpreadCombatAttack(length, spread);
                }

                if(attackName == "lifedrain")
                {
                    var shootType = ShootTypeParser.Parse(shootEffect);

                    combatAttack.CombatAttack = new DrainCombatAttack(range, radius, shootType);
                }
                if (attackName == "manadrain")
                {
                    var shootType = ShootTypeParser.Parse(shootEffect);

                    combatAttack.CombatAttack = new DrainCombatAttack(range, radius, shootType);
                }

                if (attackName == "speed")
                {
                    attack.TryGetValue("duration", out uint duration);
                    attack.TryGetValue("speedchange", out short speedchange);

                    combatAttack.DamageType = default;
                    combatAttack.CombatAttack = new SpeedCombatAttack(duration, speedchange, range, ShootTypeParser.Parse(shootEffect));
                }

                if (combatAttack.CombatAttack is null)
                {
             //       Console.WriteLine($"{attackName} was not created on monster: {data.Name}");
                }

                attacks.Add(combatAttack);
            }
            return attacks.ToArray();
        }

    }
}
