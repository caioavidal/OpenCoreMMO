using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Creatures.Enums;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface INeedsCooldowns
    {
        void StartSpellCooldown(ISpell spell);
    }
}
