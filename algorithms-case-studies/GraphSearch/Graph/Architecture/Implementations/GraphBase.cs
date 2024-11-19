using GraphSearch.Graph.Architecture.Abstractions;
using GraphSearch.Graph.Architecture.Components;
using GraphSearch.Graph.Architecture.NodeCollections.Abstractions;
using GraphSearch.Graph.Parameters;
using GraphSearch.Graph.Search;
using Monads.Optional;

namespace GraphSearch.Graph.Architecture.Implementations;

internal abstract class GraphBase<T> : IGraph<T>, IGraphComponents<T> 
    where T : IEquatable<T>
{
    protected INodeCollection<T> NodesCollection;

    public IReadOnlySet<T> NodeValues => NodesCollection.Values;
    public Node<T> this[T value] => NodesCollection[value];
    public Option<OnVisit<T>> OnVisitActionParameter { get; set; } = Option<OnVisit<T>>.None();

    public IPathSearch<T> ToPathSearch() => new PathSearch<T>(this);

    public void TraverseDfs(T start)
    {
        if (!NodesCollection.Contains(start)) return;
        
        var node = NodesCollection[start];
        var visited = new HashSet<Node<T>>();
        var stack = new Stack<Node<T>>([node]);

        while (stack.Count > 0)
        {
            node = stack.Pop();
            
            visited.Add(node);
            ActionExecute(node);
            
            var childrenLength = node.Neighbors.Length;
            
            for (var i = 0; i < childrenLength; i++)
            {
                var child = node.Neighbors.Span[i];
                if (visited.Contains(child.To)) 
                    continue;
                
                stack.Push(child.To);
            }
        }
    }
    
    public void TraverseBfs(T start)
    {
        if (!NodesCollection.Contains(start)) return;
        
        var node = NodesCollection[start];
        var visited = new HashSet<Node<T>>() { node };
        var queue = new Queue<Node<T>>([node]);
        
        while (queue.Count > 0)
        {
            node = queue.Dequeue();
            
            ActionExecute(node);
            
            var childrenLength = node.Neighbors.Length;
            
            for (var i = 0; i < childrenLength; i++)
            {
                var child = node.Neighbors.Span[i];
                if (!visited.Add(child.To)) 
                    continue;
                
                queue.Enqueue(child.To);
            }
        }
    }

    protected virtual void ActionExecute(Node<T> node)
    {
        var action = OnVisitActionParameter.Reduce(_ => { });
        
        action(node.Value);
    }
}