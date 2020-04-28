using NeoServer.Game.Enums.Location;

namespace NeoServer.Networking.Packets.Outgoing
{
    public sealed class TextMessageOutgoingParser
    {
        public static string Parse(InvalidOperation error) =>
                error switch
                {
                    InvalidOperation.NotEnoughRoom => "There is not enough room.",
                    InvalidOperation.Impossible => "This is impossible",
                    _ => string.Empty
                };

    }
}
