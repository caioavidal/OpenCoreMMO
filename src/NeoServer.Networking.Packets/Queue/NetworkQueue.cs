// using NeoServer.Networking.Packets.Outgoing;
// using NeoServer.Server.Contracts.Network;
// using System;
// using System.Collections.Concurrent;
// using System.Collections.Generic;
// using System.Text;
// using System.Threading.Tasks;

// namespace NeoServer.Networking.Queue
// {
//     public class NetworkQueue
//     {
//         private BlockingCollection<OutgoingPacket> queue;

//         public void Enqueue(OutgoingPacket message)
//         {

//         }

//         public void Complete() => queue.CompleteAdding();

//         public void Start()
//         {
//             queue = new BlockingCollection<IOutputStreamMessage>();
//             Task.Run(() =>
//             {
//                 while (!queue.IsCompleted)
//                 {
//                     try
//                     {
//                         var message = queue.Take();
//                         message.Send();
//                     }
//                     catch (Exception ex)
//                     {

//                     }
//                 }
//             });
//         }
//     }
// }
