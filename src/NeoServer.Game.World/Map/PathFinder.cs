using NeoServer.Game.Contracts;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.World.PathFinding;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NeoServer.Game.World.Map
{
    public class PathFinder
    {
        public PathNode Start { get; set; }
        public PathNode Target { get; set; }
        public Direction[] Find(Location start, Location target)
        {

            Start = new PathNode(start);
            Target = new PathNode(target);

            var a = new AStar(Start, Target, 12);
            a.Run();

            var path = a.GetPath().ToList();

            var directionsCount = path.Count - 1;

            var pool = ArrayPool<Direction>.Shared;
            var directions = pool.Rent(directionsCount);

            for (int i = 0; i < path.Count - 1; i++)
            {
                var from = (path[i] as PathNode).Location;
                var dest = (path[i + 1] as PathNode).Location;

                directions[i] = from.DirectionTo(dest);
            }
            pool.Return(directions);

            return directions[0..directionsCount];
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

        private static int[] childXPos = new int[] { 0, -1, 1, 0, };
        private static int[] childYPos = new int[] { -1, 0, 0, 1, };
    }
}
