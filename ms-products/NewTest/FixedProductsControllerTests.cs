using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Products.Interfaces;

namespace NewTest
{
    // Mock classes for testing
    public class GetAllProductsQuery : IRequest<IPagedResponse<IProductDto>> 
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetProductByIdQuery : IRequest<IProductDto>
    {
        public int Id { get; set; }
    }

    public class ProductDto : IProductDto
    {
        public int Id { get; }
        public string Name { get; }
        public string Description { get; }
        public decimal Price { get; }
        public int Stock { get; }
        public string Currency { get; }

        public ProductDto(int id, string name, string description, decimal price, int stock, string currency = "USD")
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            Stock = stock;
            Currency = currency;
        }
    }

    public class PagedResponse<T> : IPagedResponse<T>
    {
        public List<T> Data { get; }
        public int TotalCount { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < (int)Math.Ceiling(TotalCount / (double)PageSize);
        
        public PagedResponse(List<T> data, int totalCount, int pageNumber, int pageSize)
        {
            Data = data;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    // Mock controller for testing
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

        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            var query = new GetAllProductsQuery { PageNumber = pageNumber, PageSize = pageSize };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetProductByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFound();
                
            return Ok(result);
        }
    }
    
    [TestFixture]
    public class ProductsControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private Mock<ICurrencyConversionService> _currencyServiceMock;
        private Mock<ILogger<ProductsController>> _loggerMock;
        private ProductsController _controller;

        [SetUp]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
            _currencyServiceMock = new Mock<ICurrencyConversionService>();
            _loggerMock = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(
                _mediatorMock.Object,
                _currencyServiceMock.Object,
                _loggerMock.Object);
        }

        [Test]
        public async Task GetAll_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            var products = new List<ProductDto>
            {
                new ProductDto(1, "Product 1", "Description 1", 10.99m, 100),
                new ProductDto(2, "Product 2", "Description 2", 20.99m, 50)
            };
            var paged = new PagedResponse<IProductDto>(products.ConvertAll(p => (IProductDto)p), 2, 1, 10);
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paged);

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.Value.Should().BeSameAs(paged);
        }

        [Test]
        public async Task GetById_WithExistingId_ReturnsOkResult()
        {
            // Arrange
            var product = new ProductDto(1, "Product 1", "Description 1", 10.99m, 100);
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.Value.Should().BeSameAs(product);
        }

        [Test]
        public async Task GetById_WithNonExistingId_ReturnsNotFound()
        {
            // Arrange
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IProductDto)null);

            // Act
            var result = await _controller.GetById(999);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
