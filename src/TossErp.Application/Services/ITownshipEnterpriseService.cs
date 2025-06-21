using System;
using System.Threading.Tasks;
using TossErp.Application.DTOs;

namespace TossErp.Application.Services
{
    public interface ITownshipEnterpriseService
    {
        Task<TownshipEnterpriseResponse> CreateTownshipEnterpriseAsync(CreateTownshipEnterpriseRequest request);
        Task<TownshipEnterpriseResponse?> GetTownshipEnterpriseByIdAsync(Guid id);
        Task<TownshipEnterpriseResponse> UpdateTownshipEnterpriseAsync(UpdateTownshipEnterpriseRequest request);
        Task<TownshipEnterpriseResponse> RegisterTownshipEnterpriseAsync(RegisterTownshipEnterpriseRequest request);
        Task<TownshipEnterpriseResponse> ActivateTownshipEnterpriseAsync(Guid id);
        Task<TownshipEnterpriseResponse> DeactivateTownshipEnterpriseAsync(Guid id);
        Task<TownshipEnterpriseListResponse> GetTownshipEnterprisesAsync(TownshipEnterpriseFilterRequest filter);
        Task<BusinessLicenseResponse> AddLicenseAsync(Guid enterpriseId, string licenseType, string licenseNumber, DateTime issueDate, DateTime expiryDate, string? issuingAuthority = null, string? notes = null);
        Task<BusinessDocumentResponse> AddDocumentAsync(Guid enterpriseId, string documentType, string documentName, string? documentUrl = null, string? description = null, long? fileSize = null, string? fileType = null);
        Task<BusinessContactResponse> AddContactAsync(Guid enterpriseId, string contactName, string contactNumber, string? emailAddress = null, string? relationship = null, string? notes = null);
        Task RemoveContactAsync(Guid enterpriseId, Guid contactId);
        Task<bool> HasValidLicenseAsync(Guid enterpriseId, string licenseType);
        Task<bool> IsInTownshipAsync(Guid enterpriseId, string townshipName);
        Task<bool> IsInProvinceAsync(Guid enterpriseId, string provinceName);
    }
} 
