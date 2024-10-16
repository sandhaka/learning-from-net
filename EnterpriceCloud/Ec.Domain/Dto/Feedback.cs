namespace Ec.Domain.Dto;

public sealed class Feedback
{
    public string ErrorMessage { get; private set; } = string.Empty;
    public required bool Success { get; init; }

    public static Feedback Successful() => new() { Success = true };
    public static Feedback Failure(string message) => new() { Success = false, ErrorMessage = message };
}