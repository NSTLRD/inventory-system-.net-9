using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Inventory.Api.Application.Commands;
using Inventory.Api.Application.Queries;
using Inventory.Api.Application.DTOs;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    /// <summary>
    /// API para gestionar inventario de productos.
    /// Permite consultas para todos los usuarios autenticados, pero solo administradores pueden realizar ajustes.
    /// </summary>
    public class InventoryController : ControllerBase
    {
        private readonly IMediator _mediator; 
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(
            IMediator mediator,
            ILogger<InventoryController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>Adjust stock (increase or decrease)</summary>
        [HttpPost("adjust")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Adjust([FromBody] AdjustInventoryDto dto)
        {
            _logger.LogInformation("Usuario con rol Admin ajustando inventario: ProductId {ProductId}, Cantidad {Quantity}, Razón {Reason}", 
                dto.ProductId, dto.Quantity, dto.Reason);
            
            var cmd = new AdjustInventory(dto.ProductId, dto.Quantity, dto.Reason);
            await _mediator.Send(cmd);
            
            _logger.LogInformation("Inventario ajustado correctamente: ProductId {ProductId}", dto.ProductId);
            return NoContent();
        }
        /// <summary>Get current stock</summary>
        [HttpGet("{productId}/stock")]
        public async Task<InventoryStockDto> GetStock(Guid productId)
        {
            _logger.LogInformation("Consultando stock para ProductId {ProductId}", productId);
            var result = await _mediator.Send(new GetInventoryStock(productId));
            return result;
        }

        /// <summary>Get movement history</summary>
        [HttpGet("{productId}/history")]
        public async Task<IEnumerable<InventoryHistoryDto>> GetHistory(Guid productId)
        {
            _logger.LogInformation("Consultando historial de movimientos para ProductId {ProductId}", productId);
            var result = await _mediator.Send(new GetInventoryHistory(productId));
            return result;
        }

        /// <summary>Get all inventory items</summary>
        [HttpGet]
        public async Task<IActionResult> GetAllInventory()
        {
            _logger.LogInformation("Consultando todos los elementos de inventario");
            var result = await _mediator.Send(new GetAllInventory());
            return Ok(result);
        }
    }
}
