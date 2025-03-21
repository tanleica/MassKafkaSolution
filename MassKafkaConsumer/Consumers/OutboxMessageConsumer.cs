using System.Text;
using System.Text.Json;
using MassTransit;

namespace MassKafkaConsumer.Consumers
{
    public class OutboxMessageConsumer : IConsumer<OutboxMessage>
    {
        private static readonly HttpClient httpClient = new();

        public async Task Consume(ConsumeContext<OutboxMessage> context)
        {
            var message = context.Message;
            Console.WriteLine($" [✔] Received Outbox Message: {message.Message}");

            var url = "https://alpha.histaff.vn/api/WebPush/SendSimpleWebPushNotification";
            var payload = new { message.UserId, message.Message };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($" [✔] Push notification sent to {message.UserId}");
            }
            else
            {
                Console.WriteLine($" [!] Failed to send push notification: {response.StatusCode}");
            }
        }
    }
}
