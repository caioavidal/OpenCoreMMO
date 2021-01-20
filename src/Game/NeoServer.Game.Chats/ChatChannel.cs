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

        private IDictionary<uint, ChatUser> users = new Dictionary<uint, ChatUser>();

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
            return users.TryAdd(player.Id, new ChatUser { Player = player });
        }
        public IEnumerable<IChatUser> Users => users.Values; 

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
        public ChatUser[] GetAllUsers() => users.Values.ToArray();
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
    public struct MuteRule
    {
        public static MuteRule Default => new MuteRule { MessagesCount = 5, TimeMultiplier = 4, TimeToBlock = 10, WaitTime = 5 };
        public bool None => MessagesCount == default && TimeToBlock == default && WaitTime == default && TimeMultiplier == default && CancelMessage == default;
        public ushort MessagesCount { get; set; }
        public ushort TimeToBlock { get; set; }
        public ushort WaitTime { get; set; }
        public double TimeMultiplier { get; set; }
        public string CancelMessage { get; set; }
    }
    public class ChatUser: IChatUser
    {
        public IPlayer Player { get; init; }
        public long LastMessage { get; private set; }
        public ushort MutedForSeconds { get; set; }
        private ushort lastMutedForSeconds;
        public bool Removed { get; private set; }
        private long firstMessageBeforeMuted;
        public ushort MessagesCount { get; private set; }
        public int RemainingMutedSeconds => MutedForSeconds == 0 ? 0 : TimeSpan.FromTicks((LastMessage + (TimeSpan.TicksPerSecond * MutedForSeconds)) - DateTime.Now.Ticks).Seconds;

        public bool IsMuted => RemainingMutedSeconds > 0;
        public void UpdateLastMessage(MuteRule rule)
        {
            ushort secondsSinceFirstMessage = (ushort)TimeSpan.FromTicks(DateTime.Now.Ticks - firstMessageBeforeMuted).Seconds;

            if (secondsSinceFirstMessage > 0 && secondsSinceFirstMessage > rule.TimeToBlock) MessagesCount = 0;

            if (!IsMuted)
            {
                MessagesCount += 1;
                LastMessage = DateTime.Now.Ticks;
                if (MessagesCount == 1)
                {
                    firstMessageBeforeMuted = LastMessage;
                    lastMutedForSeconds = MutedForSeconds;
                    MutedForSeconds = 0;
                }
            }
            else
            {
                MessagesCount = 0;
            }

            if (MessagesCount >= rule.MessagesCount && secondsSinceFirstMessage <= rule.TimeToBlock)
            {
                var waitTime = lastMutedForSeconds == 0 ? (rule.WaitTime == default ? 1 : rule.WaitTime) : lastMutedForSeconds * (rule.TimeMultiplier == 0 ? 1 : rule.TimeMultiplier);
                MutedForSeconds = (ushort)waitTime;
            }
        }

        public void MarkAsRemoved() => Removed = true;
        public void MarkAsAdded() => Removed = false;

    }
    public struct ChannelRule
    {
        public bool None => AllowedVocations?.Length == 0 && (MinMaxAllowedLevel.Item1 == 0 && MinMaxAllowedLevel.Item2 == 0);
        public byte[] AllowedVocations { get; set; }
        public (ushort, ushort) MinMaxAllowedLevel { get; set; }
    }
}
