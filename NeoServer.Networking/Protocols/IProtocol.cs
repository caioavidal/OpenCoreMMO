//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace NeoServer.Networking.Protocols
//{
//    public interface IProtocol
//    {
//        /// <summary>
//        /// Gets a value indicating whether the protocol should keep the connection open after recieving a packet.
//        /// </summary>
//        bool KeepConnectionOpen { get; }

//        /// <summary>
//        /// Handles a new connection.
//        /// </summary>
//        /// <param name="connection">The connection.</param>
//        /// <param name="ar">The result of connecting.</param>
//        void OnAcceptNewConnection(Connection connection, IAsyncResult ar);

//        /// <summary>
//        /// Processes an incomming message from the connection.
//        /// </summary>
//        /// <param name="connection">The connection where the message is being read from.</param>
//        /// <param name="inboundMessage">The message to process.</param>
//        void ProcessMessage(Connection connection, NetworkMessage inboundMessage);

//        /// <summary>
//        /// Runs after processing a message from the connection.
//        /// </summary>
//        /// <param name="connection">The connection where the message is from.</param>
//        void PostProcessMessage(Connection connection);
//    }
//}
