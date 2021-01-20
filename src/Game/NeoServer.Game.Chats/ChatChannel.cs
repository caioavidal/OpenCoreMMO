using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Chats;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Chats
{
    public class ChatChannel : IChatChannel
    {
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
        public TextColor ChatColor { get; init; }
        public Dictionary<byte, TextColor> ChatColorByVocation { private get; init; }

        public TextColor GetTextColor(byte vocation)
        {
            if (ChatColorByVocation.TryGetValue(vocation, out var color)) return color;

            return ChatColor;
        }
        public void Join(IPlayer player)
        {
            AddUser(player);
        }
        public void Exit(IPlayer player)
        {
            RemoveUser(player);
        }
        protected bool AddUser(IPlayer player) => users.TryAdd(player.Id, new ChatUser { Player = player });
        protected bool RemoveUser(IPlayer player) => users.Remove(player.Id);
        public ChatUser[] GetAllUsers() => users.Values.ToArray();
        public bool PlayerCanJoin(IPlayer player) => Validate(JoinRule, player);
        public bool PlayerCanWrite(IPlayer player) => Validate(WriteRule, player);
        public bool Validate(ChannelRule rule, IPlayer player)
        {
            if (rule.None) return true;
            if (rule.AllowedVocations?.Length > 0 && !JoinRule.AllowedVocations.Contains(player.VocationType)) return false;

            if (rule.MinMaxAllowedLevel.Item1 > 0 && player.Level <= JoinRule.MinMaxAllowedLevel.Item1) return false;
            if (rule.MinMaxAllowedLevel.Item2 > 0 && player.Level > JoinRule.MinMaxAllowedLevel.Item2) return false;
            return true;
        }
    }

    public struct ChatUser
    {
        public IPlayer Player { get; init; }
        public long LastMessage { get; set; }
        public ushort MutedForSeconds { get; set; }
    }
    public struct ChannelRule
    {
        public bool None => AllowedVocations?.Length == 0 && (MinMaxAllowedLevel.Item1 == 0 && MinMaxAllowedLevel.Item2 == 0);
        public byte[] AllowedVocations { get; set; }
        public (ushort, ushort) MinMaxAllowedLevel { get; set; }
    }
}
