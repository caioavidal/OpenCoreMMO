// <copyright file="INode.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

/*
astar-1.0.cs may be freely distributed under the MIT license.

Copyright (c) 2013 Josh Baldwin https://github.com/jbaldwin/astar.cs

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

using System.Collections.Generic;

namespace NeoServer.Game.Contracts
{
    /// <summary>
    /// The A* algorithm takes a starting node and a goal node and searchings from
    /// start to the goal.
    ///
    /// The nodes can be setup in a graph ahead of running the algorithm or the children
    /// nodes can be generated on the fly when the A* algorithm requests the Children property.
    ///
    /// See the square puzzle implementation to see the children being generated on the fly instead
    /// of the classical image/graph search with walls.
    /// </summary>
    public interface INode
    {
        bool IsInOpenList { get; set; }

        bool IsInClosedList { get; set; }

        ///// <summary>
        ///// Determines if this node is on the open list.
        ///// </summary>
        // bool IsOpenList(IEnumerable<INode> openList);

        ///// <summary>
        ///// Sets this node to be on the open list.
        ///// </summary>
        // void SetOpenList(bool value);

        ///// <summary>
        ///// Determines if this node is on the closed list.
        ///// </summary>
        // bool IsClosedList(IEnumerable<INode> closedList);

        ///// <summary>
        ///// Sets this node to be on the open list.
        ///// </summary>
        // void SetClosedList(bool value);

        /// <summary>
        /// Gets the total cost for this node.
        /// f = g + h
        /// TotalCost = MovementCost + EstimatedCost
        /// </summary>
        int TotalCost { get; }

        /// <summary>
        /// Gets the movement cost for this node.
        /// This is the movement cost from this node to the starting node, or g.
        /// </summary>
        int MovementCost { get; }

        /// <summary>
        /// Gets the estimated cost for this node.
        /// This is the heuristic from this node to the goal node, or h.
        /// </summary>
        int EstimatedCost { get; }

        /// <summary>
        /// Sets the movement cost for the current node, or g.
        /// </summary>
        /// <param name="parent">Parent node, for access to the parents movement cost.</param>
        void SetMovementCost(INode parent);

        /// <summary>
        /// Sets the estimated cost for the current node, or h.
        /// </summary>
        /// <param name="goal">Goal node, for acces to the goals position.</param>
        void SetEstimatedCost(INode goal);

        /// <summary>
        /// Gets or sets the parent node to this node.
        /// </summary>
        INode Parent { get; set; }

        /// <summary>
        /// Gets this node's children.
        /// </summary>
        /// <remarks>The children can be setup in a graph before starting the
        /// A* algorithm or they can be dynamically generated the first time
        /// the A* algorithm calls this property.</remarks>
        IEnumerable<INode> Children { get; }

        /// <summary>
        /// Returns true if this node is the goal, false if it is not the goal.
        /// </summary>
        /// <param name="goal">The goal node to compare this node against.</param>
        /// <returns>True if this node is the goal, false if it s not the goal.</returns>
        bool IsGoal(INode goal);
    }
}
