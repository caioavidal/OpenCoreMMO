using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures.Guilds;

namespace NeoServer.Game.Chat.Channels;

public class GuildChatChannel : ChatChannel
{
    public GuildChatChannel(ushort id, string name, IGuild guild) : base(id, name)
    {
        Guild = guild;
    }

    private IGuild Guild { get; }

    public override bool Opened
    {
        get => true;
        init => base.Opened = value;
    }

    public override bool AddUser(IPlayer player)
    {
        if (player.Guild is null) return false;
        if (Guild is null) return false;

        return Guild.HasMember(player) && base.AddUser(player);
    }

    public override SpeechType GetTextColor(IPlayer player)
    {
        if (Guild.GetMemberLevel(player) is not { } guildMember) return SpeechType.ChannelYellowText;

        return guildMember.Level switch
        {
            GuildRank.Leader => SpeechType.ChannelOrangeText,
            _ => SpeechType.ChannelYellowText
        };
    }
}