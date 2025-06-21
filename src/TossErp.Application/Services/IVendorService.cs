using TossErp.Application.DTOs;

namespace TossErp.Application.Services;

public interface IVendorService
{
    Task<VendorDto> GetByIdAsync(Guid id);
    Task<IEnumerable<VendorDto>> GetAllAsync();
    Task<IEnumerable<VendorDto>> GetByBusinessAsync(Guid businessId);
    Task<IEnumerable<VendorDto>> SearchAsync(string searchTerm);
    Task<VendorDto> CreateAsync(CreateVendorDto createVendorDto);
    Task<VendorDto> UpdateAsync(Guid id, CreateVendorDto updateVendorDto);
    Task DeleteAsync(Guid id);
} 
