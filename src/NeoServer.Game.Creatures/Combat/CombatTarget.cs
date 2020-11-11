using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Location;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Model.Combat
{
    public class CombatTarget
    {
        public ICombatActor Creature { get; set; }
        public Direction[] PathToCreature { get; private set; }
        public bool CanReachCreature { get; private set; } = true;
        public bool CanSee { get; set; } = true;
        public CombatTarget(ICombatActor creature)
        {
            Creature = creature;
        }

        public void SetAsUnreachable() => CanReachCreature = false;
        public void SetAsReachable(Direction[] path)
        {
            CanReachCreature = true;
            PathToCreature = path;
        }
    }
}
