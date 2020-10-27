using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Combat
{
    public interface ICombatDefense : IProbability
    {
        ushort Interval { get; set; }
        EffectT Effect { get; set; }

        void Defende(ICreature actor);
    }
}
