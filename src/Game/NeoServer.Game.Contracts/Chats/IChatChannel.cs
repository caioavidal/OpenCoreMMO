using NeoServer.Game.Common;
using NeoServer.Game.Common.Talks;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Chats
{
    public delegate void AddMessage(IPlayer player, IChatChannel channel, SpeechType speechType, string message );
    public interface IChatChannel
    {
        ushort Id { get; }
        string Name { get; }
        IEnumerable<IChatUser> Users { get; }
        string Description { get; }
        bool Opened { get; }

        event AddMessage OnMessageAdded;

        bool AddUser(IPlayer player);
        SpeechType GetTextColor(byte vocation);
        bool HasUser(IPlayer player);
        bool PlayerCanJoin(IPlayer player);
        bool PlayerCanWrite(IPlayer player);
        bool PlayerIsMuted(IPlayer player, out string cancelMessage);
        bool RemoveUser(IPlayer player);
        bool WriteMessage(IPlayer player, string message, out string cancelMessage);
    }

    public interface IChatUser
    {
        bool Removed { get; }
        bool IsMuted { get; }
        IPlayer Player { get; init; }
    }
}
