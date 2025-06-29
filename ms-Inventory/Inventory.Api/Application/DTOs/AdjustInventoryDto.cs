// Inventory.Api/Application/DTOs/AdjustInventoryDto.cs
namespace Inventory.Api.Application.DTOs
{
    public class AdjustInventoryDto
    {
        public Guid   ProductId { get; set; }
        public int    Quantity  { get; set; }
        public string Reason    { get; set; } = default!;
    }
}
