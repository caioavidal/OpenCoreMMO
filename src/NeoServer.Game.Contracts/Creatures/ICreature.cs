using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Enums.Creatures.Players;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void OnCreatureStateChange();
    public delegate void OnTurnedToDirection(ICreature creature, Direction direction);
    public delegate void RemoveCreature(ICreature creature);
    public delegate void StopWalk(ICreature creature);
    public delegate void Die(ICreature creature);
    public delegate void GainExperience(ICreature creature, uint exp);
    public delegate void StartWalk(ICreature creature);
    public delegate void Heal(ICreature creature, ushort amount);

    public interface ICreature : INeedsCooldowns, IMoveableThing, ICombatActor
    {
        // event OnCreatureStateChange OnZeroHealth;
        // event OnCreatureStateChange OnInventoryChanged;
        uint CreatureId { get; }

        ushort Corpse { get; }

        /// <summary>
        /// Health points
        /// </summary>
        /// <value></value>
        uint HealthPoints { get; }

        uint MaxHealthpoints { get; }

        IOutfit Outfit { get; }

        Direction Direction { get; }

        Direction ClientSafeDirection { get; }

        byte LightBrightness { get; }

        byte LightColor { get; }

        ushort Speed { get; }

        uint Flags { get; }

        IWalkableTile Tile { get; set; }

        bool IsInvisible { get; } // TODO: implement.

        bool CanSeeInvisible { get; } // TODO: implement.

        bool IsDead { get; }

        void GainExperience(uint exp);

        bool IsRemoved { get; }
        int StepDelayMilliseconds { get; }
        double LastStep { get; }

        bool TryGetNextStep(out Direction direction);

        bool StopWalkingRequested { get; set; }
        uint EventWalk { get; set; }
        byte Skull { get; }
        byte Shield { get; }
        bool IsHealthHidden => false;

        bool IsFollowing { get; }
        uint Following { get; }
        bool HasNextStep { get; }

        event OnTurnedToDirection OnTurnedToDirection;
        event RemoveCreature OnCreatureRemoved;
        event StopWalk OnStoppedWalking;
        event Die OnKilled;
        event StartWalk OnStartedWalking;
        event Heal OnHeal;

        /// <summary>
        /// Checks whether creature can see another creature or not
        /// </summary>
        /// <param name="creature">Creeature to see</param>
        /// <returns></returns>
        bool CanSee(ICreature creature);

        /// <summary>
        /// Checks whether creature can see location or not
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        bool CanSee(Location location);

        /// <summary>
        /// Turns creature to direction
        /// </summary>
        /// <param name="direction"></param>
        void TurnTo(Direction direction);

        void StopWalking();

        bool TryWalkTo(params Direction[] directions);

        double CalculateRemainingCooldownTime(CooldownType type, DateTime currentTime);

        void UpdateLastStepInfo(byte lastStepId, bool wasDiagonal = true);
        byte[] GetRaw(IPlayer playerRequesting);
        void SetAsRemoved();
        void ReceiveAttack(ICreature enemy, ICombatAttack attack, ushort damage);
        void ResetHealthPoints();
        void StartFollowing(uint id, params Direction[] directions);
        void SetDirection(Direction direction);
        void StopFollowing();
        bool TryUpdatePath(Direction[] newPath);
        void IncreaseSpeed(ushort speed);
        void Heal(ushort increasing);
        void DecreaseSpeed(ushort speedBoost);
        void AddCondition(ICondition condition);
        bool HasCondition(ConditionType type, out ICondition condition);
    }
}
