using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Creatures.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures.Events
{
    public class CreaturePropagatedAttackEventHandler : IGameEventHandler
    {
        private readonly IMap map;
        public CreaturePropagatedAttackEventHandler(IMap map)
        {
            this.map = map;
        }
        public void Execute(ICombatActor actor, CombatDamage damage, Coordinate[] area)
        {
            map.PropagateAttack(actor, damage, area);
        }
    }
}
