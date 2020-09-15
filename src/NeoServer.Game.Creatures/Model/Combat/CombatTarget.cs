using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Model.Combat
{
    public class CombatTarget
    {
        public ICreature Creature { get; set; }
        public bool CanReachCreature { get; private set; } = true;

        public CombatTarget(ICreature creature)
        {
            Creature = creature;
        }

        public bool SetAsUnreachable() => CanReachCreature = false;

    }
}
