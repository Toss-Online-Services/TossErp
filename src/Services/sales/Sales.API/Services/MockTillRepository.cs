using TossErp.Sales.Application.Common.Interfaces;
using TossErp.Sales.Domain.Common;
using TossErp.Sales.Domain.Entities;
using TossErp.Sales.Domain.Enums;
using TossErp.Sales.Domain.ValueObjects;

namespace TossErp.Sales.API.Services;

/// <summary>
/// Mock implementation of ITillRepository for MVP
/// </summary>
public class MockTillRepository : ITillRepository
{
    private readonly List<Till> _tills = new();
    private readonly ILogger<MockTillRepository> _logger;

    public MockTillRepository(ILogger<MockTillRepository> logger)
    {
        _logger = logger;
        
        // Add some sample tills for testing
        var till1 = new Till(
            Guid.NewGuid(),
            "Main Till",
            "TILL001",
            "Main Store",
            "MT",
            "default-tenant");
        
        var till2 = new Till(
            Guid.NewGuid(),
            "Secondary Till",
            "TILL002",
            "Secondary Store",
            "ST",
            "default-tenant");
        
        _tills.Add(till1);
        _tills.Add(till2);
    }

    public Task<Till?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var till = _tills.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(till);
    }

    public Task<IEnumerable<Till>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_tills.AsEnumerable());
    }

    public Task<Till> AddAsync(Till entity, CancellationToken cancellationToken = default)
    {
        _tills.Add(entity);
        _logger.LogInformation("Added till {TillId} to mock repository", entity.Id);
        return Task.FromResult(entity);
    }

    public Task<Till> UpdateAsync(Till entity, CancellationToken cancellationToken = default)
    {
        var existingTill = _tills.FirstOrDefault(t => t.Id == entity.Id);
        if (existingTill != null)
        {
            var index = _tills.IndexOf(existingTill);
            _tills[index] = entity;
            _logger.LogInformation("Updated till {TillId} in mock repository", entity.Id);
        }
        return Task.FromResult(entity);
    }

    public Task DeleteAsync(Till entity, CancellationToken cancellationToken = default)
    {
        _tills.RemoveAll(t => t.Id == entity.Id);
        _logger.LogInformation("Deleted till {TillId} from mock repository", entity.Id);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _tills.RemoveAll(t => t.Id == id);
        _logger.LogInformation("Deleted till {TillId} from mock repository", id);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_tills.Any(t => t.Id == id));
    }

    public Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult((long)_tills.Count);
    }

    public Task<Till?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var till = _tills.FirstOrDefault(t => t.Code == code);
        return Task.FromResult(till);
    }

    public Task<IEnumerable<Till>> GetByStatusAsync(TillStatus status, CancellationToken cancellationToken = default)
    {
        var tills = _tills.Where(t => t.Status == status);
        return Task.FromResult(tills);
    }

    public Task<IEnumerable<Till>> GetByLocationAsync(string location, CancellationToken cancellationToken = default)
    {
        var tills = _tills.Where(t => t.Location.Contains(location, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(tills);
    }

    public Task<bool> IsCodeUniqueAsync(string code, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(!_tills.Any(t => t.Code == code));
    }
}
