using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace NeoServer.Networking
{

    public class Connection
    {
        public bool KeepConnectionAlive { get; }

        public event EventHandler<ConnectionEventArgs> OnProcessEvent;
        public event EventHandler<ConnectionEventArgs> OnCloseEvent;
        public event EventHandler<ConnectionEventArgs> OnPostProcessEvent;

        private Socket Socket;
        private Stream Stream;

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
            Stream.BeginRead(InMessage.Buffer, 0, 2, OnRead, null);
        }

        private void OnRead(IAsyncResult ar)
        {
            //if (!CompleteRead(ar))
            //{
            //    return;
            //}

            try
            {
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
    }
}
