using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Events
{
    public class PlayerAddedOnMapEvent
    {
        public PlayerAddedOnMapEvent(IPlayer player)
        {
            Player = player;
        }

        public IPlayer Player { get; }

        public string EventId => throw new System.NotImplementedException();

        public uint RequestorId => throw new System.NotImplementedException();

        public string ErrorMessage => throw new System.NotImplementedException();
    }
}