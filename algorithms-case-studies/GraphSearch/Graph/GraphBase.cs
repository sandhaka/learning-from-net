using GraphSearch.Graph.Abstractions;
using GraphSearch.Graph.NodeCollections.Abstractions;
using Monads.Optional;

namespace GraphSearch.Graph;

public delegate void OnVisit<in T>(T node);

internal abstract class GraphBase<T> : IGraph<T>
{
    protected INodeCollection<T> NodesCollection;

    public IReadOnlySet<T> NodeValues => NodesCollection.Values;

    public Option<OnVisit<T>> OnVisitActionParameter { get; set; } = Option<OnVisit<T>>.None();

    public IGraphSearch<T> ToSearchGraph() => new GraphSearch<T>(this);

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