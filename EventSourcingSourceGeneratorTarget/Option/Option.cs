namespace EventSourcingSourceGeneratorTarget.Option;

public interface IOption<out T>
{
    T Reduce();
    bool IsNone();
}

public abstract class Option<T> : IOption<T>
{
    public static implicit operator Option<T>(T value) => new Some<T>(value);
    public static implicit operator Option<T>(None _) => new None<T>();

    public abstract T Reduce();
    public abstract bool IsNone();
    
    public IOption<T> AsOption() => this;
}
