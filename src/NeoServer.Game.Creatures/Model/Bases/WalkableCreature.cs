using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Model;
using NeoServer.Game.World.Map.Tiles;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Model.Bases
{

    public abstract class WalkableCreature : Creature, IWalkableCreature
    {
        #region Events
        public event StopWalk OnStoppedWalking;
        public event StartWalk OnStartedWalking;
        public event OnTurnedToDirection OnTurnedToDirection;
        public event StartFollow OnStartedFollowing;
        public event ChangeSpeed OnChangedSpeed;
        #endregion

        private readonly object _enqueueWalkLock = new object();
        protected PathFinder FindPathToDestination { get; }

        private uint lastStepCost = 1;

        protected WalkableCreature(ICreatureType type, PathFinder pathFinder, IOutfit outfit = null, uint healthPoints = 0) : base(type, outfit, healthPoints)
        {
            Speed = type.Speed;
            FindPathToDestination = pathFinder;
        }

        protected CooldownList Cooldowns { get; } = new CooldownList();
        public uint EventWalk { get; set; }
        public IWalkableTile Tile { get; set; }
        public ushort Speed { get; protected set; }
        public uint Following { get; private set; }
        public bool IsFollowing => Following > 0;
        public double LastStep { get; private set; }
        public ConcurrentQueue<Direction> WalkingQueue { get; } = new ConcurrentQueue<Direction>();
        public bool HasNextStep => WalkingQueue.Count > 0;
        public bool FollowCreature { get; protected set; }
        public uint FollowEvent { get; set; }

        public void UpdateLastStepInfo(bool wasDiagonal = true)
        {
            var tilePenalty = Tile?.MovementPenalty;
            var totalPenalty = (tilePenalty ?? 200) * (wasDiagonal ? 2 : 1);

            Cooldowns.Start(CooldownType.Move, (int)(1000 * totalPenalty / (double)Math.Max(1, (int)Speed)));
            lastStepCost = 1;
        }

        private int stepDelay
        {
            get
            {
                var stepDuration = CalculateStepDuration() * lastStepCost;
                return (int)(stepDuration - (DateTime.Now.TimeOfDay.TotalMilliseconds - LastStep));
            }
        }
        private int CalculateStepDuration()
        {
            var duration = Math.Floor((double)(1000 * Tile.StepSpeed / Speed));

            var stepDuration = (int)Math.Ceiling(duration / 50) * 50;

            //todo check monster creature.cpp 1367
            return stepDuration;
        }
        public override void Moved(ITile fromTile, ITile toTile)
        {
            LastStep = DateTime.Now.TimeOfDay.TotalMilliseconds;

            lastStepCost = 1;

            if (fromTile.Location.Z != toTile.Location.Z)
            {
                lastStepCost = 2;
            }
            else if (fromTile.Location.IsDiagonalMovement(toTile.Location))
            {
                lastStepCost = 3;
            }
        }
        public void TurnTo(Direction direction)
        {
            Direction = direction;
            OnTurnedToDirection?.Invoke(this, direction);
        }
        public int StepDelayMilliseconds
        {
            get
            {
                if (stepDelay > 0)
                {
                    return stepDelay;
                }

                return (int)(CalculateStepDuration() * lastStepCost);
            }
        }
        public void StopWalking()
        {
            WalkingQueue.Clear(); // reset the actual queue
            UpdateLastStepInfo();

            OnStoppedWalking?.Invoke(this);
        }


        public void StopFollowing()
        {
            Following = 0;
            StopWalking();
        }

        public void StartFollowing(IWalkableCreature creature, FindPathParams fpp)
        {
            Following = creature.CreatureId;
            OnStartedFollowing?.Invoke(this, creature, fpp);
        }
        public void Follow(IWalkableCreature creature, FindPathParams fpp)
        {
            if (!FindPathToDestination(this, creature.Location, fpp, out var directions)) return;
            TryUpdatePath(directions);
        }


        public virtual bool TryWalkTo(params Direction[] directions)
        {
            lock (_enqueueWalkLock)
            {
                if (!WalkingQueue.IsEmpty)
                {
                    WalkingQueue.Clear();
                }
                foreach (var direction in directions)
                {
                    WalkingQueue.Enqueue(direction);
                }
            }

            OnStartedWalking?.Invoke(this);
            return true;
        }

        public bool TryUpdatePath(Direction[] newPath)
        {
            if (!Cooldowns.Expired(CooldownType.UpdatePath)) return false;

            Cooldowns.Start(CooldownType.UpdatePath, 1000);

            TryWalkTo(newPath);

            return true;
        }

        public virtual bool TryGetNextStep(out Direction direction)
        {
            if (!Cooldowns.Expired(CooldownType.Move))
            {
                direction = Direction.None;
                return false;
            }

            if (WalkingQueue.TryDequeue(out direction))
            {
                Cooldowns.Start(CooldownType.Move, StepDelayMilliseconds);
                return true;
            }

            return false;
        }

        public byte[] GetRaw(IPlayer playerRequesting) => CreatureRaw.Convert(playerRequesting, this);

        public void ChangeSpeed(int newSpeed)
        {
            Speed = (ushort)newSpeed;
            OnChangedSpeed?.Invoke(this, Speed);
        }
        public void IncreaseSpeed(ushort speed) => ChangeSpeed(speed + Speed);
        public void DecreaseSpeed(ushort speedBoost) => ChangeSpeed(Speed - speedBoost);

        public void Follow(uint id, params Direction[] pathToCreature)
        {
            throw new NotImplementedException();
        }
    }
}
