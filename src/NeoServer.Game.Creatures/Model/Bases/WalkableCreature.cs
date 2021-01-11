using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Creatures.Monsters;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Concurrent;

namespace NeoServer.Game.Creatures.Model.Bases
{
    public abstract class WalkableCreature : Creature, IWalkableCreature
    {
        #region Events
        public event StopWalk OnStoppedWalking;
        public event StopWalk OnCompleteWalking;
        public event StartWalk OnStartedWalking;
        public event OnTurnedToDirection OnTurnedToDirection;
        public event StartFollow OnStartedFollowing;
        public event ChangeSpeed OnChangedSpeed;
        public event TeleportTo OnTeleported;
        public event Moved OnCreatureMoved;
        #endregion

        protected IPathAccess PathAccess { get; }

        private uint lastStepCost = 1;
        protected WalkableCreature(ICreatureType type, IPathAccess pathAccess, IOutfit outfit = null, uint healthPoints = 0) : base(type, outfit, healthPoints)
        {
            Speed = type.Speed;
            PathAccess = pathAccess;
            OnCompleteWalking += ExecuteNextAction;

        }

        protected CooldownList Cooldowns { get; } = new CooldownList();
        public uint EventWalk { get; set; }
       
        public virtual ushort Speed { get; protected set; }
        public uint Following { get; private set; }
        public bool IsFollowing => Following > 0;
        public ConcurrentQueue<Direction> WalkingQueue { get; } = new ConcurrentQueue<Direction>();
        public bool HasNextStep => WalkingQueue.Count > 0;
        public bool FollowCreature { get; protected set; }
        public uint FollowEvent { get; set; }
        public bool HasFollowPath { get; private set; }
        public virtual FindPathParams PathSearchParams => new FindPathParams(!HasFollowPath, true, true, false, 12, 1, 1, false);

        public virtual void OnMoved(IDynamicTile fromTile, IDynamicTile toTile, ICylinderSpectator[] spectators)
        {
            lastStepCost = 1;

            if (fromTile.Location.Z != toTile.Location.Z || fromTile.Location.IsDiagonalMovement(toTile.Location))
            {
                lastStepCost = 2;
            }
            if (WalkingQueue.IsEmpty)
            {
                OnCompleteWalking?.Invoke(this);
            }
            OnCreatureMoved?.Invoke(this, fromTile.Location, toTile.Location, spectators);
        }
        public void TurnTo(Direction direction)
        {
            if (direction == Direction) return;
            SetDirection(direction);
            OnTurnedToDirection?.Invoke(this, direction);
        }
        public int StepDelayMilliseconds
        {
            get
            {
                if (FirstStep)
                    return 0;

                if (Speed == 0) return 0;
                return (int)(Tile.StepSpeed / (decimal)Speed * 1000 * lastStepCost);
            }
        }

        public bool FirstStep { get; private set; }

        public void StopWalking()
        {
            WalkingQueue.Clear();
            OnStoppedWalking?.Invoke(this);
        }

        public void StopFollowing()
        {
            if (!IsFollowing) return;

            Following = 0;
            HasFollowPath = false;
            StopWalking();
        }

        public virtual void OnCreatureDisappear(ICreature creature)
        {
            StopFollowing();
        }

        public void StartFollowing(ICreature creature, FindPathParams fpp)
        {
            if (Speed == 0) return;
            if (creature is null) return;

            if (IsFollowing)
            {
                Following = creature.CreatureId;
                Follow(creature);
                return;
            }

            Following = creature.CreatureId;
            OnStartedFollowing?.Invoke(this, creature, fpp);
        }
        public void Follow(ICreature creature)
        {
            if (!CanSee(creature.Location, 9, 9))
            {
                OnCreatureDisappear(creature);
                return;
            }
            if (!PathAccess.FindPathToDestination(this, creature.Location, PathSearchParams, CreatureEnterTileRule.Rule, out var directions))
            {
                HasFollowPath = false;
                return;
            }
            HasFollowPath = true;
            TryUpdatePath(directions);
        }

        public virtual bool WalkTo(Location location)
        {
            StopWalking();
            if (PathAccess.FindPathToDestination(this, location, PathSearchParams, CreatureEnterTileRule.Rule, out var directions))
            {
                return TryWalkTo(directions);
            }
            return false;
        }
        public virtual bool WalkTo(Location location, Action<ICreature> callbackAction)
        {
            StopWalking();
            if (PathAccess.FindPathToDestination(this, location, PathSearchParams, CreatureEnterTileRule.Rule, out var directions))
            {
                NextAction = callbackAction;
                return TryWalkTo(directions);
            }
            return false;
        }

        public virtual bool TryWalkTo(params Direction[] directions)
        {
            if (Speed == 0) return false;

            if (!WalkingQueue.IsEmpty)
            {
                WalkingQueue.Clear();
            }

            if (directions.Length >= 1 && Cooldowns.Expired(CooldownType.Move))
            {
                FirstStep = true;
            }

            foreach (var direction in directions)
            {
                if (direction == Direction.None) continue;

                WalkingQueue.Enqueue(direction);
            }

            if (WalkingQueue.IsEmpty) return true;

            OnStartedWalking?.Invoke(this);
            return true;
        }

        public bool TryUpdatePath(Direction[] newPath)
        {
            if (newPath.Length == 0) return false;
            if (!Cooldowns.Expired(CooldownType.UpdatePath)) return false;

            Cooldowns.Start(CooldownType.UpdatePath, 1000);

            TryWalkTo(newPath);

            return true;
        }

        public virtual bool TryGetNextStep(out Direction direction)
        {
            if (WalkingQueue.TryDequeue(out direction))
            {
                FirstStep = false;
                Cooldowns.Start(CooldownType.Move, StepDelayMilliseconds);

                return true;
            }

            return false;
        }

        public virtual void TeleportTo(Location location)
        {
            OnTeleported?.Invoke(this, location);
        }

        public byte[] GetRaw(IPlayer playerRequesting) => CreatureRaw.Convert(playerRequesting, this);
        public void ChangeSpeed(int newSpeed)
        {
            Speed = (ushort)newSpeed;
            OnChangedSpeed?.Invoke(this, Speed);
        }
        public void IncreaseSpeed(ushort speed) => ChangeSpeed(speed + Speed);
        public void DecreaseSpeed(ushort speedBoost) => ChangeSpeed(Speed - speedBoost);
    }
}
