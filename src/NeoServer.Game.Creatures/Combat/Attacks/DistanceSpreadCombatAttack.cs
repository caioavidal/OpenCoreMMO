using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Model.Monsters;
using NeoServer.Game.Effects.Magical;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoServer.Game.Creatures.Combat.Attacks
{
    public class SpreadCombatAttack : DistanceCombatAttack, IDistanceSpreadCombatAttack, IAreaAttack
    {
        public SpreadCombatAttack(DamageType damageType, CombatAttackOption option) : base(damageType, option)
        {
        }

        public override void BuildAttack(ICreature actor)
        {
            var i = 0;

            var affectedLocations = SpreadEffect.Create(actor.Direction, Length);
            AffectedArea = new Location[affectedLocations.Count()];

            foreach (var location in SpreadEffect.Create(actor.Direction, Length))
            {
                AffectedArea[i++] = actor.Location + location;
            }

            base.BuildAttack(actor);
        }

        public override ushort CalculateDamage(ushort attackPower, ushort minAttackPower) => base.CalculateDamage(Option.MaxDamage, Option.MinDamage);

        public override void CauseDamage(ICreature actor, ICreature enemy)
        {
            base.CauseDamage(actor, enemy);
        }

        public byte Spread => Option.Spread;
        public byte Length => Option.Length;

        public Location[] AffectedArea { get; private set; }
    }
}
