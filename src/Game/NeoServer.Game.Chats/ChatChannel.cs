using NeoServer.Game.Contracts.Chats;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.Chats
{
    public class ChatChannel:IChatChannel
    {
        private IDictionary<uint, ChatUser> users = new Dictionary<uint, ChatUser>();

        public ChatChannel(ushort id, string name, uint owner)
        {
            Id = id;
            Name = name;
            Owner = owner;
        }

        public ushort Id { get; }
        public string Name { get; }
        public uint Owner { get; }
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
    }

    public struct ChatUser
    {
        public IPlayer Player { get; init; }
        public long LastMessage { get; set; }
        public ushort MutedForSeconds { get; set; }
    }
}
