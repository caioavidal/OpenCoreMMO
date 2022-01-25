using Moq;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Creatures.Npcs;
using NeoServer.Game.Creatures.Npcs.Dialogs;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Npcs.Dialogs;

public class NpcDialogTest
{
    [Fact]
    public void GetNextAnswer_Must_Get_Right_Answer()
    {
        var npc = new Mock<INpc>();

        npc.Setup(x => x.Name).Returns("Eryn");
        npc.Setup(x => x.Metadata.Dialogs).Returns(new IDialog[]
        {
            new Dialog
            {
                OnWords = new[] { "hi" },
                Then = new IDialog[]
                {
                    new Dialog
                    {
                        OnWords = new[] { "carlin" },
                        Answers = new[] { "Do you want to sail to carlin for 1000 gold coins ?" },
                        Then = new IDialog[]
                        {
                            new Dialog { OnWords = new[] { "yes" } },
                            new Dialog { OnWords = new[] { "no" }, Back = 1, Answers = new[] { "Then stay here!" } }
                        }
                    }
                }
            }
        });

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

    [Theory]
    [InlineData(1, "fourth")]
    [InlineData(2, "third")]
    [InlineData(3, "second")]
    [InlineData(4, "first")]
    public void Back_Should_Remove_Last_Positions_On_Dialog_History(byte count, string answer)
    {
        var npc = new Mock<INpc>();
        npc.Setup(x => x.Name).Returns("Eryn");
        npc.Setup(x => x.Metadata.Dialogs).Returns(new IDialog[]
        {
            new Dialog
            {
                OnWords = new[] { "first" },
                Answers = new[] { "first" },
                Then = new IDialog[]
                {
                    new Dialog
                    {
                        OnWords = new[] { "second" },
                        Answers = new[] { "second" },
                        Then = new IDialog[]
                        {
                            new Dialog
                            {
                                OnWords = new[] { "third" },
                                Answers = new[] { "third" },
                                Then = new IDialog[]
                                {
                                    new Dialog
                                    {
                                        OnWords = new[] { "fourth" },
                                        Answers = new[] { "fourth" },
                                        Then = new IDialog[]
                                        {
                                            new Dialog
                                            {
                                                OnWords = new[] { "fifth" },
                                                Answers = new[] { "fifth" }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        });

        var player = new Mock<IPlayer>();
        player.Setup(x => x.CreatureId).Returns(1);

        var sut = new NpcDialog(npc.Object);

        sut.GetNextAnswer(1, "first");
        sut.GetNextAnswer(1, "second");
        sut.GetNextAnswer(1, "third");
        sut.GetNextAnswer(1, "fourth");
        sut.GetNextAnswer(1, "fifth");

        sut.Back(1, count);

        var result = sut.GetNextAnswer(1, answer);

        Assert.Equal(answer, result.Answers[0]);
    }
}