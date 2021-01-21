using NeoServer.Game.Common;
using NeoServer.Game.Common.Talks;
using NeoServer.Game.Contracts.Chats;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Chats
{
    public class ChatChannel : IChatChannel
    {
        public event AddMessage OnMessageAdded;

        private IDictionary<uint, UserChat> users = new Dictionary<uint, UserChat>();

        public ChatChannel(ushort id, string name)
        {
            Id = id;
            Name = name;
        }

        public ushort Id { get; }
        public string Name { get; }
        public ChannelRule JoinRule { get; init; }
        public ChannelRule WriteRule { get; init; }
        public MuteRule MuteRule { get; init; }
        public SpeechType ChatColor { get; init; }
        public Dictionary<byte, SpeechType> ChatColorByVocation { private get; init; }

        public SpeechType GetTextColor(byte vocation)
        {
            if (ChatColorByVocation is not null && ChatColorByVocation.TryGetValue(vocation, out var color)) return color;

            return ChatColor;
        }
        public bool HasUser(IPlayer player) => users.TryGetValue(player.Id, out var user) && user.Removed == false;
        public bool AddUser(IPlayer player)
        {
            if (!PlayerCanJoin(player)) return false;

            if (users.TryGetValue(player.Id, out var user))
            {
                if (user.Removed == true)
                {
                    user.MarkAsAdded();
                    return true;
                }
                return false;
            }
            return users.TryAdd(player.Id, new UserChat { Player = player });
        }
        public IEnumerable<IUserChat> Users => users.Values; 

        public string Description { get; init; }
        public bool Opened { get; init; }

        public bool RemoveUser(IPlayer player)
        {
            if (users.TryGetValue(player.Id, out var user))
            {
                if (!user.IsMuted) users.Remove(player.Id);
                else user.MarkAsRemoved();
            }
            return true;
        }
        public UserChat[] GetAllUsers() => users.Values.ToArray();
        public bool PlayerCanJoin(IPlayer player) => Validate(JoinRule, player);
        public bool PlayerCanWrite(IPlayer player) => users.ContainsKey(player.Id) && Validate(WriteRule, player);
        public bool PlayerIsMuted(IPlayer player, out string cancelMessage)
        {
            cancelMessage = default;
            if (users.TryGetValue(player.Id, out var user) && user.IsMuted)
            {
                cancelMessage = string.IsNullOrWhiteSpace(MuteRule.CancelMessage) ? $"You are muted for {user.RemainingMutedSeconds} seconds" : MuteRule.CancelMessage;
                return true;
            }
            return false;
        }

        public bool WriteMessage(IPlayer player, string message, out string cancelMessage)
        {
            cancelMessage = default;
            if (!PlayerCanWrite(player))
            {
                cancelMessage = "You cannot send message to this channel";
                return false;
            }

            if (PlayerIsMuted(player, out cancelMessage)) return false;

            if (users.TryGetValue(player.Id, out var user))
            {
                user.UpdateLastMessage(MuteRule);
            }

            OnMessageAdded?.Invoke(player, this, GetTextColor(player.VocationType), message);
            return true;
        }

        public bool Validate(ChannelRule rule, IPlayer player)
        {
            if (rule.None) return true;
            if (rule.AllowedVocations?.Length > 0 && !rule.AllowedVocations.Contains(player.VocationType)) return false;

            if (rule.MinMaxAllowedLevel.Item1 > 0 && player.Level <= rule.MinMaxAllowedLevel.Item1) return false;
            if (rule.MinMaxAllowedLevel.Item2 > 0 && player.Level > rule.MinMaxAllowedLevel.Item2) return false;
            return true;
        }
    }
}
