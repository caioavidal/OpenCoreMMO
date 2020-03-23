// <copyright file="IItemType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using NeoServer.Game.Enums;

namespace NeoServer.Game.Contracts.Item
{
    public interface IItemType
    {
        ushort TypeId { get; }

        string Name { get; }

        string Description { get; }

        ISet<ItemFlag> Flags { get; }

        IDictionary<ItemAttribute, IConvertible> DefaultAttributes { get; } // TODO: get rid of this and add all attributes as properties.

        ushort ClientId { get; }
    }
}
