using System;
using System.Collections.Generic;
using NeoServer.OTB.DataStructures;
using NeoServer.OTB.Enums;

namespace NeoServer.OTB.Structure;

public class OTBNode
{
    private readonly List<OTBNode> children;
    private readonly List<byte> data;

    /// <summary>
    ///     The type of the node.
    /// </summary>
    public readonly NodeType Type;

    /// <summary>
    ///     Creates a new instance of a <see cref="OTBNode" />.
    /// </summary>
    public OTBNode(NodeType type)
    {
        children = new List<OTBNode>();
        data = new List<byte>();
        Type = type;
    }

    /// <summary>
    ///     The children of this node.
    /// </summary>
    public ReadOnlyArray<OTBNode> Children => ReadOnlyArray<OTBNode>.WrapCollection(children.ToArray());

    /// <summary>
    ///     The data of this node.
    /// </summary>
    public ReadOnlyMemory<byte> Data => data.ToArray();

    /// <summary>
    ///     Adds child node
    /// </summary>
    /// <param name="node"></param>
    public void AddChild(OTBNode node)
    {
        children.Add(node);
    }

    /// <summary>
    ///     Adds byte to node's data
    /// </summary>
    /// <param name="b">The byte data to add</param>
    public void AddData(byte b)
    {
        data.Add(b);
    }
}