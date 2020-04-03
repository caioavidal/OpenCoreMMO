using NeoServer.OTB.DataStructures;
using NeoServer.OTB.Enums;
using System;

namespace NeoServer.OTB.Structure
{
    public sealed class OTBNode
	{
		/// <summary>
		/// The type of the node.
		/// </summary>
		public readonly NodeType Type;

		/// <summary>
		/// The children of this node.
		/// </summary>
		public readonly ReadOnlyArray<OTBNode> Children;

		/// <summary>
		/// The data of this node.
		/// </summary>
		public readonly ReadOnlyMemory<byte> Data;

		/// <summary>
		/// Creates a new instance of a <see cref="OTBNode"/>.
		/// </summary>
		public OTBNode(NodeType type, ReadOnlyArray<OTBNode> children, ReadOnlyMemory<byte> data)
		{
			if (children == null)
				throw new ArgumentNullException(nameof(children));

			Type = type;
			Children = children;
			Data = data;
		}
	}
}
