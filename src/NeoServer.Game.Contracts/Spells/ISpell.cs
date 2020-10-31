using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Creatures.Players;
using NeoServer.Game.Enums.Spells;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Spells
{
    public interface ISpell
    {
        EffectT Effect { get; }
        ConditionType ConditionType { get; }
        ushort Mana { get; set; }

        bool Invoke(ICombatActor actor, out SpellError error);
    }
}
