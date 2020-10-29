using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Enums.Location;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void StartFollow(uint creatureId);
    public interface IWalkableCreature: ICreature
    {
        uint EventWalk { get; set; }
        uint Following { get; }
        bool HasNextStep { get; }
        bool IsFollowing { get; }
        double LastStep { get; }
        ushort Speed { get; }
        int StepDelayMilliseconds { get; }
        IWalkableTile Tile { get; set; }
        ConcurrentQueue<Direction> WalkingQueue { get; }
        bool FollowCreature { get; }

        event StartWalk OnStartedWalking;
        event StopWalk OnStoppedWalking;
        event OnTurnedToDirection OnTurnedToDirection;
        event StartFollow OnStartedFollowing;

        void DecreaseSpeed(ushort speedBoost);
        byte[] GetRaw(IPlayer playerRequesting);
        void IncreaseSpeed(ushort speed);
        void Moved(ITile fromTile, ITile toTile);
        void StartFollowing(uint id, params Direction[] pathToCreature);
        void StopFollowing();
        void StopWalking();
        bool TryGetNextStep(out Direction direction);
        bool TryUpdatePath(Direction[] newPath);
        bool TryWalkTo(params Direction[] directions);
        void TurnTo(Direction direction);
        void UpdateLastStepInfo(bool wasDiagonal = true);
    }
}
