using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Combat.Attacks;
using NeoServer.Game.Creatures.Model.Monsters;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using NeoServer.Server.Helpers.Extensions;

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

                    monster.Attacks.Add(new DistanceCombatAttack(ParseDamageType(attackName), new CombatAttackOption
                    {
                        Chance = chance,
                        Range = range,
                        Min = (ushort)Math.Abs(min),
                        Max = (ushort)Math.Abs(max),
                        ShootType = ShootType.Bolt
                    }));


                }
                else if(ParseDamageType(attackName?.ToString()) == DamageType.Melee)
                {

                    monster.Attacks.Add(new MeleeCombatAttack(attackValue, skill));
                  
                }
            }

            return monster;
        }

        private static DamageType ParseDamageType(string type)
        {
            return type switch
            {
                "melee" => DamageType.Melee,
                "physical" => DamageType.Physical,
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
                _ => ShootType.None
            };
        }
    }
}
