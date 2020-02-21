using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Handlers;
using NeoServer.Server.Handlers.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Networking.Packets
{
    public static class HandlerFactory
    {
        //private static Dictionary<GameIncomingPacketType, Handler> Handlers = new Dictionary<GameIncomingPacketType, Handler>()
        //{
        //   {GameIncomingPacketType.AddVip, new Handler(AccountLoginEventHandler.Instance, typeof(AccountLoginPacket)) }
        //};
        //public static Handler GetHandler(GameIncomingPacketType type)
        //{
        //    return Handlers[type];
        //}
    }
}
