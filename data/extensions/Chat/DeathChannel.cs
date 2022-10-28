using NeoServer.Game.Chats;
using NeoServer.Game.Chats.Rules;
using NeoServer.Game.Common.Chats;

namespace NeoServer.Extensions.Chat;

public class DeathChannel : ChatChannel
{
    public DeathChannel(ushort id, string name) : base(id, name)
    {
    }

    public override string Name => "Deaths";

    public override bool Opened
    {
        get => false;
        init => base.Opened = value;
    }

    public override SpeechType ChatColor
    {
        get => SpeechType.ChannelWhiteText;
        init => base.ChatColor = SpeechType.ChannelWhiteText;
    }

    public override ChannelRule WriteRule
    {
        get => new()
        {
            AllowedVocations = new[] { byte.MaxValue }
        };
        init => base.WriteRule = value;
    }
}