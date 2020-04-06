// <copyright file="IContainer.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using NeoServer.Game.Contracts.Items;

namespace NeoServer.Game.Contracts
{
    public delegate void OnContentAdded(IContainer container, IItem item);

    public delegate void OnContentUpdated(IContainer container, byte index, IItem item);

    public delegate void OnContentRemoved(IContainer container, byte index);

    public interface IContainer : IItem
    {
        event OnContentAdded OnContentAdded;

        event OnContentUpdated OnContentUpdated;

        event OnContentRemoved OnContentRemoved;

        Dictionary<uint, byte> OpenedBy { get; }

        IList<IItem> Content { get; }

        byte Volume { get; }

        bool AddContent(IItem item, byte index);

        bool RemoveContent(ushort itemId, byte index, byte count, out IItem splitItem);

        sbyte CountContentAmountAt(byte fromIndex, ushort itemIdExpected = 0);

        byte Open(uint creatureOpeningId, byte containerId);

        void Close(uint creatureClosingId);

        sbyte GetIdFor(uint creatureId);
    }
}
