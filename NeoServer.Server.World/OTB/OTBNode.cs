using NeoServer.Server.World.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.World.OTB
{
	public sealed class OTBNode
	{
		/// <summary>
		/// The type of the node.
		/// </summary>
		public readonly OTBNodeType Type;

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
		public OTBNode(OTBNodeType type, ReadOnlyArray<OTBNode> children, ReadOnlyMemory<byte> data)
		{
			if (children == null)
				throw new ArgumentNullException(nameof(children));

			Type = type;
			Children = children;
			Data = data;
		}
	}
}
