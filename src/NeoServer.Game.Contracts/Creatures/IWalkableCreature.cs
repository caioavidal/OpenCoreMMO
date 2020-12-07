using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate bool PathFinder(IWalkableCreature creature, Location target, FindPathParams options, out Direction[] directions);
    public delegate void StartFollow(IWalkableCreature creature, IWalkableCreature following, FindPathParams fpp);
    public delegate void ChangeSpeed(IWalkableCreature creature, ushort speed);
    public delegate bool CanGoToDirection(Location location, Direction direction, ITileEnterRule rule);

    public interface IWalkableCreature: ICreature
    {
        uint EventWalk { get; set; }
        uint Following { get; }
        bool HasNextStep { get; }
        bool IsFollowing { get; }
        double LastStep { get; }
        ushort Speed { get; }
        int StepDelayMilliseconds { get; }
        IDynamicTile Tile { get; set; }
        ConcurrentQueue<Direction> WalkingQueue { get; }
        bool FollowCreature { get; }
        uint FollowEvent { get; set; }

        event StartWalk OnStartedWalking;
        event StopWalk OnStoppedWalking;
        event OnTurnedToDirection OnTurnedToDirection;
        event StartFollow OnStartedFollowing;
        event ChangeSpeed OnChangedSpeed;

        void DecreaseSpeed(ushort speedBoost);
        byte[] GetRaw(IPlayer playerRequesting);
        void IncreaseSpeed(ushort speed);
        void OnMoved(IDynamicTile fromTile, IDynamicTile toTile);
        void Follow(IWalkableCreature creature);
        void StopFollowing();
        void StopWalking();
        bool TryGetNextStep(out Direction direction);
        bool TryUpdatePath(Direction[] newPath);
        bool TryWalkTo(params Direction[] directions);
        void TurnTo(Direction direction);
        void UpdateLastStepInfo(bool wasDiagonal = true);
        void StartFollowing(IWalkableCreature creature, FindPathParams fpp);
        bool WalkTo(Location location);
    }
}
