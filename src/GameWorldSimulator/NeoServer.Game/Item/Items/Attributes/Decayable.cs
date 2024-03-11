using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Item.Items.Attributes;

public record ItemStartedToDecayEvent(IItem Item) : IGameEvent;

public class Decayable : IDecayable
{
    private readonly IItem _item;

    private uint _duration;

    private uint _lastElapsedSeconds;
    private ulong _startedToDecayTime;

    public Decayable(IItem item)
    {
        _lastElapsedSeconds = item.Metadata.Attributes.GetAttribute<uint>(ItemAttribute.DecayElapsed);
        _item = item;
    }

    private bool ShowDuration =>
        !_item.Metadata.Attributes.TryGetAttribute<byte>(ItemAttribute.ShowDuration, out var showDuration) ||
        showDuration == 1;

    public bool StartedToDecay => _startedToDecayTime != default;
    public bool IsPaused { get; private set; } = true;
    public event PauseDecay OnPaused;
    public event StartDecay OnStarted;

    public ushort DecaysTo => _item.Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.ExpireTarget);

    public uint Duration => _duration = _item.Metadata.Attributes.GetAttribute<uint>(ItemAttribute.Duration) == 0
        ? _duration
        : _item.Metadata.Attributes.GetAttribute<uint>(ItemAttribute.Duration);

    public uint Remaining => Duration <= ElapsedSeconds ? 0 : Math.Max(0, Duration - ElapsedSeconds);

    public uint ElapsedSeconds
    {
        get
        {
            if (IsPaused) return _lastElapsedSeconds;
            var elapsedSeconds = _startedToDecayTime == 0
                ? 0
                : (uint)Math.Floor(((ulong)DateTime.Now.Ticks - _startedToDecayTime) /
                                   (decimal)TimeSpan.TicksPerSecond);

            return _lastElapsedSeconds + elapsedSeconds;
        }
    }

    public bool Expired => ElapsedSeconds >= Duration;
    public bool ShouldDisappear => DecaysTo == default;

    public void StartDecay()
    {
        if (Expired || !IsPaused || Duration is 0) return;
        
        IsPaused = false;
        _startedToDecayTime = (ulong)DateTime.Now.Ticks;

        OnStarted?.Invoke(_item);
    }

    public void PauseDecay()
    {
        if (_startedToDecayTime == 0) return;
        IsPaused = true;
        _lastElapsedSeconds += (uint)(((ulong)DateTime.Now.Ticks - _startedToDecayTime) / TimeSpan.TicksPerSecond);
        OnPaused?.Invoke(this);
    }

    public bool TryDecay()
    {
        if (!Expired) return false;

        Reset();

        return true;
    }

    public Queue<IGameEvent> Events { get; } = new();

    public void RaiseEvent(IGameEvent gameEvent)
    {
        Events.Enqueue(gameEvent);
    }

    private void Reset()
    {
        _startedToDecayTime = default;
        IsPaused = true;
        _lastElapsedSeconds = default;
        _duration = default;
    }

    public override string ToString()
    {
        if (!ShowDuration) return string.Empty;
        if (!StartedToDecay) return "is brand-new";

        var minutes = Math.Max(0, Remaining / 60);
        var seconds = Math.Max(0, (int)Math.Truncate(Remaining % 60d));
        return
            $"will expire in {minutes} minute{(minutes > 1 ? "s" : "")} and {seconds} second{(seconds > 1 ? "s" : "")}";
    }
}