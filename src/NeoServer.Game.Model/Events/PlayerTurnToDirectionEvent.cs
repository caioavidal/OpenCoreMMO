using NeoServer.Game.Enums.Location;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Events
{
    public class PlayerTurnToDirectionEvent
    {
        public PlayerTurnToDirectionEvent(IPlayer player, Direction direction)
        {
            Player = player;
            Direction = direction;
        }

        public IPlayer Player { get; }
        public Direction Direction { get; }

        public string EventId => throw new System.NotImplementedException();

        public uint RequestorId => throw new System.NotImplementedException();

        public string ErrorMessage => throw new System.NotImplementedException();
    }
}