using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Spells
{
    public interface ISpell
    {
        EffectT Effect { get; }

        void Invoke(ICombatActor actor);
    }
}
