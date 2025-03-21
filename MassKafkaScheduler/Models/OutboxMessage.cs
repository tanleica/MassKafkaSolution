namespace MassKafkaScheduler.Models
{
    public class OutboxMessage
    {
        public long Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool Processed { get; set; }
    }
}
