/*
 * Liskov principle guarantee that a method will exist on any implementation that derives,
 * directly or indirectly, from the variable type on the left, the variable to which I have
 * assigned a reference to an object. 
 */

using System.Collections;

namespace lsp;

// Best way to ensure the LSP is use it in design phase.
// Create two sets of properties:

public interface IStackable<T> : IEnumerable<T>
{
    int Count {get;}
    void Push(T item, params T[] more);
    T Pop();
}

public interface IStackProper<T> : IStackable<T> { }

// Stack<> inherits from IStackProper implements both sets

public class Stack<T> : IStackProper<T>
{
    private List<T> Items { get; } = new();
    
    public int Count => Items.Count;
    
    public void Push(T item, params T[] more)
    {
        Items.Add(item);
        foreach (var additional in more)
            Items.Add(additional);
    }
    
    public T Pop()
    {
        if (Items.Count == 0)
            throw new InvalidOperationException("Stack is empty");
        
        T result = Items[^1];
        Items.RemoveAt(Items.Count - 1);
        return result;
    }
    
    public IEnumerator<T> GetEnumerator()
    {
        for (int i = Items.Count - 1; i >= 0; i--)
            yield return Items[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

// UniqueStack inherits first set only

public class UniqueStack<T> : IStackable<T>
{
    private Stack<T> Items { get; } = new ();

    public int Count => Items.Count;

    public void Push(T item, params T[] more) => new T[] { item }.Concat(more)
        .Distinct()
        .Except(Items)
        .ToList()
        .ForEach(x => Items.Push(x));

    public T Pop() => Items.Pop();

    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}