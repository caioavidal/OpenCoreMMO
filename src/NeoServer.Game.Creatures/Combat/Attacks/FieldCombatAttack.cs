using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Items;
using NeoServer.Game.Items.Items;
using NeoServer.Server.Items;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Combat.Attacks
{
    public class FieldCombatAttack : DistanceAreaCombatAttack
    {
        public FieldCombatAttack(CombatAttackOption option) : base(DamageType.FireField, option)
        {
        }

        public override ushort CalculateDamage(ushort attackPower, ushort minAttackPower)
        {
            return 10;
        }

        public override void CauseDamage(ICreature actor, ICreature enemy)
        {
            var field = ItemFactory.Create(1487, enemy.Location, null) as IThing;
            enemy.Tile.AddThing(ref field);

            base.CauseDamage(actor, enemy);
        }
    }
}
