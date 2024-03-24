using Mediator;
using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.Contracts.Scripts;
using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Networking.Packets.Network;
using NeoServer.Networking.Packets.Outgoing;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NeoServer.Application.Features.Chat.PlayerSay;

public record PlayerSayCommand(
    IPlayer Player,
    IConnection Connection,
    SpeechType TalkType,
    string Receiver,
    string Message,
    ushort ChannelId) : ICommand;

public class PlayerSayCommandHandler : ICommandHandler<PlayerSayCommand>
{
    private readonly IChatChannelStore _chatChannelStore;
    private readonly IGameCreatureManager _creatureManager;
    private readonly IMap _map;
    private readonly ILuaManager _luaManager;

    public PlayerSayCommandHandler(IMap map, IChatChannelStore chatChannelStore, IGameCreatureManager creatureManager, ILuaManager luaManager)
    {
        _map = map;
        _chatChannelStore = chatChannelStore;
        _creatureManager = creatureManager;
        _luaManager = luaManager;
    }

    public ValueTask<Unit> Handle(PlayerSayCommand command, CancellationToken cancellationToken)
    {
        command.Deconstruct(out var player, out var connection,
            out var talkType, out var receiver, out var message, out var channelId);

        if (string.IsNullOrWhiteSpace(message) || (message?.Length ?? 0) > 255) return Unit.ValueTask;

        if ((receiver?.Length ?? 0) > 30) return Unit.ValueTask;

        message = message.Trim();

        var messageData = message.Split(" ");

        var talkAction = _luaManager.GetTalkAction(messageData[0]);
        var parameter = "";

        if (messageData.Count() > 1)
            parameter = messageData[1];

        if (talkAction != null)
        {
            talkAction.ExecuteSay(player, messageData[0], parameter, talkType);
            return Unit.ValueTask;
        }

        if (player.CastSpell(message)) return Unit.ValueTask;

        switch (talkType)
        {
            case SpeechType.None:
                break;
            case SpeechType.Say:
                player.Say(message, talkType);
                break;
            case SpeechType.Whisper:
                break;
            case SpeechType.Yell:
                break;
            case SpeechType.PrivatePlayerToNpc:
                SendMessageToNpc(player, talkType, message);
                break;
            case SpeechType.PrivateNpcToPlayer:
                break;

            case SpeechType.ChannelOrangeText:
            case SpeechType.ChannelRed1Text:
            case SpeechType.ChannelYellowText:
                SendMessageToChannel(player, channelId, message);
                break;

            case SpeechType.ChannelRed2Text:
                break;
            case SpeechType.ChannelWhiteText:
                break;
            case SpeechType.RvrChannel:
                break;
            case SpeechType.RvrAnswer:
                break;
            case SpeechType.RvrContinue:
                break;
            case SpeechType.Broadcast:
                break;
            case SpeechType.Private:
            case SpeechType.PrivateRed:
                SendMessageToPlayer(player, connection, receiver, talkType, message);
                break;
            case SpeechType.MonsterSay:
                break;
            case SpeechType.MonsterYell:
                break;
        }

        return Unit.ValueTask;
    }

    private void SendMessageToNpc(IPlayer player, SpeechType talkType, string message)
    {
        foreach (var creature in _map.GetCreaturesAtPositionZone(player.Location))
            if (creature is INpc npc)
            {
                npc.Hear(player, talkType, message);
                return;
            }
    }

    private void SendMessageToChannel(IPlayer player, ushort channelId, string message)
    {
        var channel = _chatChannelStore.Get(channelId) ??
                      player.Channels.PrivateChannels.FirstOrDefault(x => x.Id == channelId);

        if (channel is null) return;

        player.Channels.SendMessage(channel, message);
    }

    private void SendMessageToPlayer(IPlayer player, IConnection connection, string receiverName, SpeechType talkType,
        string message)
    {
        if (string.IsNullOrWhiteSpace(receiverName) ||
            !_creatureManager.TryGetPlayer(receiverName, out var receiver))
        {
            connection.OutgoingPackets.Enqueue(new TextMessagePacket("A player with this name is not online.",
                TextMessageOutgoingType.Small));
            connection.Send();
            return;
        }

        player.SendMessageTo(receiver, talkType, message);
    }
}