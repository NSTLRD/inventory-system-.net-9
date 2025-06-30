using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Inventory.Api.Application.DTOs;
using Inventory.Api.Application.Queries;
using Inventory.Api.Application.Queries.Handlers;
using Inventory.Api.Common.Interfaces;
using Inventory.Api.Domain.Entities;
using Moq;
using NUnit.Framework;

namespace Inventory.Tests.Application.Handlers
{
    [TestFixture]
    public class GetInventoryStockHandlerTests
    {
        private Mock<IInventoryRepository> _repositoryMock;
        private GetInventoryStockHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IInventoryRepository>();
            _handler = new GetInventoryStockHandler(_repositoryMock.Object);
        }

        [Test]
        public async Task Handle_WhenInventoryItemExists_ReturnsStockDto()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var query = new GetInventoryStock(productId);

            var inventoryItem = new InventoryItem
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Stock = 42
            };

            _repositoryMock
                .Setup(r => r.GetByProductIdAsync(productId))
                .ReturnsAsync(inventoryItem);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.ProductId.Should().Be(productId);
            result.Stock.Should().Be(42);
            _repositoryMock.Verify(r => r.GetByProductIdAsync(productId), Times.Once);
        }

        [Test]
        public async Task Handle_WhenInventoryItemDoesNotExist_ReturnsStockDtoWithZeroStock()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var query = new GetInventoryStock(productId);

            _repositoryMock
                .Setup(r => r.GetByProductIdAsync(productId))
                .ReturnsAsync((InventoryItem)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.ProductId.Should().Be(productId);
            result.Stock.Should().Be(0);
            _repositoryMock.Verify(r => r.GetByProductIdAsync(productId), Times.Once);
        }
    }

    [TestFixture]
    public class GetInventoryHistoryHandlerTests
    {
        private Mock<IInventoryRepository> _repositoryMock;
        private GetInventoryHistoryHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IInventoryRepository>();
            _handler = new GetInventoryHistoryHandler(_repositoryMock.Object);
        }

        [Test]
        public async Task Handle_ReturnsHistoryForProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var query = new GetInventoryHistory(productId);

            var movements = new List<InventoryMovement>
            {
                new InventoryMovement
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    Quantity = 10,
                    Reason = "Initial stock",
                    Timestamp = DateTime.UtcNow.AddDays(-1)
                },
                new InventoryMovement
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    Quantity = -3,
                    Reason = "Sale",
                    Timestamp = DateTime.UtcNow
                }
            };

            _repositoryMock
                .Setup(r => r.GetMovementsByProductIdAsync(productId))
                .ReturnsAsync(movements);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            _repositoryMock.Verify(r => r.GetMovementsByProductIdAsync(productId), Times.Once);
        }

        [Test]
        public async Task Handle_WhenNoMovementsExist_ReturnsEmptyList()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var query = new GetInventoryHistory(productId);

            _repositoryMock
                .Setup(r => r.GetMovementsByProductIdAsync(productId))
                .ReturnsAsync(new List<InventoryMovement>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _repositoryMock.Verify(r => r.GetMovementsByProductIdAsync(productId), Times.Once);
        }
    }
}
