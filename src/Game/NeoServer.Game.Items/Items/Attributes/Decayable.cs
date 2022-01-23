using System;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Items.Items.Attributes;

public class Decayable : IDecayable
{
    private readonly IItem _item;

    private uint _duration;
    private bool _isPaused = true;

    private uint _lastElapsed;
    private ulong _startedToDecayTime;

    public Decayable(IItem item)
    {
        _item = item;
    }

    private bool ShowDuration =>
        !_item.Metadata.Attributes.TryGetAttribute<byte>(ItemAttribute.ShowDuration, out var showDuration) ||
        showDuration == 1;

    public bool StartedToDecay => _startedToDecayTime != default;

    public event DecayDelegate OnDecayed;
    public event PauseDecay OnPaused;
    public event StartDecay OnStarted;

    public ushort DecaysTo => _item.Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.ExpireTarget);

    public uint Duration => _duration = _item.Metadata.Attributes.GetAttribute<uint>(ItemAttribute.Duration) == 0
        ? _duration
        : _item.Metadata.Attributes.GetAttribute<uint>(ItemAttribute.Duration);

    public uint Remaining => Duration <= Elapsed ? 0 : Math.Max(0, Duration - Elapsed);

    public uint Elapsed
    {
        get
        {
            if (_isPaused) return _lastElapsed;
            var elapsedSeconds = _startedToDecayTime == 0
                ? 0
                : (uint)Math.Ceiling(((ulong)DateTime.Now.Ticks - _startedToDecayTime) /
                                     (decimal)TimeSpan.TicksPerSecond);

            return _lastElapsed + elapsedSeconds;
        }
    }
    // (uint)Math.Max(0, _isPaused
    //     ? _lastElapsed
    //     : _lastElapsed +
    //       (int)Math.Ceiling((DateTime.Now.Ticks - _startedToDecayTime) / (decimal)TimeSpan.TicksPerSecond));

    public bool Expired => Elapsed >= Duration;
    public bool ShouldDisappear => DecaysTo == default;

    public void StartDecay()
    {
        if (Expired) return;
        _isPaused = false;
        _startedToDecayTime = (ulong)DateTime.Now.Ticks;
        OnStarted?.Invoke(this);
    }

    public void PauseDecay()
    {
        if (_startedToDecayTime == 0) return;
        _isPaused = true;
        _lastElapsed += (uint)(((ulong)DateTime.Now.Ticks - _startedToDecayTime) / TimeSpan.TicksPerSecond);
        OnPaused?.Invoke(this);
    }

    public bool TryDecay()
    {
        if (!Expired) return false;

        var decaysTo = DecaysTo;

        Reset();

        OnDecayed?.Invoke(decaysTo);

        return true;
    }

    private void Reset()
    {
        _startedToDecayTime = default;
        _isPaused = default;
        _lastElapsed = default;
    }

    public override string ToString()
    {
        if (!ShowDuration) return string.Empty;
        if (Elapsed == 0) return "is brand-new";

        var minutes = Math.Max(0, Remaining / 60);
        var seconds = Math.Max(0, (int)Math.Truncate(Remaining % 60d));
        return
            $"will expire in {minutes} minute{(minutes > 1 ? "s" : "")} and {seconds} second{(seconds > 1 ? "s" : "")}";
    }
}