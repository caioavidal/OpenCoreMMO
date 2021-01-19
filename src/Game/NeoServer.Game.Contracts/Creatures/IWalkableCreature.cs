using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Concurrent;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate bool PathFinder(IWalkableCreature creature, Location target, FindPathParams options, ITileEnterRule tileEnterRule, out Direction[] directions);
    public delegate void StartFollow(IWalkableCreature creature, ICreature following, FindPathParams fpp);
    public delegate void ChangeSpeed(IWalkableCreature creature, ushort speed);
    public delegate bool CanGoToDirection(Location location, Direction direction, ITileEnterRule rule);
    public delegate void TeleportTo(IWalkableCreature creature, Location location);
    public delegate void Moved(IWalkableCreature creature, Location fromLocation, Location toLocation, ICylinderSpectator[] spectators);
    
    public interface IWalkableCreature: ICreature
    {
        uint EventWalk { get; set; }
        uint Following { get; }
        bool HasNextStep { get; }
        bool IsFollowing { get; }
        ushort Speed { get; }
        int StepDelayMilliseconds { get; }
        IDynamicTile Tile { get; set; }
        ConcurrentQueue<Direction> WalkingQueue { get; }
        bool FollowCreature { get; }
        uint FollowEvent { get; set; }
        bool FirstStep { get; }

        event StartWalk OnStartedWalking;
        event StopWalk OnStoppedWalking;
        event OnTurnedToDirection OnTurnedToDirection;
        event StartFollow OnStartedFollowing;
        event ChangeSpeed OnChangedSpeed;
        event StopWalk OnCompleteWalking;
        event TeleportTo OnTeleported;
        event Moved OnCreatureMoved;

        void DecreaseSpeed(ushort speedBoost);
        byte[] GetRaw(IPlayer playerRequesting);
        void IncreaseSpeed(ushort speed);
        void OnMoved(IDynamicTile fromTile, IDynamicTile toTile, ICylinderSpectator[] spectators);
        void Follow(ICreature creature);
        void StopFollowing();
        void StopWalking();
        bool TryGetNextStep(out Direction direction);
        bool TryUpdatePath(Direction[] newPath);
        bool TryWalkTo(params Direction[] directions);
        void TurnTo(Direction direction);
        void StartFollowing(ICreature creature, FindPathParams fpp);
        bool WalkTo(Location location);
        bool WalkTo(Location location, Action<ICreature> callbackAction);
        void TeleportTo(Location location);
    }
}
