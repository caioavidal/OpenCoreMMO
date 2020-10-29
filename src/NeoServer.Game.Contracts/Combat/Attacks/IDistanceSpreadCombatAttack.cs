using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Combat
{
    public interface IDistanceSpreadCombatAttack: IAreaAttack, IDistanceCombatAttack
    {
        byte Length { get; }
        byte Spread { get; }
    }
}
