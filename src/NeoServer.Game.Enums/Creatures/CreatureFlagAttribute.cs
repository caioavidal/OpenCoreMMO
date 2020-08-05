using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Enums.Creatures
{
    public enum CreatureFlagAttribute : byte
    {
        Summonable,
        Attackable,
        Hostile,
        Illusionable,
        Convinceable,
        Pushable,
        CanPushItems,
        CanPushCreatures,
        TargetDistance,
        StaticAttack,
        RunOnHealth,
    }
}
