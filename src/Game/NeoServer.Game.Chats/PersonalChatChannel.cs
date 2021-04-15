using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Chats
{
    public abstract class PersonalChatChannel : ChatChannel, IChatChannel
    {
        public PersonalChatChannel(ushort id, string name) : base(id, name)
        {
        }
        public abstract new string Name { get; }
        public override bool AddUser(IPlayer player)
        {
            if (users.Count == 1) return false;
            return base.AddUser(player);
        }
    }
}
