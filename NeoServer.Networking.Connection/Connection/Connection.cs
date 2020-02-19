using NeoServer.Networking.Connections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace NeoServer.Networking
{

    public class Connection
    {
        public event EventHandler<ConnectionEventArgs> OnProcessEvent;
        public event EventHandler<ConnectionEventArgs> OnCloseEvent;
        public event EventHandler<ConnectionEventArgs> OnPostProcessEvent;

        private Socket Socket;
        private Stream Stream;

        private byte[] Buffer = new byte[1024];

        public NetworkMessage InMessage { get; private set; }

        public void OnAccept(IAsyncResult ar)
        {
            if (ar == null)
            {
                throw new ArgumentNullException(nameof(ar));
            }

            Socket = ((TcpListener)ar.AsyncState).EndAcceptSocket(ar);
            Stream = new NetworkStream(Socket);

            BeginStreamRead();

        }
        public void BeginStreamRead()
        {
            Stream.BeginRead(Buffer, 0, 1024, OnRead, this);
        }

        private void OnRead(IAsyncResult ar)
        {
            //if (!CompleteRead(ar))
            //{
            //    return;
            //}

            try
            {
                InMessage = new NetworkMessage(Buffer);
                var eventArgs = new ConnectionEventArgs(this);
                OnProcessEvent(this, eventArgs);
                OnPostProcessEvent(this, eventArgs);
            }
            catch (Exception e)
            {
                // Invalid data from the client
                // TODO: I must not swallow exceptions.
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);

                // TODO: is closing the connection really necesary?
                // Close();
            }
        }

        public void Close()
        {
            Stream.Close();
            Socket.Close();

            // Tells the subscribers of this event that this connection has been closed.
            OnCloseEvent(this, new ConnectionEventArgs(this));
        }
    }
}
