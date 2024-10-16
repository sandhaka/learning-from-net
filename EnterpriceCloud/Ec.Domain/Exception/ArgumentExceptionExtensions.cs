#nullable enable
namespace Ec.Domain.Exception;

public static class PreConditionException
{
    public static void ThrowIf(Func<bool>? condition, string? message = null)
    {
        if (condition?.Invoke() ?? false)
            throw new ArgumentException(message);
    }
}