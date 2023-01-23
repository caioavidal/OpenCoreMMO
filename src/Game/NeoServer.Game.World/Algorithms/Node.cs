using System.Collections.Generic;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.World.Algorithms;

public struct Node
{
    private readonly List<AStarNode> nodes = new(512);
    private readonly Dictionary<AStarNode, int> nodesIndexMap = new();
    private readonly Dictionary<AStarPosition, AStarNode> nodesMap = new();
    private readonly bool[] openNodes = new bool[512];
    private readonly AStarNode startNode;
    public int currentNode;

    public Node(Location location)
    {
        currentNode = 1;
        ClosedNodes = 0;
        openNodes[0] = true;

        startNode = new AStarNode(location.X, location.Y)
        {
            F = 0
        };

        nodes.Add(startNode);
        nodesIndexMap.Add(startNode, nodes.Count - 1);
        nodesMap.Add(new AStarPosition(location.X, location.Y), nodes[0]);
    }

    public int ClosedNodes { get; private set; }

    public AStarNode GetBestNode()
    {
        var bestNodeF = int.MaxValue;
        var bestNode = -1;
        for (var i = 0; i < currentNode; ++i)
        {
            if (!openNodes[i]) continue;

            var diffNode = nodes[i].F + nodes[i].Heuristic;

            if (diffNode < bestNodeF)
            {
                bestNodeF = diffNode;
                bestNode = i;
            }
        }

        return bestNode != -1 ? nodes[bestNode] : null;
    }

    internal void CloseNode(AStarNode node)
    {
        var index = 0;
        var start = 0;
        while (true)
        {
            index = nodesIndexMap[node];
            if (openNodes[index] == false)
                start = ++index;
            else
                break;
        }

        if (index >= 512) return;

        openNodes[index] = false;
        ++ClosedNodes;
    }

    internal AStarNode CreateOpenNode(AStarNode parent, int x, int y, int newF, int heuristic, int extraCost)
    {
        if (currentNode >= 512) return null;

        var retNode = currentNode++;
        openNodes[retNode] = true;

        var node = new AStarNode(x, y)
        {
            F = newF,
            Heuristic = heuristic,
            ExtraCost = extraCost,
            Parent = parent
        };
        nodes.Add(node);

        nodesIndexMap.Add(node, nodes.Count - 1);
        nodesMap.TryAdd(new AStarPosition(node.X, node.Y), node);
        return node;
    }

    internal AStarNode GetNodeByPosition(Location location)
    {
        nodesMap.TryGetValue(new AStarPosition(location.X, location.Y), out var foundNode);
        return foundNode;
    }

    internal void OpenNode(AStarNode node)
    {
        var index = nodesIndexMap[node];

        if (index >= 512) return;

        ClosedNodes -= openNodes[index] ? 0 : 1;
        openNodes[index] = true;
    }
}