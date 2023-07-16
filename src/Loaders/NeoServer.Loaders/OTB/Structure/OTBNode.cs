using System;
using System.Collections.Generic;
using NeoServer.Loaders.OTB.DataStructures;
using NeoServer.Loaders.OTB.Enums;

namespace NeoServer.Loaders.OTB.Structure;

public class OtbNode
{
    private readonly List<OtbNode> children;
    private readonly List<byte> data;

    /// <summary>
    ///     The type of the node.
    /// </summary>
    public readonly NodeType Type;

    /// <summary>
    ///     Creates a new instance of a <see cref="OtbNode" />.
    /// </summary>
    public OtbNode(NodeType type)
    {
        children = new List<OtbNode>();
        data = new List<byte>();
        Type = type;
    }

    /// <summary>
    ///     The children of this node.
    /// </summary>
    public ReadOnlyArray<OtbNode> Children => ReadOnlyArray<OtbNode>.WrapCollection(children.ToArray());

    /// <summary>
    ///     The data of this node.
    /// </summary>
    public ReadOnlyMemory<byte> Data => data.ToArray();

    /// <summary>
    ///     Adds child node
    /// </summary>
    /// <param name="node"></param>
    public void AddChild(OtbNode node)
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