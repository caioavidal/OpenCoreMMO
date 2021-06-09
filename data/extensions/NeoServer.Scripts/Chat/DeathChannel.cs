using NeoServer.Game.Chats;
using NeoServer.Game.Common.Talks;

namespace NeoServer.Scripts.Chat
{
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
                AllowedVocations = new[] {byte.MaxValue}
            };
            init => base.WriteRule = value;
        }
    }
}