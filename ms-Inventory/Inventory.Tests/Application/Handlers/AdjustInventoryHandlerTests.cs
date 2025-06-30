using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Inventory.Api.Application.Commands;
using Inventory.Api.Application.Commands.Handlers;
using Inventory.Api.Common.Interfaces;
using Inventory.Api.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Inventory.Tests.Application.Handlers
{
    [TestFixture]
    public class AdjustInventoryHandlerTests
    {
        private Mock<IInventoryRepository> _repositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger<AdjustInventoryHandler>> _loggerMock;
        private AdjustInventoryHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IInventoryRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<AdjustInventoryHandler>>();
            _handler = new AdjustInventoryHandler(
                _repositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object);
        }

        [Test]
        public async Task Handle_WhenInventoryItemExists_AdjustsInventoryAndCreatesMovement()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new AdjustInventory(productId, 5, "Test adjustment");

            var inventoryItem = new InventoryItem
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Stock = 10
            };

            _repositoryMock
                .Setup(r => r.GetByProductIdAsync(productId))
                .ReturnsAsync(inventoryItem);

            _repositoryMock
                .Setup(r => r.UpdateStockAndCreateMovement(It.IsAny<InventoryItem>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(
                r => r.GetByProductIdAsync(productId),
                Times.Once);

            _repositoryMock.Verify(
                r => r.UpdateStockAndCreateMovement(
                    It.Is<InventoryItem>(i => i.Id == inventoryItem.Id),
                    It.Is<int>(q => q == 5),
                    It.Is<string>(s => s == "Test adjustment")),
                Times.Once);

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task Handle_WhenInventoryItemDoesNotExist_CreatesNewItemAndAdjusts()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var command = new AdjustInventory(productId, 5, "Initial stock");

            _repositoryMock
                .Setup(r => r.GetByProductIdAsync(productId))
                .ReturnsAsync((InventoryItem)null);

            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<InventoryItem>()))
                .Returns(Task.CompletedTask);

            _repositoryMock
                .Setup(r => r.UpdateStockAndCreateMovement(It.IsAny<InventoryItem>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(
                r => r.GetByProductIdAsync(productId),
                Times.Once);

            _repositoryMock.Verify(
                r => r.AddAsync(It.Is<InventoryItem>(i => i.ProductId == productId && i.Stock == 0)),
                Times.Once);

            _repositoryMock.Verify(
                r => r.UpdateStockAndCreateMovement(
                    It.IsAny<InventoryItem>(),
                    It.Is<int>(q => q == 5),
                    It.Is<string>(s => s == "Initial stock")),
                Times.Once);

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
