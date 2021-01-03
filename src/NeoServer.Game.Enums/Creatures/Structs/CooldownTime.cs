using System;

namespace NeoServer.Game.Common.Creatures.Structs
{
    public struct CooldownTime
    {
        public CooldownTime(DateTime start, int duration)
        {
            Start = start.Ticks;
            Duration = TimeSpan.TicksPerMillisecond * duration;
        }

        public long Start { get; set; }
        public long Duration { get; set; }
        public bool Expired
        {
            get
            {
                return Start + Duration <= DateTime.Now.Ticks;
            }
        }

        public void Reset() => Start = DateTime.Now.Ticks;
    }
}
