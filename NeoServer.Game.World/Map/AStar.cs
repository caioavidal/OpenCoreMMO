// <copyright file="AStar.cs" company="2Dudes">
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

using System.Collections.Generic;
using NeoServer.Game.Contracts;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs.Helpers;
using NeoServer.Server.Helpers.Extensions;

namespace NeoServer.Game.World.PathFinding
{


    /// <summary>
    /// Interface to setup and run the AStar algorithm.
    /// </summary>
    public class AStar {
		/// <summary>
		/// The open list.
		/// </summary>
		private readonly SortedList<int, INode> _openList;

		/// <summary>
		/// The closed list.
		/// </summary>
		private readonly SortedList<int, INode> _closedList;

		/// <summary>
		/// The current node.
		/// </summary>
		private INode _current;

		/// <summary>
		/// The goal node.
		/// </summary>
		private INode _goal;

		/// <summary>
		/// Gets the current amount of steps that the algorithm has performed.
		/// </summary>
		public int Steps { get; private set; }

		public int MaxSteps { get; }

		/// <summary>
		/// Gets the current state of the open list.
		/// </summary>
		public IEnumerable<INode> OpenList => _openList.Values;

		/// <summary>
		/// Gets the current state of the closed list.
		/// </summary>
		public IEnumerable<INode> ClosedList => _closedList.Values;

		/// <summary>
		/// Gets the current node that the AStar algorithm is at.
		/// </summary>
		public INode CurrentNode => _current;

		/// <summary>
		/// Initializes a new instance of the <see cref="AStar"/> class.
		/// </summary>
		/// <param name="start">The starting node for the AStar algorithm.</param>
		/// <param name="goal">The goal node for the AStar algorithm.</param>
		/// <param name="maxSearchSteps">Optional. The maximum number of Step operations to perform on the search.</param>
		public AStar(INode start, INode goal, int maxSearchSteps = 100) {
			var duplicateComparer = new DuplicateComparer();
			_openList = new SortedList<int, INode>(duplicateComparer);
			_closedList = new SortedList<int, INode>(duplicateComparer);
			Reset(start, goal);
			MaxSteps = maxSearchSteps;
		}

		/// <summary>
		/// Resets the AStar algorithm with the newly specified start node and goal node.
		/// </summary>
		/// <param name="start">The starting node for the AStar algorithm.</param>
		/// <param name="goal">The goal node for the AStar algorithm.</param>
		public void Reset(INode start, INode goal) {
			_openList.Clear();
			_closedList.Clear();
			_current = start;
			_goal = goal;
			_openList.Add(_current);
			_current.IsInOpenList = true;
		}

		/// <summary>
		/// Steps the AStar algorithm forward until it either fails or finds the goal node.
		/// </summary>
		/// <returns>Returns the state the algorithm finished in, Failed or GoalFound.</returns>
		public SearchState Run() {
			// Continue searching until either failure or the goal node has been found.
			while (true) {
				var s = Step();
				if (s != SearchState.Searching) {
					return s;
				}
			}
		}

		/// <summary>
		/// Moves the AStar algorithm forward one step.
		/// </summary>
		/// <returns>Returns the state the alorithm is in after the step, either Failed, GoalFound or still Searching.</returns>
		public SearchState Step() {
			if (MaxSteps > 0 && Steps == MaxSteps) {
				return SearchState.Failed;
			}

			Steps++;

			while (true) {
				// There are no more nodes to search, return failure.
				if (_openList.IsEmpty()) {
					return SearchState.Failed;
				}

				// Check the next best node in the graph by TotalCost.
				_current = _openList.Pop();

				// This node has already been searched, check the next one.
				if (_current.IsInClosedList) {
					continue;
				}

				// An unsearched node has been found, search it.
				break;
			}

			// Remove from the open list and place on the closed list
			// since this node is now being searched.
			_current.IsInOpenList = false;
			_closedList.Add(_current);
			_current.IsInClosedList = true;

			// Found the goal, stop searching.
			if (_current.IsGoal(_goal)) {
				return SearchState.GoalFound;
			}

			// Node was not the goal so add all children nodes to the open list.
			// Each child needs to have its movement cost set and estimated cost.
			foreach (var child in _current.Children) {
				// If the child has already been searched (closed list) just ignore.
				if (child.IsInClosedList) {
					continue;
				}

				// If the child has already beem searched, lets see if we get a better MovementCost by setting this parent instead.
				if (child.IsInOpenList) {
					var oldCost = child.MovementCost;
					child.SetMovementCost(_current);

					if (oldCost != child.MovementCost) {
						// it's better with this parent.
						child.Parent = _current;
					}

					continue;
				}

				child.Parent = _current;
				child.SetMovementCost(_current);
				child.SetEstimatedCost(_goal);
				_openList.Add(child);
				child.IsInOpenList = true;
			}

			// This step did not find the goal so return status of still searching.
			return SearchState.Searching;
		}

		/// <summary>
		/// Gets the path of the last solution of the AStar algorithm.
		/// Will return a partial path if the algorithm has not finished yet.
		/// </summary>
		/// <returns>Returns null if the algorithm has never been run.</returns>
		public IEnumerable<INode> GetPath() {
			if (_current != null) {
				var next = _current;
				var path = new List<INode>();
				while (next != null) {
					path.Add(next);
					next = next.Parent;
				}

				path.Reverse();
				return path.ToArray();
			}

			return null;
		}
	}
}
