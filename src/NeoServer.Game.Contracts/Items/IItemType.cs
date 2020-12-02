// <copyright file="IItemType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using NeoServer.Game.Common;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Players;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Items
{
    public interface IItemType
    {
        ushort TypeId { get; }

        string Name { get; }

        string Description { get; }

        ISet<ItemFlag> Flags { get; }
        ItemTypeAttribute TypeAttribute { get; }

        ItemGroup Group { get; }

        ushort ClientId { get; }

        void SetArticle(string article);
        void SetPlural(string plural);

        ushort WareId { get; }
        LightBlock LightBlock { get; }
        ushort Speed { get; }
        string Article { get; }
        IItemAttributeList Attributes { get; }
        ShootType ShootType { get; }
        AmmoType AmmoType { get; }
        WeaponType WeaponType { get; }
        Slot BodyPosition { get; }
        float Weight { get; }
        ushort TransformTo { get; }
        string Plural { get; }

        void SetName(string value);
        void LockChanges();
        void SetSpeed(ushort speed);
        void SetLight(LightBlock lightBlock);
        void SetWareId(ushort wareId);
        bool HasFlag(ItemFlag flag);
    }
}
