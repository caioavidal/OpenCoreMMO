using NeoServer.Server.World.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.World.OTB
{
	public static class OTBDeserializer
	{

		public static readonly int IdentifierLength = 4;

		/// <summary>
		/// Parses data serialized as .otb and returns the deserialized .otb tree structure.
		/// </summary>
		/// <remarks>
		/// Beware that some .otb serializers add a "format identifier" before the data.
		/// Maps (worlds?), for instance, contain 4 "format identifier bytes" that should be skiped.
		/// </remarks>
		public static OTBNode DeserializeOTBData(ReadOnlyMemory<byte> serializedOTBData)
		{
			// Lets throw away the identifier bytes and keep the rest
			serializedOTBData = serializedOTBData.Slice(IdentifierLength);
			var stream = new ReadOnlyMemoryStream(serializedOTBData);

			var treeBuilder = new OTBTreeBuilder(serializedOTBData);
			while (!stream.IsOver)
			{
				var currentMark = (OTBMarkupByte)stream.ReadByte();
				if (currentMark < OTBMarkupByte.Escape)
				{
					// Since <see cref="OTBMarkupByte"/> can only have values Escape (0xFD), Start (0xFE) and
					// End (0xFF), if currentMark < Escape, then it's just prop data 
					// and we can safely skip it.
					continue;
				}

				switch (currentMark)
				{
					case OTBMarkupByte.Start:
						var nodeType = (OTBNodeType)stream.ReadByte();
						
						treeBuilder.AddNodeDataBegin(
							start: stream.Position,
							type: nodeType);
						break;

					case OTBMarkupByte.End:
						treeBuilder.AddNodeEnd(stream.Position - 1);
						break;

					case OTBMarkupByte.Escape:
						stream.Skip();
						break;

					default:
						throw new InvalidOperationException();
				}
			}

			return treeBuilder.BuildTree();
		}
	}
}
