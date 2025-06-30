using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Inventory.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Inventory.Tests.Controllers
{
    // Mock DTOs
    public class InventoryStockDto : IInventoryStockDto
    {
        public Guid ProductId { get; }
        public int Stock { get; }

        public InventoryStockDto(Guid productId, int stock)
        {
            ProductId = productId;
            Stock = stock;
        }
    }

    public class InventoryHistoryDto : IInventoryHistoryDto
    {
        public Guid Id { get; }
        public Guid InventoryItemId { get; }
        public int QuantityChanged { get; }
        public DateTime Timestamp { get; }

        public InventoryHistoryDto(Guid id, Guid inventoryItemId, int quantityChanged, DateTime timestamp)
        {
            Id = id;
            InventoryItemId = inventoryItemId;
            QuantityChanged = quantityChanged;
            Timestamp = timestamp;
        }
    }

    public class AdjustInventoryDto : IAdjustInventoryDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; } = "";
    }

    // Mock Commands and Queries
    public class AdjustInventory : IRequest, IAdjustInventory
    {
        public Guid ProductId { get; }
        public int Quantity { get; }
        public string Reason { get; }

        public AdjustInventory(Guid productId, int quantity, string reason)
        {
            ProductId = productId;
            Quantity = quantity;
            Reason = reason;
        }
    }

    public class GetInventoryStock : IRequest<IInventoryStockDto>, IGetInventoryStock
    {
        public Guid ProductId { get; }

        public GetInventoryStock(Guid productId)
        {
            ProductId = productId;
        }
    }

    public class GetInventoryHistory : IRequest<IEnumerable<IInventoryHistoryDto>>, IGetInventoryHistory
    {
        public Guid ProductId { get; }

        public GetInventoryHistory(Guid productId)
        {
            ProductId = productId;
        }
    }

    public class GetAllInventory : IRequest<IEnumerable<IInventoryStockDto>>, IGetAllInventory
    {
    }

    // Mock Controller
    public class InventoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IMediator mediator, ILogger<InventoryController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<IActionResult> Adjust(IAdjustInventoryDto dto)
        {
            _logger.LogInformation("Ajustando inventario para producto {ProductId}", dto.ProductId);
            
            await _mediator.Send(new AdjustInventory(dto.ProductId, dto.Quantity, dto.Reason));
            
            _logger.LogInformation("Inventario ajustado correctamente");
            return NoContent();
        }

        public async Task<IInventoryStockDto> GetStock(Guid productId)
        {
            _logger.LogInformation("Consultando stock para producto {ProductId}", productId);
            return await _mediator.Send(new GetInventoryStock(productId));
        }

        public async Task<IEnumerable<IInventoryHistoryDto>> GetHistory(Guid productId)
        {
            _logger.LogInformation("Consultando historial para producto {ProductId}", productId);
            return await _mediator.Send(new GetInventoryHistory(productId));
        }

        public async Task<IActionResult> GetAllInventory()
        {
            _logger.LogInformation("Consultando todos los elementos de inventario");
            var items = await _mediator.Send(new GetAllInventory());
            return Ok(items);
        }
    }

    [TestFixture]
    public class InventoryControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private Mock<ILogger<InventoryController>> _loggerMock;
        private InventoryController _controller;

        [SetUp]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<InventoryController>>();
            _controller = new InventoryController(_mediatorMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Adjust_CallsMediatorWithCorrectCommand_ReturnsNoContent()
        {
            // Arrange
            var dto = new AdjustInventoryDto
            {
                ProductId = Guid.NewGuid(),
                Quantity = 10,
                Reason = "Manual adjustment"
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AdjustInventory>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Adjust(dto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<AdjustInventory>(
                        cmd => cmd.ProductId == dto.ProductId && 
                               cmd.Quantity == dto.Quantity && 
                               cmd.Reason == dto.Reason), 
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task GetStock_CallsMediatorWithCorrectQuery_ReturnsStock()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var stockDto = new InventoryStockDto(productId, 42);

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetInventoryStock>(q => q.ProductId == productId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(stockDto);

            // Act
            var result = await _controller.GetStock(productId);

            // Assert
            result.Should().NotBeNull();
            result.ProductId.Should().Be(productId);
            result.Stock.Should().Be(42);
            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<GetInventoryStock>(q => q.ProductId == productId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task GetHistory_CallsMediatorWithCorrectQuery_ReturnsHistory()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var inventoryItemId = Guid.NewGuid();
            
            var history = new List<InventoryHistoryDto>
            {
                new InventoryHistoryDto(
                    Guid.NewGuid(),
                    inventoryItemId,
                    10,
                    DateTime.UtcNow.AddDays(-1)
                ),
                new InventoryHistoryDto(
                    Guid.NewGuid(),
                    inventoryItemId,
                    -3,
                    DateTime.UtcNow
                )
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetInventoryHistory>(q => q.ProductId == productId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(history);

            // Act
            var result = await _controller.GetHistory(productId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<GetInventoryHistory>(q => q.ProductId == productId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task GetAllInventory_CallsMediator_ReturnsOkWithInventory()
        {
            // Arrange
            var inventoryItems = new List<InventoryStockDto>
            {
                new InventoryStockDto(Guid.NewGuid(), 10),
                new InventoryStockDto(Guid.NewGuid(), 20)
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllInventory>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(inventoryItems);

            // Act
            var result = await _controller.GetAllInventory();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedItems = okResult.Value.Should().BeAssignableTo<IEnumerable<InventoryStockDto>>().Subject;
            returnedItems.Should().HaveCount(2);
            _mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<GetAllInventory>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Test]
        public async Task Adjust_WithNegativeQuantity_StillProcessesCorrectly()
        {
            // Arrange
            var dto = new AdjustInventoryDto
            {
                ProductId = Guid.NewGuid(),
                Quantity = -5, // Negative quantity to simulate removal
                Reason = "Stock reduction"
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AdjustInventory>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Adjust(dto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<AdjustInventory>(
                        cmd => cmd.ProductId == dto.ProductId && 
                               cmd.Quantity == dto.Quantity && 
                               cmd.Reason == dto.Reason), 
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Test]
        public async Task GetStock_WithZeroStock_ReturnsZeroStock()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var stockDto = new InventoryStockDto(productId, 0);

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetInventoryStock>(q => q.ProductId == productId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(stockDto);

            // Act
            var result = await _controller.GetStock(productId);

            // Assert
            result.Should().NotBeNull();
            result.ProductId.Should().Be(productId);
            result.Stock.Should().Be(0);
        }
        
        [Test]
        public async Task GetHistory_WithEmptyHistory_ReturnsEmptyList()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var emptyHistory = new List<InventoryHistoryDto>();

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetInventoryHistory>(q => q.ProductId == productId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyHistory);

            // Act
            var result = await _controller.GetHistory(productId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mediatorMock.Verify(
                m => m.Send(
                    It.Is<GetInventoryHistory>(q => q.ProductId == productId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task GetAllInventory_WithEmptyInventory_ReturnsEmptyList()
        {
            // Arrange
            var emptyInventory = new List<InventoryStockDto>();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllInventory>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyInventory);

            // Act
            var result = await _controller.GetAllInventory();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedItems = okResult.Value.Should().BeAssignableTo<IEnumerable<InventoryStockDto>>().Subject;
            returnedItems.Should().BeEmpty();
        }

        [Test]
        public async Task Adjust_LogsCorrectInformation()
        {
            // Arrange
            var dto = new AdjustInventoryDto
            {
                ProductId = Guid.NewGuid(),
                Quantity = 15,
                Reason = "Testing logging"
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AdjustInventory>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Adjust(dto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            
            // Verificar que se registraron los logs apropiados
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("ajustando inventario")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
                
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("ajustado correctamente")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetStock_LogsCorrectInformation()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var stockDto = new InventoryStockDto(productId, 25);

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetInventoryStock>(q => q.ProductId == productId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(stockDto);

            // Act
            var result = await _controller.GetStock(productId);

            // Assert
            result.Should().NotBeNull();
            
            // Verificar que se registró el log apropiado
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Consultando stock")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetHistory_LogsCorrectInformation()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var history = new List<InventoryHistoryDto>();

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetInventoryHistory>(q => q.ProductId == productId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(history);

            // Act
            var result = await _controller.GetHistory(productId);

            // Assert
            result.Should().NotBeNull();
            
            // Verificar que se registró el log apropiado
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Consultando historial")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetAllInventory_LogsCorrectInformation()
        {
            // Arrange
            var inventoryItems = new List<InventoryStockDto>();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllInventory>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(inventoryItems);

            // Act
            var result = await _controller.GetAllInventory();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            
            // Verificar que se registró el log apropiado
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Consultando todos los elementos")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
