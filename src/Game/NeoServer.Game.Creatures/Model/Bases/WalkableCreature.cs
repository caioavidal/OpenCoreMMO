using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Creatures.Monsters;
using NeoServer.Game.DataStore;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

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

        protected virtual IPathFinder PathFinder => ConfigurationStore.PathFinder;

        private uint lastStepCost = 1;
        protected WalkableCreature(ICreatureType type, IOutfit outfit = null, uint healthPoints = 0) : base(type, outfit, healthPoints)
        {
            Speed = type.Speed;
            OnCompleteWalking += ExecuteNextAction;

        }

        protected CooldownList Cooldowns { get; } = new CooldownList();

        public virtual ITileEnterRule TileEnterRule => CreatureEnterTileRule.Rule;
        public virtual ushort Speed { get; protected set; }
        public uint Following { get; private set; }
        public bool IsFollowing => Following > 0;
        private Queue<Direction> WalkingQueue = new Queue<Direction>();
        public bool HasNextStep => WalkingQueue.Count > 0;
        public bool FollowModeEnabled { get; protected set; }
        public bool HasFollowPath { get; private set; }
        public virtual FindPathParams PathSearchParams => new FindPathParams(!HasFollowPath, true, true, false, 12, 1, 1, false);

        public virtual void OnMoved(IDynamicTile fromTile, IDynamicTile toTile, ICylinderSpectator[] spectators)
        {
            lastStepCost = 1;

            if (fromTile.Location.Z != toTile.Location.Z || fromTile.Location.IsDiagonalMovement(toTile.Location))
            {
                lastStepCost = 2;
            }
            if (WalkingQueue.IsEmpty())
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

            Following = 0;
            HasFollowPath = false;
            StopWalking();
        }

        public virtual void OnCreatureDisappear(ICreature creature)
        {
            StopFollowing();
        }

        public void Follow(ICreature creature) => Follow(creature, PathSearchParams);

        public void Follow(ICreature creature, FindPathParams fpp)
        {
            if (Speed == 0) return;
            if (creature is null) return;

            if (IsFollowing)
            {
                Following = creature.CreatureId;
                StartFollowing(creature);
                return;
            }

            Following = creature.CreatureId;
            StartFollowing(creature);
            OnStartedFollowing?.Invoke(this, creature, fpp);
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

        public virtual bool WalkTo(params Direction[] directions)
        {
            return TryWalkTo(directions);
        }
        public virtual bool WalkTo(Location location)
        {
            StopWalking();
            if (PathFinder.Find(this, location, PathSearchParams, TileEnterRule, out var directions))
            {
                return TryWalkTo(directions);
            }
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

        public virtual bool TryWalkTo(params Direction[] directions)
        {
            if (Speed == 0) return false;

            if (!WalkingQueue.IsEmpty())
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

            if (WalkingQueue.IsEmpty()) return true;

            OnStartedWalking?.Invoke(this);
            return true;
        }

        protected Direction GetRandomStep() => PathFinder.FindRandomStep(this, TileEnterRule);

        public virtual bool WalkRandomStep()
        {
            var direction = GetRandomStep();

            if (direction == Direction.None) return false;

            TryWalkTo(direction);

            return true;
        }

        public void SetCurrentTile(IDynamicTile tile) => Tile = tile;

        public bool TryUpdatePath(Direction[] newPath)
        {
            if (newPath.Length == 0) return false;
            if (!Cooldowns.Expired(CooldownType.UpdatePath)) return false;

            Cooldowns.Start(CooldownType.UpdatePath, 1000);

            TryWalkTo(newPath);

            return true;
        }

        public virtual Direction GetNextStep()
        {
            if (!TryGetNextStep(out var direction)) return Direction.None;

            return direction;
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

        public virtual void TeleportTo(Location location)
        {
            OnTeleported?.Invoke(this, location);
        }
        public virtual void TeleportTo(ushort x, ushort y, byte z)
        {
            OnTeleported?.Invoke(this, new Location(x, y, z));
        }
        public byte[] GetRaw(IPlayer playerRequesting) => CreatureRaw.Convert(playerRequesting, this);
        public void ChangeSpeed(int newSpeed)
        {
            Speed = (ushort)newSpeed;
            OnChangedSpeed?.Invoke(this, Speed);
        }
        public void IncreaseSpeed(ushort speed) => ChangeSpeed(speed + Speed);
        public void DecreaseSpeed(ushort speedBoost) => ChangeSpeed(Math.Max(0,(int)Speed - (int)speedBoost));
    }
}
