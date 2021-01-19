namespace NeoServer.Networking.Listeners
{
    internal interface IListener
    {
        void BeginListening();
        void EndListening();
    }
}