using NeoServer.Server.Model.Items;
using NeoServer.Server.Model.Items.Contracts;
using NeoServer.Server.Model.World.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Model.Creatures.Contracts
{
    public delegate void OnAttackTargetChange(uint oldTargetId, uint newTargetId);

    public interface ICombatActor : INeedsCooldowns
    {
        event OnAttackTargetChange OnTargetChanged;

        /// <summary>
        /// Gets the id of the actor.
        /// </summary>
        uint ActorId { get; }

        /// <summary>
        /// Gets the blood type of the actor.
        /// </summary>
        BloodType Blood { get; }

        uint AutoAttackTargetId { get; }

        byte AutoAttackRange { get; }

        byte AutoAttackCredits { get; }

        byte AutoDefenseCredits { get; }

        DateTime LastAttackTime { get; }

        TimeSpan LastAttackCost { get; }

        TimeSpan CombatCooldownTimeRemaining { get; }

        /// <summary>
        /// Gets a metric of how fast an Actor can earn a new AutoAttack credit per second.
        /// </summary>
        decimal BaseAttackSpeed { get; }

        /// <summary>
        /// Gets a metric of how fast an Actor can earn a new AutoDefense credit per second.
        /// </summary>
        decimal BaseDefenseSpeed { get; }

        ushort AttackPower { get; }

        ushort DefensePower { get; }

        ushort ArmorRating { get; }

        Location Location { get; }

        void SetAttackTarget(uint targetId);

        void UpdateLastAttack(TimeSpan cost);

        void CheckAutoAttack(IThing thingChanged, ThingStateChangedEventArgs eventAgrs);
    }
}
