// using System;
// using System.Collections.Generic;
// using NeoServer.OTB.DataStructures;
// using NeoServer.OTB.Enums;

// namespace NeoServer.OTB.Structure
// {
//     public sealed class OTBTreeBuilder
// 	{
// 		private readonly ReadOnlyMemory<byte> _serializedTreeData;
// 		private readonly Stack<int> _nodeStarts = new Stack<int>();
// 		private readonly Stack<NodeType> _nodeTypes = new Stack<NodeType>();
// 		private readonly Stack<int> _childrenCounts = new Stack<int>();
// 		private readonly Stack<OTBNode> _builtNodes = new Stack<OTBNode>();

// 		public OTBTreeBuilder(ReadOnlyMemory<byte> serializedTreeData)
// 		{
// 			_serializedTreeData = serializedTreeData;
// 		}

// 		/// <remarks>Start will be included in the data.</remarks>
// 		public void AddNodeDataBegin(int start, NodeType type)
// 		{
// 			if (start < 0 || start > _serializedTreeData.Length)
// 				throw new ArgumentOutOfRangeException(nameof(start));

// 			// Sanity check
// 			if (_nodeStarts.TryPeek(out var lastStart))
// 			{
// 				if (start <= lastStart)
// 					throw new InvalidOperationException();
// 			}

// 			_nodeStarts.Push(start);
// 			_childrenCounts.Push(0);
// 			_nodeTypes.Push(type);
// 		}

// 		/// <remarks>End will not be included in the data.</remarks>
// 		public void AddNodeEnd(int end)
// 		{
// 			if (end < 0 || end > _serializedTreeData.Length)
// 				throw new ArgumentOutOfRangeException();

// 			// Sanity checks
// 			if (!_nodeStarts.TryPop(out var start))
// 				throw new InvalidOperationException();
// 			//if (end <= start)
// 			//	throw new InvalidOperationException();

// 			var nodeData = _serializedTreeData.Slice(
// 				start: start,
// 				length: end - start);

// 			// Fiding this node's children
// 			if (!_childrenCounts.TryPop(out var childCount))
// 				throw new InvalidOperationException();
				
// 			// Since we are using a queue, we need to store the children in reverse order
// 			var currentNodeChildren = new OTBNode[childCount];
// 			for (int i = childCount - 1; i >= 0; i--)
// 				currentNodeChildren[i] = _builtNodes.Pop();

// 			// Updating the child count of the parent of the node we are creating
// 			if (_childrenCounts.Count > 0)
// 			{
// 				var siblingCount = _childrenCounts.Pop();
// 				_childrenCounts.Push(siblingCount + 1);
// 			}

// 			// Creating node and storing it
// 			var node = new OTBNode(
// 				type: _nodeTypes.Pop(),
// 				children: ReadOnlyArray<OTBNode>.WrapCollection(currentNodeChildren),
// 				data: nodeData);
// 			_builtNodes.Push(node);
// 		}

// 		public OTBNode BuildTree()
// 		{
// 			if (_builtNodes.Count != 1)
// 				throw new InvalidOperationException();
// 			if (_nodeStarts.Count != 0)
// 				throw new InvalidOperationException();
// 			if (_childrenCounts.Count != 0)
// 				throw new InvalidOperationException();

// 			return _builtNodes.Pop();
// 		}
// 	}
// }
