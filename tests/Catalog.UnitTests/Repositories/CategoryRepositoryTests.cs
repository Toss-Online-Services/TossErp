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
    public class CategoryRepositoryTests : TestBase
    {
        private readonly CategoryRepository _repository;
        private readonly List<Category> _testCategories;

        public CategoryRepositoryTests()
        {
            _testCategories = new List<Category>
            {
                new Category
                {
                    Id = 1,
                    Name = "Test Category 1",
                    Description = "Description 1",
                    IsPublished = true,
                    IsDeleted = false,
                    CreatedOnUtc = DateTime.UtcNow
                },
                new Category
                {
                    Id = 2,
                    Name = "Test Category 2",
                    Description = "Description 2",
                    IsPublished = true,
                    IsDeleted = false,
                    CreatedOnUtc = DateTime.UtcNow
                }
            };

            SetupMockDbSet(_testCategories.AsQueryable());
            _mockContext.Setup(c => c.Categories).Returns(_mockDbSet.Object);
            _repository = new CategoryRepository(_mockContext.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingCategory_ReturnsCategory()
        {
            // Arrange
            var categoryId = 1;

            // Act
            var result = await _repository.GetByIdAsync(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryId, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingCategory_ReturnsNull()
        {
            // Arrange
            var categoryId = 999;

            // Act
            var result = await _repository.GetByIdAsync(categoryId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllCategories()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_testCategories.Count, result.Count());
        }

        [Fact]
        public async Task GetPublishedCategoriesAsync_ReturnsOnlyPublishedCategories()
        {
            // Arrange
            _testCategories[1].IsPublished = false;

            // Act
            var result = await _repository.GetPublishedCategoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.True(result.First().IsPublished);
        }

        [Fact]
        public async Task GetCategoriesForHomePageAsync_ReturnsPublishedCategoriesOrderedByName()
        {
            // Arrange
            _testCategories[0].Name = "Z Category";
            _testCategories[1].Name = "A Category";

            // Act
            var result = await _repository.GetCategoriesForHomePageAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("A Category", result.First().Name);
        }

        [Fact]
        public async Task AddAsync_AddsNewCategory()
        {
            // Arrange
            var newCategory = new Category
            {
                Name = "New Category",
                Description = "New Description",
                IsPublished = true
            };

            // Act
            await _repository.AddAsync(newCategory);

            // Assert
            _mockContext.Verify(c => c.Categories.AddAsync(It.IsAny<Category>(), default), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesExistingCategory()
        {
            // Arrange
            var category = _testCategories[0];
            category.Name = "Updated Name";

            // Act
            await _repository.UpdateAsync(category);

            // Assert
            _mockContext.Verify(c => c.Categories.Update(It.IsAny<Category>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ExistingCategory_DeletesCategory()
        {
            // Arrange
            var categoryId = 1;

            // Act
            await _repository.DeleteAsync(categoryId);

            // Assert
            _mockContext.Verify(c => c.Categories.Remove(It.IsAny<Category>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_NonExistingCategory_DoesNothing()
        {
            // Arrange
            var categoryId = 999;

            // Act
            await _repository.DeleteAsync(categoryId);

            // Assert
            _mockContext.Verify(c => c.Categories.Remove(It.IsAny<Category>()), Times.Never);
            _mockContext.Verify(c => c.SaveChangesAsync(default), Times.Never);
        }
    }
} 
