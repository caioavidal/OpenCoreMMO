using NeoServer.OTBM.Enums;
using NeoServer.OTBM.Helpers;
using System;

namespace NeoServer.OTBM.Structure
{
    public sealed class OTBMNode
	{
		/// <summary>
		/// The type of the node.
		/// </summary>
		public readonly NodeType Type;

		/// <summary>
		/// The children of this node.
		/// </summary>
		public readonly ReadOnlyArray<OTBMNode> Children;

		/// <summary>
		/// The data of this node.
		/// </summary>
		public readonly ReadOnlyMemory<byte> Data;

		/// <summary>
		/// Creates a new instance of a <see cref="OTBNode"/>.
		/// </summary>
		public OTBMNode(NodeType type, ReadOnlyArray<OTBMNode> children, ReadOnlyMemory<byte> data)
		{
			if (children == null)
				throw new ArgumentNullException(nameof(children));

			Type = type;
			Children = children;
			Data = data;
		}
	}
}
