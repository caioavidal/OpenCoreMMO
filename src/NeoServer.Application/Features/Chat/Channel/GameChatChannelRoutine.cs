using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Game.Chat.Channels.Contracts;
using NeoServer.Game.Common.Contracts.DataStores;

namespace NeoServer.Application.Features.Chat.Channel;

public class GameChatChannelRoutine
{
    private const ushort EVENT_CHECK_ITEM_INTERVAL = 10000;
    private readonly IChatChannelStore _chatChannelStore;
    private readonly IGameServer _game;

    public GameChatChannelRoutine(IGameServer game, IChatChannelStore chatChannelStore)
    {
        _game = game;
        _chatChannelStore = chatChannelStore;
    }

    public void StartChecking()
    {
        _game.Scheduler.AddEvent(new SchedulerEvent(EVENT_CHECK_ITEM_INTERVAL, StartChecking));

        foreach (var channel in _chatChannelStore.All) CleanUp(channel);
    }


    private static void CleanUp(IChatChannel channel)
    {
        var removedUsers = channel.Users.Where(x => x.Removed && !x.IsMuted);
        foreach (var user in removedUsers) channel.RemoveUser(user.Player);
    }
}