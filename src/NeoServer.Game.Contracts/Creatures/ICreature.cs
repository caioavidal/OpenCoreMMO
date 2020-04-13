// <copyright file="ICreature.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using NeoServer.Game.Creatures.Enums;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Creatures
{
    public delegate void OnCreatureStateChange();
    public delegate void OnTurnedToDirection(ICreature creature, Direction direction);
    public delegate void RemoveCreature(ICreature creature);
    public delegate void StopWalk(ICreature creature);

    public interface ICreature : IThing, INeedsCooldowns
    {
        // event OnCreatureStateChange OnZeroHealth;
        // event OnCreatureStateChange OnInventoryChanged;
        uint CreatureId { get; }

        string Article { get; }

        string Name { get; }

        ushort Corpse { get; }

        uint Hitpoints { get; }

        uint MaxHitpoints { get; }

        uint Manapoints { get; }

        uint MaxManapoints { get; }

        decimal CarryStrength { get; }

        IOutfit Outfit { get; }

        Direction Direction { get; }

        Direction ClientSafeDirection { get; }

        byte LightBrightness { get; }

        byte LightColor { get; }

        ushort Speed { get; }

        uint Flags { get; }

        ConcurrentQueue<Tuple<byte, Direction>> WalkingQueue { get; }

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
        bool StopWalkingRequested { get; set; }
        List<uint> NextSteps { get; set; }

        event OnTurnedToDirection OnTurnedToDirection;
        event RemoveCreature OnCreatureRemoved;
        event StopWalk OnStoppedWalking;

        bool CanSee(ICreature creature);

        bool CanSee(Location location);

        void TurnTo(Direction direction);

        void StopWalking();

        bool TryWalkTo(params Direction[] directions);

        TimeSpan CalculateRemainingCooldownTime(CooldownType type, DateTime currentTime);

        void UpdateLastStepInfo(byte lastStepId, bool wasDiagonal = true);
        void SignalNextWalkEvent();
    }
}
