using NeoServer.OTB.DataStructures;
using NeoServer.OTB.Enums;
using System;
using System.Collections.Generic;

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
		public ReadOnlyArray<OTBNode> Children => ReadOnlyArray<OTBNode>.WrapCollection(children.ToArray());

		/// <summary>
		/// The data of this node.
		/// </summary>
		public ReadOnlyMemory<byte> Data => data.ToArray();

		private List<OTBNode> children = new List<OTBNode>();
		private List<byte> data = new List<byte>();

		/// <summary>
		/// Creates a new instance of a <see cref="OTBNode"/>.
		/// </summary>
		public OTBNode(NodeType type)
		{
			Type = type;
		}

		public void AddChild(OTBNode node)
		{
			children.Add(node);
		}
		public void AddData(byte b){
			data.Add(b);
		}
	}
}


// namespace NeoServer.OTB.Structure
// {
//     public sealed class OTBNode
// 	{
// 		/// <summary>
// 		/// The type of the node.
// 		/// </summary>
// 		public readonly NodeType Type;

// 		/// <summary>
// 		/// The children of this node.
// 		/// </summary>
// 		public readonly ReadOnlyArray<OTBNode> Children;

// 		/// <summary>
// 		/// The data of this node.
// 		/// </summary>
// 		public readonly ReadOnlyMemory<byte> Data;

// 		/// <summary>
// 		/// Creates a new instance of a <see cref="OTBNode"/>.
// 		/// </summary>
// 		public OTBNode(NodeType type, ReadOnlyArray<OTBNode> children, ReadOnlyMemory<byte> data)
// 		{
// 			if (children == null)
// 				throw new ArgumentNullException(nameof(children));

// 			Type = type;
// 			Children = children;
// 			Data = data;
// 		}
// 	}
// }
