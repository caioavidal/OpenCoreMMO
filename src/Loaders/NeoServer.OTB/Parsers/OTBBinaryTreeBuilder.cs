using NeoServer.OTB.DataStructures;
using NeoServer.OTB.Enums;
using NeoServer.OTB.Structure;
using System;
using System.Linq;

namespace NeoServer.OTB.Parsers
{
    public sealed class OTBBinaryTreeBuilder
    {

        /// <summary>
        /// Creates a OTBNode Binary Tree from otbm stream
        /// </summary>
        /// <param name="otbmStream"></param>
        /// <returns></returns>
        public static OTBNode Deserialize(ReadOnlyMemory<byte> otbmStream)
        {

            var serializedOTBMData = otbmStream.Slice(4);
            var memoryStream = new ReadOnlyMemoryStream(serializedOTBMData);

            return BuildTree(new OTBNode(NodeType.NotSetYet), memoryStream).Children.First();
        }

        private static OTBNode BuildTree(OTBNode node, ReadOnlyMemoryStream stream) //recursive method to create a binary tree
        {
            var currentByte = stream.ReadByte();

            switch ((OTBMarkupByte)currentByte)
            {
                case OTBMarkupByte.Start:
                    while (currentByte == (byte)OTBMarkupByte.Start)
                    {
                        var childNode = new OTBNode((NodeType)stream.ReadByte());
                        node.AddChild(BuildTree(childNode, stream));
                        if (stream.IsOver)
                        {
                            break;
                        }
                        currentByte = stream.ReadByte();
                    }
                    return node;

                case OTBMarkupByte.Escape:
                    node.AddData(currentByte);
                    node.AddData(stream.ReadByte());
                    return BuildTree(node, stream);
                case OTBMarkupByte.End:
                    return node;
                default:
                    node.AddData(currentByte);
                    return BuildTree(node, stream);
            }
        }
    }
}