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
using Products.Api.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Products.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrencyConversionService _currencyService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IMediator mediator, 
            ICurrencyConversionService currencyService,
            ILogger<ProductsController> logger)
        {
            _mediator = mediator;
            _currencyService = currencyService;
            _logger = logger;
        }

        

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] string? category = null,
            [FromQuery] string? currency = null)
        {
            var query = new GetAllProducts(pageNumber, pageSize, category);
            var result = await _mediator.Send(query);
            
            var dtos = new List<ProductDto>();
            
            foreach (var p in result.Items)
            {
                var dto = new ProductDto(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Category,
                    p.Price,
                    p.SKU,
                    p.History?.Entries ?? new List<(decimal, decimal, DateTime)>()
                );
                
                if (!string.IsNullOrEmpty(currency))
                {
                    await ConvertProductPriceAsync(dto, currency);
                }
                
                dtos.Add(dto);
            }
            
            return Ok(dtos);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetByCategory(
            string category,
            [FromQuery] string? currency = null)
        {
            
            var query = new GetProductsByCategory(category);
            var products = await _mediator.Send(query);
            
            var dtos = new List<ProductDto>();
            
            foreach (var p in products)
            {
                var dto = new ProductDto(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Category,
                    p.Price,
                    p.SKU,
                    p.History?.Entries ?? new List<(decimal, decimal, DateTime)>()
                );
                
                if (!string.IsNullOrEmpty(currency))
                {
                    await ConvertProductPriceAsync(dto, currency);
                }
                
                dtos.Add(dto);
            }
            
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> Get(Guid id, [FromQuery] string? currency = null)
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
            
            if (!string.IsNullOrEmpty(currency))
            {
                await ConvertProductPriceAsync(dto, currency);
            }
            
            return Ok(dto);
        }
        
        [HttpGet("{id}/price-history")]
        public async Task<ActionResult<PriceHistoryDto>> GetPriceHistory(Guid id, [FromQuery] string? currency = null)
        {
            var product = await _mediator.Send(new GetProductById(id));
            if (product == null)
            {
                return NotFound();
            }
            
            var priceChanges = new List<PriceChangeDto>();
            
            if (product.History != null && product.History.Entries.Any())
            {
                foreach (var entry in product.History.Entries)
                {
                    var priceChange = new PriceChangeDto(entry.OldPrice, entry.NewPrice, entry.At);
                    
                    
                    if (!string.IsNullOrEmpty(currency))
                    {
                        try 
                        {
                            bool isSupported = await _currencyService.IsCurrencySupportedAsync(currency);
                            if (!isSupported)
                            {
                                return BadRequest($"La moneda '{currency}' no es soportada");
                            }
                            
                            decimal rate = await _currencyService.GetExchangeRateAsync(currency);
                            decimal oldPriceConverted = await _currencyService.ConvertFromUsdAsync(entry.OldPrice, currency);
                            decimal newPriceConverted = await _currencyService.ConvertFromUsdAsync(entry.NewPrice, currency);
                            
                            priceChange.OldPriceConverted = oldPriceConverted;
                            priceChange.NewPriceConverted = newPriceConverted;
                            priceChange.Currency = currency;
                            priceChange.ExchangeRate = rate;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error al convertir precios a {Currency}", currency);
                        }
                    }
                    
                    priceChanges.Add(priceChange);
                }
            }
            
            var priceHistoryDto = new PriceHistoryDto(
                product.Id,
                product.Name,
                priceChanges
            );
            
            return Ok(priceHistoryDto);
        }

       

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateProduct command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id }, id);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteProduct(id));
            return NoContent();
        }
        
        private async Task ConvertProductPriceAsync(ProductDto dto, string currency)
        {
            try
            {
                bool isSupported = await _currencyService.IsCurrencySupportedAsync(currency);
                if (!isSupported)
                {
                    _logger.LogWarning("Moneda no soportada: {Currency}", currency);
                    return;
                }
                
                decimal rate = await _currencyService.GetExchangeRateAsync(currency);
                decimal convertedPrice = await _currencyService.ConvertFromUsdAsync(dto.Price, currency);
                
                dto.ConvertedPrice = convertedPrice;
                dto.Currency = currency;
                dto.ExchangeRate = rate;
                
                _logger.LogInformation(
                    "Precio convertido: {OriginalPrice} USD → {ConvertedPrice} {Currency} (tasa: {Rate})",
                    dto.Price, convertedPrice, currency, rate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al convertir precio a {Currency}", currency);
            }
        }
    }
}
