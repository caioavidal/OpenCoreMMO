using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Items;
using NeoServer.Game.Items.Items;
using NeoServer.Networking.Packets;
using NeoServer.Networking.Packets.Outgoing;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NeoServer.Networking.Tests.Packets.Outgoing
{
    public class RemoveTileItemPacketTest
    {
        private Item CreateRegularItem(ushort id)
        {
            var type = new ItemType();
            type.SetClientId(id);
            type.SetName("item");

            return new Item(type, new Location(100, 100, 7));
        }
        [Fact]
        public void Constructor_Item_Null_Throws()
        {
            Action sut = () => new RemoveTileItemPacket(new Location(100, 100, 7), 1, null);
            Assert.Throws<NullReferenceException>(sut);
        }
        [Fact]
        public void WriteToMessage_Adds_Bytes_To_NetworkMessage()
        {
            var sut = new RemoveTileItemPacket(new Location(100, 100, 7), 1, CreateRegularItem(20));
            var message = new NetworkMessage();
            sut.WriteToMessage(message);

            var expected = new byte[] { 0x6A, (byte)100, (byte)0, (byte)100, (byte)0, (byte)7, (byte)1, (byte)20, (byte)0 };

            Assert.Equal(expected, message.GetMessageInBytes());
        }
    }
}
