// <copyright file="SearchState.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

/*
From https://github.com/jbaldwin/astar.cs

Copyright (c) 2013 Josh Baldwin

Licensed under the MIT license.
See LICENSE file in the project root for full license information.
*/

namespace NeoServer.Game.Common.Location
{
    /// <summary>
    /// A* algorithm states while searching for the goal.
    /// </summary>
    public enum SearchState
    {
        /// <summary>
        /// The A* algorithm is still searching for the goal.
        /// </summary>
        Searching,

        /// <summary>
        /// The A* algorithm has found the goal.
        /// </summary>
        GoalFound,

        /// <summary>
        /// The A* algorithm has failed to find a solution.
        /// </summary>
        Failed
    }
}
