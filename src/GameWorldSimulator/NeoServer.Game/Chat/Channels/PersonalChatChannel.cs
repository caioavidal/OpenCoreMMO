using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Chat.Channels;

public abstract class PersonalChatChannel : ChatChannel
{
    protected PersonalChatChannel(ushort id, string name) : base(id, name)
    {
    }

    public new abstract string Name { get; }

    public override bool AddUser(IPlayer player)
    {
        return users.Count != 1 && base.AddUser(player);
    }
}