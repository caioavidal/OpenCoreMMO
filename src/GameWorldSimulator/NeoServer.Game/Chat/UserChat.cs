using NeoServer.Game.Chat.Rules;
using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Chat;

public class UserChat : IUserChat
{
    private const byte MAX_MUTED_TIMES = 36;
    private long FirstMessageBeforeMuted { get; set; }
    private ushort LastMutedForSeconds { get; set; }
    private byte MutedTimes { get; set; } //count how many times user get muted.
    public long LastMessage { get; private set; }
    public ushort MutedForSeconds { get; set; }
    public ushort MessagesCount { get; private set; }

    public int RemainingMutedSeconds => MutedForSeconds == 0
        ? 0
        : (int)Math.Round(
            TimeSpan.FromTicks(LastMessage + TimeSpan.TicksPerSecond * MutedForSeconds - DateTime.Now.Ticks)
                .TotalSeconds, MidpointRounding.AwayFromZero);

    public IPlayer Player { get; init; }
    public bool Removed { get; private set; }
    public bool IsMuted => RemainingMutedSeconds > 0;

    public void UpdateLastMessage(MuteRule rule)
    {
        var secondsSinceFirstMessage =
            (ushort)TimeSpan.FromTicks(DateTime.Now.Ticks - FirstMessageBeforeMuted).Seconds;

        if (secondsSinceFirstMessage > 0 && secondsSinceFirstMessage > rule.TimeToBlock) MessagesCount = 0;

        UpdateMessageCount();

        if (MessagesCount < rule.MessagesCount || secondsSinceFirstMessage > rule.TimeToBlock) return;

        MutedForSeconds = (ushort)rule.Formula(LastMutedForSeconds, MutedTimes);

        LastMutedForSeconds = MutedForSeconds;
        MutedTimes += (byte)(MutedTimes < MAX_MUTED_TIMES ? 1 : 0);
    }

    private void UpdateMessageCount()
    {
        if (IsMuted)
        {
            MessagesCount = 0;
            return;
        }

        MessagesCount += 1;
        LastMessage = DateTime.Now.Ticks;

        if (MessagesCount != 1) return;

        FirstMessageBeforeMuted = LastMessage;
        MutedForSeconds = 0;
    }

    public void MarkAsRemoved()
    {
        Removed = true;
    }

    public void MarkAsAdded()
    {
        Removed = false;
    }
}