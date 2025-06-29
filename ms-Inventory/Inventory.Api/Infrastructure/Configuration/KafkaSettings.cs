namespace Inventory.Api.Infrastructure.Configuration
{
   public class KafkaSettings
{
    public required string BootstrapServers { get; set; }
    public required string Topic            { get; set; }
    public required string GroupId          { get; set; }
}

}
