using NeoServer.Game.Creatures.Enums;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface INeedsCooldowns
    {
        /// <summary>
        /// Gets the information about cooldowns for a creature where the key is a <see cref="CooldownType"/>, and the value is a <see cref="Tuple{T1, T2}"/>
        /// </summary>
        /// <remarks>The tuple elements are a <see cref="DateTime"/>, to store the time when the cooldown started, and a <see cref="TimeSpan"/> to denote how long it should last.</remarks>
        IDictionary<CooldownType, Tuple<DateTime, TimeSpan>> Cooldowns { get; }
    }
}
