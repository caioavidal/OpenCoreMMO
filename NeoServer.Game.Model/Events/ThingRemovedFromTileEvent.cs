
using NeoServer.Game.Contracts;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Schedulers.Contracts;

namespace NeoServer.Game.Events
{
    public class ThingRemovedFromTileEvent : IEvent
    {
        public ThingRemovedFromTileEvent(ITile tile, byte stackPosition)
        {
            Tile = tile;
            StackPosition = stackPosition;
        }

        public ITile Tile { get; }
        public byte StackPosition { get; }

        public string EventId => throw new System.NotImplementedException();

        public uint RequestorId => throw new System.NotImplementedException();

        public string ErrorMessage => throw new System.NotImplementedException();
    }
}