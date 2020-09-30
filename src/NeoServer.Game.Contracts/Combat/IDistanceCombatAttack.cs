using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Combat
{
    public interface IDistanceCombatAttack
    {
        ShootType ShootType { get; }
    }
}
