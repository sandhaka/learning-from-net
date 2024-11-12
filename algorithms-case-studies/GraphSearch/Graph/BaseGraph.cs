using GraphSearch.Graph.NodeCollections.Abstractions;
using Monads.Optional;

namespace GraphSearch.Graph;

public delegate void OnVisit<in T>(T node);

internal abstract class BaseGraph<T> : IGraph<T> where T : class
{
    protected INodeCollection<T> NodesCollection;
    
    public ISet<T> NodeValues => NodesCollection.Nodes.Select(n => n.Value).ToHashSet();
    public Option<OnVisit<T>> OnVisitActionParameter { get; set; } = Option<OnVisit<T>>.None();

    public void Dfs(T start)
    {
        if (!NodesCollection.Contains(start)) return;
        
        var node = NodesCollection[start];
        var visited = new HashSet<Node<T>>();
        var stack = new Stack<Node<T>>([node]);

        while (stack.Count > 0)
        {
            node = stack.Pop();
            
            // Visit
            visited.Add(node);
            ActionExecute(node);
            
            var childrenLength = node.Neighbors.Length;
            
            for (var i = 0; i < childrenLength; i++)
            {
                var child = node.Neighbors.Span[i];
                if (visited.Contains(child)) 
                    continue;
                
                stack.Push(child);
            }
        }
    }
    
    public void Bfs(T start)
    {
        if (!NodesCollection.Contains(start)) return;
        
        var node = NodesCollection[start];
        var visited = new HashSet<Node<T>>() { node };
        var queue = new Queue<Node<T>>([node]);
        
        while (queue.Count > 0)
        {
            node = queue.Dequeue();
            
            // Visit
            ActionExecute(node);
            
            var childrenLength = node.Neighbors.Length;
            
            for (var i = 0; i < childrenLength; i++)
            {
                var child = node.Neighbors.Span[i];
                if (!visited.Add(child)) 
                    continue;
                
                queue.Enqueue(child);
            }
        }
    }

    protected virtual void ActionExecute(Node<T> node)
    {
        var action = OnVisitActionParameter.Reduce(_ => { });
        
        action(node.Value);
    }
}