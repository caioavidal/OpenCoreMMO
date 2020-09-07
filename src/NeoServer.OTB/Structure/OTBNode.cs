using NeoServer.OTB.DataStructures;
using NeoServer.OTB.Enums;
using System;
using System.Collections.Generic;

namespace NeoServer.OTB.Structure
{
    public class OTBNode
    {
        /// <summary>
        /// The type of the node.
        /// </summary>
        public readonly NodeType Type;

        /// <summary>
        /// The children of this node.
        /// </summary>
        public ReadOnlyArray<OTBNode> Children => ReadOnlyArray<OTBNode>.WrapCollection(children.ToArray());

        /// <summary>
        /// The data of this node.
        /// </summary>
        public ReadOnlyMemory<byte> Data => data.ToArray();

        private List<OTBNode> children;
        private List<byte> data;

        /// <summary>
        /// Creates a new instance of a <see cref="OTBNode"/>.
        /// </summary>
        public OTBNode(NodeType type)
        {
            children = new List<OTBNode>();
            data = new List<byte>();
            Type = type;
        }

        /// <summary>
        /// Adds child node
        /// </summary>
        /// <param name="node"></param>

        public void AddChild(OTBNode node)
        {
            children.Add(node);
        }

        /// <summary>
        /// Adds byte to node's data
        /// </summary>
        /// <param name="b">The byte data to add</param>
        public void AddData(byte b)
        {
            data.Add(b);
        }

    }
}