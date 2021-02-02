using NeoServer.Game.Chats;
using NeoServer.Game.Common.Talks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Scripts.Chat
{
    public class LootChannel : PersonalChatChannel
    {
        public LootChannel(ushort id, string name) : base(id, name)
        {
        }

        public override string Name => "Loot";
        public override bool Opened { get => false; init => base.Opened = value; }
        public override SpeechType ChatColor { get => SpeechType.ChannelWhiteText; init => base.ChatColor = SpeechType.ChannelWhiteText; }
        public override ChannelRule WriteRule
        {
            get => new ChannelRule
            {
                AllowedVocations = new byte[] { byte.MaxValue }
            }; init => base.WriteRule = value;
        }
    }
}
