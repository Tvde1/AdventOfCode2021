using System;
using System.Collections.Generic;

namespace AdventOfCode.Common;

public static class AStar
{
    public static (T Reuslt, int Cost) Calculate<T>(T source,
        Func<T, bool> completedFunc,
        Func<T, IEnumerable<T>> getNext,
        Func<T, int> getCost,
        Func<T, int> getHeuristic)
    {
        var visited = new HashSet<T>();
        var queue = new PriorityQueue<(T Item, int ActualCost), int>();

        queue.Enqueue((source, 0), 0);

        while (queue.TryDequeue(out var current, out _))
        {
            var (currentItem, currentCost) = current;

            if (completedFunc(currentItem))
            {
                return current;
            }

            visited.Add(currentItem);

            foreach (var next in getNext(currentItem))
            {
                if (visited.Contains(next))
                {
                    continue;
                }

                var nextCost = currentCost + getCost(next);
                var nextHeuristicCost = nextCost + getHeuristic(next);

                queue.Enqueue((next, nextCost), nextHeuristicCost);
            }
        }

        throw new ArgumentException("Could not find a completing path.");
    }
}