using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Combat.Attacks;
using NeoServer.Game.Creatures.Model.Monsters;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using NeoServer.Server.Helpers.Extensions;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace NeoServer.Loaders.World
{
    public class MonsterConverter
    {
        public static IMonsterType Convert(MonsterData monsterData)
        {
            var data = monsterData.Monster;
            var monster = new MonsterType()
            {
                Name = data.Name,
                MaxHealth = data.Health.Max,
                Look = new Dictionary<LookType, ushort>() { { LookType.Type, data.Look.Type }, { LookType.Corpse, data.Look.Corpse } },
                Speed = data.Speed,
                Armor = ushort.Parse(data.Defenses.Armor),
                Defence = ushort.Parse(data.Defenses.Defense),
                Attacks = new List<ICombatAttack>(),
                Experience = data.Experience
            };


            foreach (var attack in data.Attacks)
            {
                attack.TryGetValue("name", out string attackName);
                attack.TryGetValue("attack", out byte attackValue);
                attack.TryGetValue("skill", out byte skill);

                if (attack.ContainsKey("range"))
                {
                    attack.TryGetValue("chance", out byte chance);
                    attack.TryGetValue("range", out byte range);
                    attack.TryGetValue("min", out decimal min);
                    attack.TryGetValue("max", out decimal max);

                    attack.TryGetValue<JArray>("attributes", out var attributes);

                    var shootEffect = attributes.FirstOrDefault(a=>a.Value<string>("key") == "shootEffect").Value<string>("value");

                    if (attack.ContainsKey("radius"))
                    {
                        attack.TryGetValue("radius", out byte radius);

                        monster.Attacks.Add(new DistanceAreaCombatAttack(ParseDamageType(attackName), new CombatAttackOption
                        {
                            Chance = chance,
                            Range = range,
                            Min = (ushort)Math.Abs(min),
                            Max = (ushort)Math.Abs(max),
                            Radius = radius,
                            ShootType = ParseShootType(shootEffect)
                        }));
                    }
                    else
                    {

                        monster.Attacks.Add(new DistanceCombatAttack(ParseDamageType(attackName), new CombatAttackOption
                        {
                            Chance = chance,
                            Range = range,
                            Min = (ushort)Math.Abs(min),
                            Max = (ushort)Math.Abs(max),
                            ShootType = ParseShootType(shootEffect)
                        }));
                    }


                }
                else if (ParseDamageType(attackName?.ToString()) == DamageType.Melee)
                {

                    monster.Attacks.Add(new MeleeCombatAttack(attackValue, skill));

                }
            }

            foreach (var flag in data.Flags)
            {
                var creatureFlag = ParseCreatureFlag(flag.Key);

                monster.Flags.Add(creatureFlag, flag.Value);
            }

            return monster;
        }

        private static DamageType ParseDamageType(string type)
        {
            return type switch
            {
                "melee" => DamageType.Melee,
                "physical" => DamageType.Physical,
                "energy" => DamageType.Energy,
                "fire" => DamageType.Fire,
                _ => DamageType.Melee
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
        private static CreatureFlagAttribute ParseCreatureFlag(string flag)
        {
            return flag switch
            {
                "summonable" => CreatureFlagAttribute.Summonable,
                "attackable" => CreatureFlagAttribute.Attackable,
                "hostile" => CreatureFlagAttribute.Hostile,
                "illusionable" => CreatureFlagAttribute.Illusionable,
                "convinceable" => CreatureFlagAttribute.Convinceable,
                "pushable" => CreatureFlagAttribute.Pushable,
                "canpushitems" => CreatureFlagAttribute.CanPushItems,
                "canpushcreatures" => CreatureFlagAttribute.CanPushCreatures,
                "targetdistance" => CreatureFlagAttribute.TargetDistance,
                "staticattack" => CreatureFlagAttribute.StaticAttack,
                "runonhealth" => CreatureFlagAttribute.RunOnHealth,
                _ => CreatureFlagAttribute.None
            };
        }

    }
}
