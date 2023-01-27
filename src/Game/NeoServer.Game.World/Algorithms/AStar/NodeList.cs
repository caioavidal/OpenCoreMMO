using System.Collections.Generic;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.World.Algorithms.AStar;

internal class NodeList
{
    private readonly List<Node> nodes = new();
    private readonly Dictionary<uint, ushort> nodesMap = new();
    private readonly List<bool> _openNodes = new();
    private int currentNode;

    public NodeList(Location location)
    {
        currentNode = 1;
        ClosedNodes = 0;
        _openNodes.Add(true);

        var startNode = new Node(location.X, location.Y)
        {
            F = 0
        };

        nodes.Add(startNode);
        nodesMap.Add((uint)((startNode.X << 16) | startNode.Y), 0);
    }

    public int ClosedNodes { get; private set; }

    public Node GetBestNode()
    {
        var bestNodeF = int.MaxValue;
        var bestNode = -1;
        for (var i = 0; i < currentNode; ++i)
        {
            if (!_openNodes[i]) continue;

            var diffNode = nodes[i].Weight;

            if (diffNode >= bestNodeF) continue;
            
            bestNodeF = diffNode;
            bestNode = i;
        }

        return bestNode != -1 ? nodes[bestNode] : null;
    }

    internal void CloseNode(Node node)
    {
        var index = nodesMap[(uint)((node.X << 16) | node.Y)];

        if (index >= 512) return;

        _openNodes[index] = false;
        ++ClosedNodes;
    }

    internal Node CreateOpenNode(Node parent, int x, int y, int newF, int heuristic, int extraCost)
    {
        if (currentNode >= 512) return null;

        currentNode++;
        _openNodes.Add(true);

        var node = new Node(x, y)
        {
            F = newF,
            Heuristic = heuristic,
            ExtraCost = extraCost,
            Parent = parent
        };
        
        nodes.Add(node);
        nodesMap.Add((uint)((node.X << 16) | node.Y), (ushort)(nodes.Count - 1));
        
        return node;
    }

    internal Node GetNodeByPosition(Location location) => 
        nodesMap.TryGetValue((uint)((location.X << 16) | location.Y), out var foundNodeIndex) ? nodes[foundNodeIndex] : null;

    internal void OpenNode(Node node)
    {
        var index = nodesMap[(uint)((node.X << 16) | node.Y)];

        if (index >= 512) return;

        ClosedNodes -= _openNodes[index] ? 0 : 1;
        _openNodes[index] = true;
    }
}