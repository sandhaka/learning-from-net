using System.Text.Json;
using System.Text.Json.Serialization;
using EasyNetQ;
using EasyNetQ.DI;
using EasyNetQ.Serialization.SystemTextJson;
using Ec.Application;
using Ec.Application.Handlers;
using Ec.Application.Messages;
using Ec.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
    {
        services.AddSingleton(RabbitHutch.CreateBus("host=localhost", serviceRegister =>
        {
            serviceRegister.Register<ISerializer>(serviceProvider => 
                new SystemTextJsonSerializer(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                }));
        }));
        
        // Adding services
        services.AddSingleton<DemoService>();
        services.AddSingleton<UserInteractionHandler>();
    })
    .ConfigureLogging(logging => { logging.AddConsole(); })
    .Build();

var userInteractionHandler = host.Services.GetRequiredService<UserInteractionHandler>();
await userInteractionHandler.SubscribeToMessagesAsync();

var demoService = host.Services.GetRequiredService<DemoService>();
// await demoService.RunDemoAsync(); TODO
    
await host.RunAsync();