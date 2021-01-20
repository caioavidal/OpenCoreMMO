using NeoServer.Game.Common;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts.Chats
{
    public interface IChatChannel
    {
        ushort Id { get; }
        string Name { get; }

        bool AddUser(IPlayer player);
        TextColor GetTextColor(byte vocation);
        bool PlayerCanJoin(IPlayer player);
        bool PlayerCanWrite(IPlayer player);
    }
}
