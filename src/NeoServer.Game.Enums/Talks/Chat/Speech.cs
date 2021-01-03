namespace NeoServer.Game.Common.Talks

{
    public struct Speech
    {
        public SpeechType Type { get; set; }

        public string Receiver { get; set; }

        public string Message { get; set; }

        public ChatChannel ChannelId { get; set; }

    }
}