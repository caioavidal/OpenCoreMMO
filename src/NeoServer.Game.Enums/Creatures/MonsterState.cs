using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Enums.Creatures
{

    /// <summary>
    /// A state machine no control monster
    /// </summary>
    public enum MonsterState
    {
        Sleeping,
        Alive,
        Running,
        LookingForEnemy
    }
}
