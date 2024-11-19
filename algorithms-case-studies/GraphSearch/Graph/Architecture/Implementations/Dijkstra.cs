using GraphSearch.Graph.Search;
using Monads.Optional;

namespace GraphSearch.Graph.Architecture.Implementations;

internal class Dijkstra<T> : IGraphSearchStrategy<T>
    where T : IEquatable<T>
{
    public string Name => "Dijkstra";
    
    // ReSharper disable once CognitiveComplexity
    public bool Run(IPathSearchContext<T> context, out Option<SearchResult<T>> result)
    {
        bool found = false;
        result = Option<SearchResult<T>>.None();
        var nodeValuesSet = context.NodeValues;
        var queue = new PriorityQueue<T, decimal>();
        
        var visited = nodeValuesSet.ToDictionary(x => x, _ => false);
        var distances = nodeValuesSet.ToDictionary(x => x, _ => decimal.MaxValue);
        var prev = nodeValuesSet.ToDictionary(x => x, _ => default(T));
        
        queue.Enqueue(context.Start, 0);
        distances[context.Start] = 0;

        while (queue.Count > 0)
        {
            var nv = queue.Dequeue();
            visited[nv] = true;

            foreach (var (nextValue, cost) in context.Neighbors(nv))
            {
                if (visited[nextValue])
                    continue;
                
                var distance = distances[nv] + cost;

                if (distance >= distances[nextValue]) 
                    continue;
                
                prev[nextValue] = nv;
                distances[nextValue] = distance;
                queue.Enqueue(nextValue, distance);
            }

            if (!nv.Equals(context.Target)) 
                continue;
            
            found = true;
            break;
        }

        if (found is false)
            return false;
        
        result = new SearchResult<T>
        {
            Path = GenPath(prev, context.Target).Reverse().ToList(),
            TotalCost = distances[context.Target]
        };

        return true;
    }

    private static IEnumerable<T> GenPath(Dictionary<T, T> prev, T target)
    {
        T n = target;
        yield return n;
        while (prev[n] is not null)
        {
            yield return prev[n];
            n = prev[n];
        }
    }
}