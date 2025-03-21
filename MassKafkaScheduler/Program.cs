using System;
using System.Text.Json;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using MassKafkaScheduler.Models;

namespace MassKafkaScheduler
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine(" [*] MassKafkaScheduler Started...");

            var services = new ServiceCollection();

            services.AddMassTransit(x =>
            {
                x.UsingInMemory((_, __) => { }); // Required for Kafka Rider to attach

                x.AddRider(rider =>
                {
                    rider.AddProducer<OutboxMessage>("outbox-topic");

                    rider.UsingKafka((context, kafka) =>
                    {
                        kafka.Host("159.223.59.17:9092");
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            // ✅ Start MassTransit (Bus + Rider)
            var bus = provider.GetRequiredService<IBusControl>();
            await bus.StartAsync();

            try
            {
                var producer = provider.GetRequiredService<ITopicProducer<OutboxMessage>>();

                while (true)
                {
                    await Task.Delay(TimeSpan.FromMinutes(15)); // Schedule-like behavior

                    var message = new OutboxMessage
                    {
                        UserId = "b2e05ec4-6022-4f35-baea-ceb7fa2ee9dd",
                        Message = $"Kafka says hi at {DateTime.UtcNow:HH:mm:ss}"
                    };

                    await producer.Produce(message);
                    Console.WriteLine($" [✔] Sent: {JsonSerializer.Serialize(message)}");
                }
            }
            finally
            {
                await bus.StopAsync();
            }
        }
    }
}
