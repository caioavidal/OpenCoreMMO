using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Chats
{
    public struct MuteRule
    {
        public static MuteRule Default => new MuteRule { MessagesCount = 5, TimeMultiplier = 10, TimeToBlock = 10, WaitTime = 5 };
        public bool None => MessagesCount == default && TimeToBlock == default && WaitTime == default && TimeMultiplier == default && CancelMessage == default;
        public ushort MessagesCount { get; set; }
        public ushort TimeToBlock { get; set; }
        public ushort WaitTime { get; set; }
        public double TimeMultiplier { get; set; }
        public string CancelMessage { get; set; }

        public uint Formula(ushort lastMutedSeconds, byte mutedTimes) => (uint)(lastMutedSeconds + (TimeMultiplier * mutedTimes) + WaitTime);
    }
}
