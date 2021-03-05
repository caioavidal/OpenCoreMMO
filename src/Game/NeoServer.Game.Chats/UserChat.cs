using NeoServer.Game.Contracts.Chats;
using NeoServer.Server.Model.Players.Contracts;
using System;

namespace NeoServer.Game.Chats
{
    public class UserChat : IUserChat
    {
        private const byte MAX_MUTED_TIMES = 36;

        public IPlayer Player { get; init; }
        public long LastMessage { get; private set; }
        public ushort MutedForSeconds { get; set; }
        public bool Removed { get; private set; }
        private long firstMessageBeforeMuted;
        private ushort lastMutedForSeconds;
        public ushort MessagesCount { get; private set; }
        public int RemainingMutedSeconds => MutedForSeconds == 0 ? 0 : (int)Math.Round(TimeSpan.FromTicks((LastMessage + (TimeSpan.TicksPerSecond * MutedForSeconds)) - DateTime.Now.Ticks).TotalSeconds,MidpointRounding.AwayFromZero);
        private byte mutedTimes; //count how many times user get muted.
        public bool IsMuted => RemainingMutedSeconds > 0;
        public void UpdateLastMessage(MuteRule rule)
        {
            ushort secondsSinceFirstMessage = (ushort)TimeSpan.FromTicks(DateTime.Now.Ticks - firstMessageBeforeMuted).Seconds;

            if (secondsSinceFirstMessage > 0 && secondsSinceFirstMessage > rule.TimeToBlock) MessagesCount = 0;

            if (!IsMuted)
            {
                MessagesCount += 1;
                LastMessage = DateTime.Now.Ticks;
                if (MessagesCount == 1)
                {
                    firstMessageBeforeMuted = LastMessage;
                    MutedForSeconds = 0;
                }
            }
            else
            {
                MessagesCount = 0;
            }

            if (MessagesCount >= rule.MessagesCount && secondsSinceFirstMessage <= rule.TimeToBlock)
            {
                MutedForSeconds = (ushort)rule.Formula(lastMutedForSeconds, mutedTimes);

                lastMutedForSeconds = MutedForSeconds;
                mutedTimes += mutedTimes < MAX_MUTED_TIMES ? (byte)1 : (byte)0;
            }
        }

        public void MarkAsRemoved() => Removed = true;
        public void MarkAsAdded() => Removed = false;

    }

}
