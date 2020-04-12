namespace NeoServer.Server.Model.Chat
{
    public enum SpeechType : byte
    {
        Say = 0x01, // normal talk
        Whisper = 0x02, // whispering - #w text
        Yell = 0x03,    // yelling - #y text
        Private = 0x04, // Players speaking privately to players
        ChannelYellow = 0x05,   // Yellow message in chat
        RuleViolationReport = 0x06, // Reporting rule violation - Ctrl+R
        RuleViolationAnswer = 0x07, // Answering report
        RuleViolationContinue = 0x08, // Answering the answer of the report
        Broadcast = 0x09,   // Broadcast a message - #b
        // ChannelRed = 0x05,   //Talk red on chat - #c
        // PrivateRed = 0x04,   //Red private - @name@ text
        // ChannelOrange = 0x05,    //Talk orange on text
        // ChannelRedAnonymous = 0x05,  //Talk red anonymously on chat - #d
        MonsterSay = 0x0E // Talk orange
        // MonsterYell = 0x0E,  //Yell orange
    }
}
