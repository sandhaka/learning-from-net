using System.Text.Json;
using System.Text.Json.Serialization;
using EasyNetQ;
using EasyNetQ.DI;
using EasyNetQ.Serialization.SystemTextJson;
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
        // services.AddSingleton<>();
    })
    .ConfigureLogging(logging => { logging.AddConsole(); })
    .Build();
    
await host.RunAsync();