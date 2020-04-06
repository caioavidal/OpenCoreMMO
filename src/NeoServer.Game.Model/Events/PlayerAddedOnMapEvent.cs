
using NeoServer.Game.Contracts;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Schedulers.Contracts;

namespace NeoServer.Game.Events
{
    public class PlayerAddedOnMapEvent : IEvent
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