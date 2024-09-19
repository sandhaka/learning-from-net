namespace Ec.Domain.Models.Abstract;

public interface ISourceEvent
{
    DateTime Timestamp { get; }
}