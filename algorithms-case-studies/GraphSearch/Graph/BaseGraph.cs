using GraphSearch.Graph.NodeCollections;
using Monads.Optional;

namespace GraphSearch.Graph;

internal abstract class BaseGraph<T> : IGraph<T> where T : class
{
    protected IReadOnlyNodeCollection<T> NodesCollection;
    
    public ISet<T> NodeValues => NodesCollection.Nodes.Select(n => n.Value).ToHashSet();
    
    public void Dfs(T start, Option<Action<T>> action)
    {
        if (!NodesCollection.Contains(start)) return;
        
        var node = NodesCollection[start];
        var visited = new HashSet<Node<T>>() { node };
        var queue = new Stack<Node<T>>([node]);

        while (queue.Count > 0)
        {
            node = queue.Pop();
            
            // Perform requested action on node
            action.Reduce(_ => { }).Invoke(node.Value);
            
            var childrenLength = node.Children.Length;
            
            for (var i = 0; i < childrenLength; i++)
            {
                var child = node.Children.Span[i];
                if (!visited.Add(child)) 
                    continue;
                
                queue.Push(child);
            }
        }
    }
}