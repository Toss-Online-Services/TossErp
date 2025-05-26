using Microsoft.EntityFrameworkCore;
using Moq;
using Catalog.Infrastructure.Data;

namespace Catalog.UnitTests
{
    public abstract class TestBase
    {
        protected readonly Mock<CatalogContext> _mockContext;
        protected readonly Mock<DbSet<T>> _mockDbSet;

        protected TestBase()
        {
            _mockContext = new Mock<CatalogContext>();
            _mockDbSet = new Mock<DbSet<T>>();
        }

        protected void SetupMockDbSet<T>(IQueryable<T> data) where T : class
        {
            _mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }
    }
} 
