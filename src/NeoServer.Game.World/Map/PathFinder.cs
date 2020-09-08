using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Buffers;
using System.Collections.Generic;

namespace NeoServer.Game.World.Map
{
    public class PathFinder
    {
        public PathNode Start { get; set; }
        public PathNode Target { get; set; }
        public Direction[] Find(IMap map, ICreature creature, Location start, Location target)
        {
            var AStarTibia = new AStarTibia();
            AStarTibia.MaxSteps = 50;
            return AStarTibia.GetPathMatching(map, creature, start, target, new FindPathParams(true, true, true, false, 0, 0, 1));
        }
    }

    public class PathNode : INode
    {
        public PathNode(Location location)
        {
            //  _grid = grid;
            Location = location;
        }
        public bool IsInOpenList { get; set; } = false;
        public bool IsInClosedList { get; set; } = false;

        public Location Location { get; set; }

        public int TotalCost => MovementCost + EstimatedCost;

        public int MovementCost { get; private set; }

        public int EstimatedCost { get; private set; }

        public INode Parent { get; set; }

        public IEnumerable<INode> Children
        {
            get
            {
                var pool = ArrayPool<PathNode>.Shared;
                var children = pool.Rent(Location.Neighbours.Length);

                var i = 0;
                foreach (var neighbour in Location.Neighbours)
                {
                    children[i++] = new PathNode(neighbour);

                }
                pool.Return(children);
                return children[0..Location.Neighbours.Length];
            }
        }

        public bool IsGoal(INode goal)
        {
            return IsEqual((PathNode)goal);
        }

        public bool IsEqual(PathNode node)
        {
            return (this == node) || (Location.X == node.Location.X && Location.Y == node.Location.Y);
        }

        public void SetEstimatedCost(INode goal)
        {
            var g = (PathNode)goal;
            EstimatedCost = Math.Abs(this.Location.X - g.Location.X) + Math.Abs(this.Location.Y - g.Location.Y);
        }

        public void SetMovementCost(INode parent)
        {
            MovementCost = Parent.MovementCost + 1;
        }

    }
}
