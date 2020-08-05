using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Enums.Creatures;
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

    public interface ICreature : INeedsCooldowns, IMoveableThing
    {
        // event OnCreatureStateChange OnZeroHealth;
        // event OnCreatureStateChange OnInventoryChanged;
        uint CreatureId { get; }

        string Name { get; }

        ushort Corpse { get; }

        /// <summary>
        /// Health points
        /// </summary>
        /// <value></value>
        uint Hitpoints { get; }

        uint MaxHitpoints { get; }

        uint Manapoints { get; }

        uint MaxManapoints { get; }

        float CarryStrength { get; }

        IOutfit Outfit { get; }

        Direction Direction { get; }

        Direction ClientSafeDirection { get; }

        byte LightBrightness { get; }

        byte LightColor { get; }

        ushort Speed { get; }

        uint Flags { get; }


        IWalkableTile Tile { get; set; }

        byte NextStepId { get; set; }

        IDictionary<SkillType, ISkill> Skills { get; }

        bool IsInvisible { get; } // TODO: implement.

        bool CanSeeInvisible { get; } // TODO: implement.

        byte Skull { get; } // TODO: implement.

        byte Shield { get; } // TODO: implement.


        IInventory Inventory { get; }
        bool IsDead { get; }
        bool IsRemoved { get; }
        int StepDelayMilliseconds { get; }
        double LastStep { get; }

        bool TryGetNextStep(out Direction direction);

        bool StopWalkingRequested { get; set; }
        List<uint> NextSteps { get; set; }
        bool CancelNextWalk { get; set; }
        uint EventWalk { get; set; }

        event OnTurnedToDirection OnTurnedToDirection;
        event RemoveCreature OnCreatureRemoved;
        event StopWalk OnStoppedWalking;

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

        TimeSpan CalculateRemainingCooldownTime(CooldownType type, DateTime currentTime);

        void UpdateLastStepInfo(byte lastStepId, bool wasDiagonal = true);
        byte[] GetRaw(IPlayer playerRequesting);
        void SetAsRemoved();
    }
}
