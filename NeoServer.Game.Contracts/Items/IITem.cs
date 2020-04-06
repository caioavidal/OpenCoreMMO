// <copyright file="IITem.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Game.Contracts.Items
{
    public delegate void ItemHolderChangeEvent(IItem itemChanged, uint oldHolderId);

    public delegate void ItemAmountChangeEvent(IItem itemChanged, byte oldAmount);

    public interface IItem : IThing
    {
        event ItemHolderChangeEvent OnHolderChanged;

        event ItemAmountChangeEvent OnAmountChanged;

        IItemType Type { get; }

        Dictionary<ItemAttribute, IConvertible> Attributes { get; }

        uint HolderId { get; }

        IContainer Parent { get; }

        bool IsGround { get; }

        byte MovementPenalty { get; }

        bool IsTop1 { get; }

        bool IsTop2 { get; }

        bool ChangesOnUse { get; }

        ushort ChangeOnUseTo { get; }

        bool IsLiquidPool { get; }

        bool IsLiquidSource { get; }

        bool IsLiquidContainer { get; }

        byte LiquidType { get; }

        bool HasCollision { get; }

        bool HasSeparation { get; }

        bool BlocksThrow { get; }

        bool BlocksPass { get; }

        void StartDecaying();

        bool BlocksLay { get; }

        bool IsCumulative { get; }

        bool IsContainer { get; }

        bool IsDressable { get; }

        byte DressPosition { get; }

        byte Attack { get; }

        byte Defense { get; }

        byte Armor { get; }

        int Range { get; }

        decimal Weight { get; }

        byte Amount { get; set; }
        bool LoadedFromMap { get; set; }
        bool CanDecay { get; set; }
        bool HasCharges { get; }

        bool IsPathBlocking(byte avoidType = 0);

        void AddContent(IEnumerable<object> content);

        void SetHolder(ICreature creature, Location fromLocation);

        void SetAmount(byte remainingCount);

        void SetParent(IContainer parentContainer);

        bool Join(IItem otherItem);

        bool Separate(byte amount, out IItem splitItem);
    }
}
