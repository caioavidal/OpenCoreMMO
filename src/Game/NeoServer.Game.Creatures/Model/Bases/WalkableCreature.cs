using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Monsters;
using NeoServer.Game.DataStore;

namespace NeoServer.Game.Creatures.Model.Bases
{
    public abstract class WalkableCreature : Creature, IWalkableCreature
    {
        private uint lastStepCost = 1;
        private readonly Queue<Direction> WalkingQueue = new();

        protected WalkableCreature(ICreatureType type, IOutfit outfit = null, uint healthPoints = 0) : base(type,
            outfit, healthPoints)
        {
            Speed = type.Speed;
            OnCompleteWalking += ExecuteNextAction;
        }

        private IPathFinder pathFinder;
        public virtual IPathFinder PathFinder
        {
            protected get => pathFinder ?? GameToolStore.PathFinder;
            init => pathFinder = value;
        }

        protected virtual IWalkToMechanism WalkToMechanism => GameToolStore.WalkToMechanism;


        protected CooldownList Cooldowns { get; } = new();

        public virtual ITileEnterRule TileEnterRule => CreatureEnterTileRule.Rule;
        public bool HasFollowPath { get; private set; }
        public virtual FindPathParams PathSearchParams => new(!HasFollowPath, true, true, false, 12, 1, 1, false);
        public virtual ushort Speed { get; protected set; }
        public ICreature Following { get; private set; }
        public bool IsFollowing => Following is not null;
        public bool HasNextStep => WalkingQueue.Count > 0;

        public virtual void OnMoved(IDynamicTile fromTile, IDynamicTile toTile, ICylinderSpectator[] spectators)
        {
            lastStepCost = 1;

            if (fromTile.Location.Z != toTile.Location.Z || fromTile.Location.IsDiagonalMovement(toTile.Location))
                lastStepCost = 2;
            SetDirection(fromTile.Location.DirectionTo(toTile.Location));

            if (WalkingQueue.IsEmpty()) OnCompleteWalking?.Invoke(this);
            OnCreatureMoved?.Invoke(this, fromTile.Location, toTile.Location, spectators);
        }

        public void TurnTo(Direction direction)
        {
            if (direction == Direction) return;
            SetDirection(direction);
            OnTurnedToDirection?.Invoke(this, direction);
        }

        public int StepDelay
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

            Following = null;
            HasFollowPath = false;
            StopWalking();
        }

        public virtual void Follow(ICreature creature)
        {
            Follow(creature, PathSearchParams);
        }

        public void Follow(ICreature creature, FindPathParams fpp)
        {
            if (Speed == 0) return;
            if (creature is null) return;

            if (IsFollowing)
            {
                Following = creature;
                StartFollowing(creature);
                return;
            }

            Following = creature;
            StartFollowing(creature);
            OnStartedFollowing?.Invoke(this, creature, fpp);
        }

        public virtual bool WalkTo(params Direction[] directions)
        {
            return TryWalkTo(directions);
        }

        public virtual bool WalkTo(Location location)
        {
            StopWalking();
            if (PathFinder.Find(this, location, PathSearchParams, TileEnterRule, out var directions))
                return TryWalkTo(directions);
            return false;
        }

        public virtual bool WalkTo(Location location, Action<ICreature> callbackAction)
        {
            StopWalking();
            if (PathFinder.Find(this, location, PathSearchParams, TileEnterRule, out var directions))
            {
                NextAction = callbackAction;
                return TryWalkTo(directions);
            }

            return false;
        }

        public virtual bool WalkRandomStep()
        {
            var direction = GetRandomStep();

            if (direction == Direction.None) return false;

            TryWalkTo(direction);

            return true;
        }

        public void SetCurrentTile(IDynamicTile tile)
        {
            Tile = tile;
        }

        public virtual Direction GetNextStep()
        {
            if (!TryGetNextStep(out var direction)) return Direction.None;

            return direction;
        }

        public virtual void TeleportTo(Location location)
        {
            OnTeleported?.Invoke(this, location);
        }

        public virtual void TeleportTo(ushort x, ushort y, byte z)
        {
            OnTeleported?.Invoke(this, new Location(x, y, z));
        }

        public byte[] GetRaw(IPlayer playerRequesting)
        {
            return CreatureRaw.Convert(playerRequesting, this);
        }

        public void IncreaseSpeed(ushort speed)
        {
            ChangeSpeed(speed + Speed);
        }

        public void DecreaseSpeed(ushort speedBoost)
        {
            ChangeSpeed(Math.Max(0, Speed - speedBoost));
        }

        public virtual void OnCreatureDisappear(ICreature creature)
        {
            StopFollowing();
        }

        public void StartFollowing(ICreature creature)
        {
            if (!CanSee(creature.Location, 9, 9))
            {
                OnCreatureDisappear(creature);
                return;
            }

            if (!PathFinder.Find(this, creature.Location, PathSearchParams, TileEnterRule, out var directions))
            {
                HasFollowPath = false;
                return;
            }

            HasFollowPath = true;
            TryUpdatePath(directions);
        }

        public virtual bool TryWalkTo(params Direction[] directions)
        {
            if (Speed == 0) return false;

            if (!WalkingQueue.IsEmpty()) WalkingQueue.Clear();

            if (directions.Length >= 1 && Cooldowns.Expired(CooldownType.Move)) FirstStep = true;

            foreach (var direction in directions)
            {
                if (direction == Direction.None) continue;

                WalkingQueue.Enqueue(direction);
            }

            if (WalkingQueue.IsEmpty()) return true;

            OnStartedWalking?.Invoke(this);
            return true;
        }

        protected Direction GetRandomStep()
        {
            return PathFinder.FindRandomStep(this, TileEnterRule);
        }

        public bool TryUpdatePath(Direction[] newPath)
        {
            if (newPath.Length == 0) return false;
            if (!Cooldowns.Expired(CooldownType.UpdatePath)) return false;

            Cooldowns.Start(CooldownType.UpdatePath, 1000);

            TryWalkTo(newPath);

            return true;
        }

        private bool TryGetNextStep(out Direction direction)
        {
            if (WalkingQueue.TryDequeue(out direction))
            {
                FirstStep = false;
                Cooldowns.Start(CooldownType.Move, StepDelay);

                return true;
            }

            return false;
        }

        public void ChangeSpeed(int newSpeed)
        {
            Speed = (ushort)newSpeed;
            OnChangedSpeed?.Invoke(this, Speed);
        }

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
    }
}