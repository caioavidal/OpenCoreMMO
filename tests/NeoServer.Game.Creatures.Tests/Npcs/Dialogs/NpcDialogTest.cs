using Moq;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Npcs;
using NeoServer.Game.Creatures.Npcs.Dialogs;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Npcs.Dialogs
{
    public class NpcDialogTest
    {
        [Fact]
        public void GetNextAnswer_Must_Get_Right_Answer()
        {
            var npc = new Mock<INpc>();

            npc.Setup(x => x.Name).Returns("Eryn");
            npc.Setup(x => x.Metadata.Dialogs).Returns(new IDialog[] { new Dialog()
            {
                OnWords = new string[] { "hi" },
                Then =new IDialog[] { new Dialog()
                {
                     OnWords = new string[] { "carlin" },
                     Answers = new string[] {"Do you want to sail to carlin for 1000 gold coins ?"},
                       Then =new IDialog[] { new Dialog() { OnWords =new string[] { "yes" } },
                       new Dialog() { OnWords = new string[] { "no" }, Back = 1,  Answers = new string[] { "Then stay here!" } },
                       }
                } }
            } });

            var sut = new NpcDialog(npc.Object);

            sut.GetNextAnswer(1, "hi");
            sut.GetNextAnswer(1, "carlin");
            sut.GetNextAnswer(1, "no");
            var result = sut.GetNextAnswer(1, "carlin");

            Assert.Equal("Do you want to sail to carlin for 1000 gold coins ?", result?.Answers[0]);
        }

        [Fact]
        public void StoreWords_Adding_Same_Storage_Key_Should_Update_Value()
        {

            var npc = new Mock<INpc>();
            npc.Setup(x => x.Name).Returns("Eryn");

            var player = new Mock<IPlayer>();
            player.Setup(x => x.CreatureId).Returns(1);

            var sut = new NpcDialog(npc.Object);

            sut.StoreWords(player.Object, "city", "carlin");
            sut.StoreWords(player.Object, "city", "venore");

            var values = sut.GetDialogStoredValues(player.Object);

            Assert.Equal("venore", values["city"]);
        }
    }
}
