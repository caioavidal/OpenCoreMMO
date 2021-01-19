namespace NeoServer.Networking.Packets.Outgoing
{
    public enum TextMessageOutgoingType : byte
    {
        MESSAGE_STATUS_CONSOLE_RED = 18, /*Red message in the console*/
        MESSAGE_EVENT_ORANGE = 19, /*Orange message in the console*/
        MESSAGE_STATUS_CONSOLE_ORANGE = 20,  /*Orange message in the console*/
        MESSAGE_STATUS_WARNING = 21, /*Red message in game window and in the console*/
        MESSAGE_EVENT_ADVANCE = 22, /*White message in game window and in the console*/
        MESSAGE_EVENT_DEFAULT = 23, /*White message at the bottom of the game window and in the console*/
        MESSAGE_STATUS_DEFAULT = 24, /*White message at the bottom of the game window and in the console*/
        MessageInfoDescription = 25, /*Green message in game window and in the console*/
        Small = 26, /*White message at the bottom of the game window"*/
        MESSAGE_STATUS_CONSOLE_BLUE = 27, /*FIXME Blue message in the console*/
    }
}
