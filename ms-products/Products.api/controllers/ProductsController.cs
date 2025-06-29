using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.Api.Application.Commands;
using Products.Api.Application.Queries;
using Products.Api.Common.DTOs;

namespace Products.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? category = null)
        {
            var query = new GetAllProducts(pageNumber, pageSize, category);
            var result = await _mediator.Send(query);
            
            var dtos = result.Items.Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Category,
                p.Price,
                p.SKU,
                p.History?.Entries ?? new List<(decimal, decimal, DateTime)>()
            )).ToList();
            
            return Ok(dtos);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetByCategory(string category)
        {
            // Usar el constructor adecuado
            var query = new GetProductsByCategory(category);
            var products = await _mediator.Send(query);
            
            var dtos = products.Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Category,
                p.Price,
                p.SKU,
                p.History?.Entries ?? new List<(decimal, decimal, DateTime)>()
            )).ToList();
            
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> Get(Guid id)
        {
            var product = await _mediator.Send(new GetProductById(id));
            if (product == null)
            {
                return NotFound();
            }
            
            var dto = new ProductDto(
                product.Id,
                product.Name,
                product.Description,
                product.Category,
                product.Price,
                product.SKU,
                product.History?.Entries ?? new List<(decimal, decimal, DateTime)>()
            );
            
            return Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateProductDto productDto)
        {
            var command = new UpdateProduct
            {
                Id = id,
                Name = productDto.Name,
                Description = productDto.Description,
                Category = productDto.Category,
                Price = productDto.Price,
                Sku = productDto.Sku
            };

            await _mediator.Send(command);
            return NoContent();
        }
        
        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateProduct command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id }, id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteProduct(id));  // Usar el constructor con parámetro en lugar de inicializar propiedades
            return NoContent();
        }
    }
}
