using EasyNetQ;
using Ec.Application.Messages;

namespace Ec.Application.Handlers;

public class UserInteractionHandler
{
    private readonly IBus _bus;
    private const string QueueName = "user-interactions-queue";
    private readonly string _instanceId = Guid.NewGuid().ToString();

    public UserInteractionHandler(IBus bus)
    {
        _bus = bus;
    }

    public async Task SubscribeToMessagesAsync()
    {
        await _bus.PubSub.SubscribeAsync<UserInteractionMessage>(
            $"{QueueName}_{_instanceId}",
            HandleMessageAsync,
            c => c.AsExclusive().WithDurable(false)
        );
    }
    
    public async Task HandleMessageAsync(UserInteractionMessage message, CancellationToken cancellationToken)
    {
        // Perform necessary actions based on the message
        await Task.CompletedTask;
    }
}