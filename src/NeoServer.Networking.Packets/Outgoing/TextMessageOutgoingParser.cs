using NeoServer.Game.Enums.Location;

namespace NeoServer.Networking.Packets.Outgoing
{
    public sealed class TextMessageOutgoingParser
    {
        public static string Parse(PathError pathError) =>
                pathError switch
                {
                    PathError.NotEnoughRoom => "There is not enough room.",
                    _ => string.Empty
                };

    }
}
