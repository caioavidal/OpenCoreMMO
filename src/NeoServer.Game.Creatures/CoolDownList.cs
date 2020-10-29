using NeoServer.Game.Contracts.Spells;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Creatures.Spells;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures
{
    public class CooldownList
    {
        public IDictionary<CooldownType, CooldownTime> Cooldowns { get; } = new Dictionary<CooldownType, CooldownTime>();
        public IDictionary<ISpell, CooldownTime> Spells { get; } = new Dictionary<ISpell, CooldownTime>();

        /// <summary>
        /// Add cooldown
        /// </summary>
        /// <param name="type"></param>
        /// <param name="duration">milliseconds</param>
        public bool Start(CooldownType type, int duration) => Cooldowns.TryAdd(type, new CooldownTime(DateTime.Now, duration));

        public void Add(ISpell spell, int duration)
        {
            Spells.TryAdd(spell, new CooldownTime(DateTime.Now, duration));
        }
        public bool Expired(CooldownType type)
        {
            if (Cooldowns.TryGetValue(type, out var cooldown))
            {
                return cooldown.Expired;
            }
            return true;
        }
        public void RestartCoolDown(CooldownType type, int duration)
        {
            if (Cooldowns.ContainsKey(type))
            {
                Cooldowns[type].Reset();
            }
        }

    }
    public struct CooldownTime
    {
        public CooldownTime(DateTime start, int duration)
        {
            Start = start.Ticks;
            Duration = duration;
        }

        public long Start { get; set; }
        public int Duration { get; set; }
        public bool Expired => Start + Duration < DateTime.Now.Ticks;

        public void Reset() => Start = DateTime.Now.Ticks;
    }
}
