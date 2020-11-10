using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Effects.Explosion;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoServer.Game.Creatures.Combat.Attacks
{
    public class DistanceAreaCombatAttack : DistanceCombatAttack, IDistanceAreaCombatAttack, IAreaAttack
    {

        public DistanceAreaCombatAttack(DamageType damageType, CombatAttackOption option) : base(damageType, option)
        {
        }

        public override void BuildAttack(ICombatActor actor, ICombatActor enemy)
        {
            var i = 0;

            var affectedLocations = ExplosionEffect.Create(Radius);
            AffectedArea = new Coordinate[affectedLocations.Count()];

            foreach (var location in affectedLocations)
            {
                AffectedArea[i++] = enemy.Location.Translate() + location;
            }

            base.BuildAttack(actor, enemy);
        }

        public override ushort CalculateDamage(ushort attackPower, ushort minAttackPower) => base.CalculateDamage(Option.MaxDamage, Option.MinDamage);

        public byte Radius => Option.Radius;

        public Coordinate[] AffectedArea { get; private set; }
    }
}
