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
    public delegate bool CanGoToDirection(ICreature creature, Location location, Direction direction, ITileEnterRule rule);
    public delegate void TeleportTo(IWalkableCreature creature, Location location);
    public delegate void Moved(IWalkableCreature creature, Location fromLocation, Location toLocation, ICylinderSpectator[] spectators);
    
    public interface IWalkableCreature: ICreature
    {
        event StartWalk OnStartedWalking;
        event StopWalk OnStoppedWalking;
        event OnTurnedToDirection OnTurnedToDirection;
        event StartFollow OnStartedFollowing;
        event ChangeSpeed OnChangedSpeed;
        event StopWalk OnCompleteWalking;
        event TeleportTo OnTeleported;
        event Moved OnCreatureMoved;

        uint Following { get; }
        bool HasNextStep { get; }
        bool IsFollowing { get; }
        ushort Speed { get; }
        int StepDelay { get; }
        bool FollowCreatureMode { get; }
        bool FirstStep { get; } //remove

        /// <summary>
        /// Decreases creature speed
        /// </summary>
        void DecreaseSpeed(ushort speed);
        /// <summary>
        /// Increases creature speed
        /// </summary>
        /// <param name="speed"></param>
        void IncreaseSpeed(ushort speed);

        byte[] GetRaw(IPlayer playerRequesting);
        void OnMoved(IDynamicTile fromTile, IDynamicTile toTile, ICylinderSpectator[] spectators); //remove
        /// <summary>
        /// Follow creature
        /// </summary>
        /// <param name="creature"></param>
        void Follow(ICreature creature, FindPathParams findPathParams);
        void StopFollowing();
        /// <summary>
        /// Stops walking
        /// </summary>
        void StopWalking();

        /// <summary>
        /// Walks to a given location
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        bool WalkTo(Location location);
        /// <summary>
        /// Walks to a given location and call a action when finished
        /// </summary>        
        bool WalkTo(Location location, Action<ICreature> callbackAction);
        /// <summary>
        /// Walks to a sequence of direction
        /// </summary>
        /// <param name="directions"></param>
        /// <returns></returns>
        bool WalkTo(params Direction[] directions);
        /// <summary>
        /// Teleport creature to a given location
        /// </summary>
        /// <param name="location"></param>
        void TeleportTo(Location location);
        /// <summary>
        /// Teleport creature to a given coordinate
        /// </summary>
        void TeleportTo(ushort x, ushort y, byte z);
        /// <summary>
        /// Walk one random step
        /// </summary>
        /// <returns></returns>
        bool WalkRandomStep();

        /// <summary>
        /// Get creature's next step direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns>Returns None when there is no next direction</returns>
        Direction GetNextStep();

        /// <summary>
        /// Sets current tile which creature is on
        /// </summary>
        /// <param name="tile"></param>
        void SetCurrentTile(IDynamicTile tile);
        void TurnTo(Direction direction);
        void Follow(ICreature creature);
    }
}
