using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Inventory.Api.Application.Commands;
using Inventory.Api.Application.Queries;
using Inventory.Api.Application.DTOs;
using Inventory.Api.Common.DTOs;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IMediator _mediator; // Declarar el campo

        // Añadir constructor que inicialice el mediator
        public InventoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Adjust stock (increase or decrease)</summary>
        [HttpPost("adjust")]
        public async Task<IActionResult> Adjust([FromBody] AdjustInventoryDto dto)
        {
            var cmd = new AdjustInventory(dto.ProductId, dto.Quantity, dto.Reason);
            await _mediator.Send(cmd);
            return NoContent();
        }
        /// <summary>Get current stock</summary>
        [HttpGet("{productId}/stock")]
        public Task<InventoryStockDto> GetStock(Guid productId)
            => _mediator.Send(new GetInventoryStock(productId));

        /// <summary>Get movement history</summary>
        [HttpGet("{productId}/history")]
        public Task<IEnumerable<InventoryHistoryDto>> GetHistory(Guid productId)
            => _mediator.Send(new GetInventoryHistory(productId));

        /// <summary>Get all inventory items</summary>
        [HttpGet]
        public async Task<IActionResult> GetAllInventory()
        {
            var result = await _mediator.Send(new GetAllInventory());
            return Ok(result);
        }
    }
}
