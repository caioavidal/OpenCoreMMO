// <copyright file="IItemEventFunction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace NeoServer.Game.Contracts.Items
{
    public interface IItemEventFunction
    {
        string FunctionName { get; }

        object[] Parameters { get; }
    }
}