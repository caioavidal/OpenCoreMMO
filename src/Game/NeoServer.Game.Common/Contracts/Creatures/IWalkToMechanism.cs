using System;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.Creatures
{
    public interface IWalkToMechanism
    {
        void WalkTo(IPlayer player, Action action, Location.Structs.Location toLocation, bool secondChance = false);
    }
}