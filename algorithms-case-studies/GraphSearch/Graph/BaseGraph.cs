using GraphSearch.Graph.NodeCollections;
using Monads.Optional;

namespace GraphSearch.Graph;

public delegate void NodeAction<in T>(T node);

internal abstract class BaseGraph<T> : IGraph<T> where T : class
{
    protected IReadOnlyNodeCollection<T> NodesCollection;
    
    public ISet<T> NodeValues => NodesCollection.Nodes.Select(n => n.Value).ToHashSet();
    public Option<NodeAction<T>> ActionParameter { get; set; } = Option<NodeAction<T>>.None();

    public void Dfs(T start)
    {
        if (!NodesCollection.Contains(start)) return;
        
        var node = NodesCollection[start];
        var visited = new HashSet<Node<T>>() { node };
        var queue = new Stack<Node<T>>([node]);

        while (queue.Count > 0)
        {
            node = queue.Pop();
            
            // Perform requested action on node
            ActionExecute(node);
            
            var childrenLength = node.Neighbors.Length;
            
            for (var i = 0; i < childrenLength; i++)
            {
                var child = node.Neighbors.Span[i];
                if (!visited.Add(child)) 
                    continue;
                
                queue.Push(child);
            }
        }
    }

    protected virtual void ActionExecute(Node<T> node)
    {
        var action = ActionParameter.Reduce(_ => { });
        
        action(node.Value);
    }
}