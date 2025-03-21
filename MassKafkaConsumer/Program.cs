using Confluent.Kafka;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using MassKafkaConsumer.Consumers;

namespace MassKafkaConsumer
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine(" [*] MassKafkaConsumer is starting...");

            var services = new ServiceCollection();
            services.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<OutboxMessageConsumer>(); // ✅ Register Kafka Consumer

                cfg.UsingInMemory((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });

                cfg.AddRider(rider =>
                {
                    rider.AddConsumer<OutboxMessageConsumer>(); // ✅ Add Consumer to Rider

                    var clientConfig = new ClientConfig
                    {
                        BootstrapServers = "159.223.59.17:9092" // ✅ Kafka Server Address
                    };

                    rider.UsingKafka(clientConfig, (context, k) =>
                    {
                        k.TopicEndpoint<OutboxMessage>("outbox-topic", "consumer-group-1", e =>
                        {
                            e.ConfigureConsumer<OutboxMessageConsumer>(context);
                        });
                    });
                });
            });

            var serviceProvider = services.BuildServiceProvider();
            var busControl = serviceProvider.GetRequiredService<IBusControl>();

            await busControl.StartAsync();
            Console.WriteLine(" [✔] MassKafkaConsumer is running...");
            await Task.Delay(-1); // Keep the app running

        }
    }
}
