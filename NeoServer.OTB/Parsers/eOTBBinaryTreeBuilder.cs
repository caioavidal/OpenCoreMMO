// using System;
// using NeoServer.OTB.DataStructures;
// using NeoServer.OTB.Enums;
// using NeoServer.OTB.Structure;

// namespace NeoServer.OTB.Parsers
// {
//     public class OTBBinaryTreeBuilder
//     {
//         private const short IdentifierLength = 4;

//         public static OTBNode Deserialize(ReadOnlyMemory<byte> otbmStream)
//         {
//             var serializedOTBMData = otbmStream.Slice(IdentifierLength);
//             var stream = new ReadOnlyMemoryStream(serializedOTBMData);

//             var treeBuilder = new OTBTreeBuilder(serializedOTBMData);
//             while (!stream.IsOver)
//             {
//                 var currentMark = (OTBMarkupByte)stream.ReadByte();
//                 if (currentMark < OTBMarkupByte.Escape)
//                 {
//                     // Since <see cref="OTBMarkupByte"/> can only have values Escape (0xFD), Start (0xFE) and
//                     // End (0xFF), if currentMark < Escape, then it's just prop data
//                     // and we can safely skip it.
//                     continue;
//                 }

//                 switch (currentMark)
//                 {
//                     case OTBMarkupByte.Start:
//                         var nodeType = (NodeType)stream.ReadByte();

//                         treeBuilder.AddNodeDataBegin(
//                             start: stream.Position,
//                             type: nodeType);
//                         break;

//                     case OTBMarkupByte.End:
//                         treeBuilder.AddNodeEnd(stream.Position - 1);
//                         break;

//                     case OTBMarkupByte.Escape:
//                         stream.Skip();
//                         break;

//                     default:
//                         throw new InvalidOperationException();
//                 }
//             }

//             return treeBuilder.BuildTree();
//         }

//     }
// }