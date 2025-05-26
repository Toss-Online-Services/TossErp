using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Domain.Entities;
using Catalog.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Catalog.UnitTests.Repositories
{
    public class ProductRepositoryTests : TestBase
    {
        private readonly ProductRepository _repository;
        private readonly List<Product> _testProducts;

        public ProductRepositoryTests()
        {
            _testProducts = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Test Product 1",
                    Description = "Description 1",
                    Price = 100,
                    IsPublished = true,
                    IsDeleted = false,
                    CreatedOnUtc = DateTime.UtcNow
                },
                new Product
                {
                    Id = 2,
                    Name = "Test Product 2",
                    Description = "Description 2",
                    Price = 200,
                    IsPublished = true,
                    IsDeleted = false,
                    CreatedOnUtc = DateTime.UtcNow
                }
            };

            SetupMockDbSet(_testProducts.AsQueryable());
            _mockContext.Setup(c => c.Products).Returns(_mockDbSet.Object);
            _repository = new ProductRepository(_mockContext.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingProduct_ReturnsProduct()
        {
            // Arrange
            var productId = 1;

            // Act
            var result = await _repository.GetByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingProduct_ReturnsNull()
        {
            // Arrange
            var productId = 999;

            // Act
            var result = await _repository.GetByIdAsync(productId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllProducts()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_testProducts.Count, result.Count());
        }

        [Fact]
        public async Task GetByCategoryIdAsync_ReturnsProductsInCategory()
        {
            // Arrange
            var categoryId = 1;
            var category = new Category { Id = categoryId };
            _testProducts[0].Categories = new List<Category> { category };

            // Act
            var result = await _repository.GetByCategoryIdAsync(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(_testProducts[0].Id, result.First().Id);
        }

        [Fact]
        public async Task AddAsync_AddsNewProduct()
        {
            // Arrange
            var newProduct = new Product
            {
                Name = "New Product",
                Description = "New Description",
                Price = 300,
                IsPublished = true
            };

            // Act
            await _repository.AddAsync(newProduct);

            // Assert
            _mockContext.Verify(c => c.Products.AddAsync(It.IsAny<Product>(), default), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesExistingProduct()
        {
            // Arrange
            var product = _testProducts[0];
            product.Name = "Updated Name";

            // Act
            await _repository.UpdateAsync(product);

            // Assert
            _mockContext.Verify(c => c.Products.Update(It.IsAny<Product>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ExistingProduct_DeletesProduct()
        {
            // Arrange
            var productId = 1;

            // Act
            await _repository.DeleteAsync(productId);

            // Assert
            _mockContext.Verify(c => c.Products.Remove(It.IsAny<Product>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_NonExistingProduct_DoesNothing()
        {
            // Arrange
            var productId = 999;

            // Act
            await _repository.DeleteAsync(productId);

            // Assert
            _mockContext.Verify(c => c.Products.Remove(It.IsAny<Product>()), Times.Never);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Never);
        }
    }
} 
