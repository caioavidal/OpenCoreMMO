
// using System.Collections.Generic;
// using NeoServer.Game.Contracts;
// using NeoServer.Game.Enums.Location;
// using NeoServer.Networking;
// using NeoServer.Networking.Packets.Outgoing;
// using NeoServer.Server.Contracts.Network;
// using NeoServer.Server.Model.Players;
// using NeoServer.Server.Model.Players.Contracts;
// using NeoServer.Server.Schedulers.Map;

// namespace NeoServer.Server.Handlers.Players
// {
//     public class PlayerMovementHandler
//     {
//         public static void Handler(Player player, IMap map, Direction direction, IConnection connection)
//         {
//             player.ResetIdleTime();

//             var stackPosition = player.GetStackPosition();

//             var location = player.Location;



//             player.Move(direction, map);

//             var toLocation = player.Location;


//             var outgoingPackets = new Queue<IOutgoingPacket>();

//             outgoingPackets.Enqueue(new CreatureMovedPacket(location, toLocation, stackPosition));
//             outgoingPackets.Enqueue(new MapPartialDescriptionPacket(player, toLocation, direction, map));

//             connection.Send(outgoingPackets);
//         }
//     }
// }