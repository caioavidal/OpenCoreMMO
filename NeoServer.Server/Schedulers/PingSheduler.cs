// using System;
// using System.Collections.Concurrent;
// using System.Collections.Generic;
// using NeoServer.Networking.Packets.Outgoing;
// using NeoServer.Server.Schedulers.Map;

// namespace NeoServer.Server.Schedulers
// {
//     public class PingScheduler: Scheduler
//     {
//         private readonly Game _game;

//         private IDictionary<uint, DateTime> PlayersPing;
//         public PingScheduler(Game game)
//         {
//             _game = game;
//             PlayersPing = new Dictionary<uint,DateTime>();
//         }
//         public void Start()
//         {
//             while (true)
//             {
//                 SendPing();
//             }

//         }

//         private void SendPing()
//         {
//             foreach (var connection in _game.Connections)
//             {
//                 if (PlayersPing.TryGetValue(connection.Key, out DateTime lastPing))
//                 {
//                     if ((DateTime.Now - lastPing).Seconds <= 5)
//                     {
//                         continue;
//                     }
//                 }
//                 else
//                 {
//                     PlayersPing.TryAdd(connection.Key, DateTime.Now);

//                 }

//            //     connection.Value.Send(new PingPacket());
//             }
//         }

//     }
// }