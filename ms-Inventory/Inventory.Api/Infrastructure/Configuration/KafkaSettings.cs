namespace Inventory.Api.Infrastructure.Configuration
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string TopicPrefix { get; set; } = string.Empty;
        public string ConsumerTopic { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
    }
}