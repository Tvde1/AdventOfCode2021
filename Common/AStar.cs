using System;
using System.Collections.Generic;

namespace AdventOfCode.Common;

public static class AStar
{
    public static int Calculate<TState, TModification>(TState source,
        Func<TState, IEnumerable<TModification>> getModifications,
        Func<TState, TModification, TState> applyModification,
        Func<TModification, int> getCost,
        Func<TState, int> getHeuristic)
    {
        var visited = new HashSet<TState>();
        var queue = new PriorityQueue<(TState Item, int ActualCost), int>();

        queue.Enqueue((source, 0), 0);

        while (queue.TryDequeue(out var current, out _))
        {
            var (currentItem, currentCost) = current;

            visited.Add(currentItem);

            foreach (var modification in getModifications(currentItem))
            {
                var newState = applyModification(currentItem, modification);

                if (visited.Contains(newState))
                {
                    continue;
                }

                var nextCost = currentCost + getCost(modification);
                var heuristic = getHeuristic(newState);

                if (heuristic == 0)
                {
                    return nextCost;
                }
                
                var nextHeuristicCost = nextCost + heuristic;

                queue.Enqueue((newState, nextCost), nextHeuristicCost);
            }
        }

        throw new ArgumentException("Could not find a completing path.");
    }
}