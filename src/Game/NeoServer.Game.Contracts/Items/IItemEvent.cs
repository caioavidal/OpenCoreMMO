// <copyright file="IItemEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using NeoServer.Game.Common;
using NeoServer.Server.Model.Players.Contracts;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Items
{
    public interface IItemEvent
    {
        ItemEventType Type { get; }

        IThing Obj1 { get; }

        IThing Obj2 { get; }

        IPlayer User { get; }

        bool CanBeExecuted { get; }

        IEnumerable<IItemEventFunction> Conditions { get; }

        IEnumerable<IItemEventFunction> Actions { get; }

        bool Setup(IThing obj1, IThing obj2 = null, IPlayer user = null);

        void Execute();
    }
}