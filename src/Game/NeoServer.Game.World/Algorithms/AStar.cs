using System;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.World.Algorithms;

public class AStar
{
    public bool GetPathMatching(IMap map, ICreature creature, Location targetPos, FindPathParams
        fpp, ITileEnterRule tileEnterRule, out Direction[] directions)
    {
        var pos = creature.Location;
        directions = Array.Empty<Direction>();
        var endPos = new Location();
        var bestMatch = 0;
        var node = new Node(pos);
        var startPos = pos;
        var sX = Math.Abs(targetPos.X - pos.X);
        var sY = Math.Abs(targetPos.Y - pos.Y);
        AStarNode found = null;

        while (fpp.MaxSearchDist != 0 || node.ClosedNodes < 100)
        {
            var bestNode = node.GetBestNode();
            if (bestNode is null)
            {
                if (found is not null) break;
                return false;
            }

            var x = bestNode.X;
            var y = bestNode.Y;

            pos.X = (ushort)x;
            pos.Y = (ushort)y;

            if (AStarCondition.Validate(map, startPos, pos, targetPos, ref bestMatch, fpp))
            {
                found = bestNode;
                endPos = pos;
                if (bestMatch == 0) break;
            }

            var result =  AStarNeighbors.GetDirectionsAndNeighbors(bestNode);
            
            var f = bestNode.F;
            for (var i = 0; i < result.dirCount; ++i)
            {
                pos.X = (ushort)(x + result.neighbors[i, 0]);
                pos.Y = (ushort)(y + result.neighbors[i, 1]);

                if (fpp.CannotWalk(startPos, pos)) continue;
                if (fpp.KeepDistance && !map.IsInRange(startPos, pos, targetPos, fpp)) continue;

                var neighborNode = node.GetNodeByPosition(pos);
                var tile = map[pos];

                if (neighborNode is null && !tileEnterRule.ShouldIgnore(tile, creature)) continue;
                
                var extraCost = CalculateExtraCost(creature, neighborNode, tile, bestNode);
                var cost = bestNode.GetMapWalkCost(pos);
                var newF = f + cost + extraCost;

                if (neighborNode is not null && neighborNode.F <= newF) continue;
                
                if (neighborNode is not null) {
                    neighborNode.F = newF;
                    neighborNode.Parent = bestNode;
                    node.OpenNode(neighborNode);
                    continue;
                } 
                
                var dX = Math.Abs(targetPos.X - pos.X);
                var dY = Math.Abs(targetPos.Y - pos.Y);
                
                neighborNode = node.CreateOpenNode(bestNode, pos.X, pos.Y, newF,
                    ((dX - sX) << 3) + ((dY - sY) << 3) + (Math.Max(dX, dY) << 3), extraCost);

                if (neighborNode is not null) continue;
                if (found is not null) break;
                
                return false;
            }

            node.CloseNode(bestNode);
        }

        if (found is null) return false;
        
        directions = AStarDirections.GetAll(found, startPos, endPos);
        return true;
    }

    private static int CalculateExtraCost(ICreature creature, AStarNode neighborNode, ITile tile,
        AStarNode bestNode)
    {
        if (neighborNode is not null)
        {
            return neighborNode.ExtraCost;
        }
        
        if (tile is IDynamicTile walkableTile)
        {
           return bestNode.GetTileWalkCost(creature, walkableTile);
        }

        return 0;
    }
}