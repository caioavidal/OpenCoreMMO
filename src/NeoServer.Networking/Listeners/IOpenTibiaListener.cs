namespace NeoServer.Networking.Listeners
{
    internal interface IOpenTibiaListener
    {
        void BeginListening();
        void EndListening();
    }
}