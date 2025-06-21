using System;
using TossErp.Domain.Enums;

namespace TossErp.Application.DTOs
{
    public class CreateTownshipEnterpriseRequest
    {
        public string BusinessName { get; set; } = string.Empty;
        public string? TradingName { get; set; }
        public BusinessType BusinessType { get; set; }
        public string? BusinessDescription { get; set; }
        public AddressDto Address { get; set; } = new();
        public ContactInfoDto ContactInfo { get; set; } = new();
        public Guid OwnerId { get; set; }
        public DateTime? EstablishedDate { get; set; }
    }

    public class UpdateTownshipEnterpriseRequest
    {
        public Guid Id { get; set; }
        public string? TradingName { get; set; }
        public string? BusinessDescription { get; set; }
        public AddressDto? Address { get; set; }
        public ContactInfoDto? ContactInfo { get; set; }
    }

    public class RegisterTownshipEnterpriseRequest
    {
        public Guid Id { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public string? TaxNumber { get; set; }
    }

    public class TownshipEnterpriseResponse
    {
        public Guid Id { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string? TradingName { get; set; }
        public BusinessType BusinessType { get; set; }
        public string? BusinessDescription { get; set; }
        public AddressDto Address { get; set; } = new();
        public ContactInfoDto ContactInfo { get; set; } = new();
        public Guid OwnerId { get; set; }
        public bool IsRegistered { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? TaxNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime EstablishedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public List<BusinessLicenseResponse> Licenses { get; set; } = new();
        public List<BusinessDocumentResponse> Documents { get; set; } = new();
        public List<BusinessContactResponse> Contacts { get; set; } = new();
    }

    public class BusinessLicenseResponse
    {
        public Guid Id { get; set; }
        public string LicenseType { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public string? IssuingAuthority { get; set; }
        public string? Notes { get; set; }
        public bool IsValid { get; set; }
        public bool IsExpired { get; set; }
        public bool IsExpiringSoon { get; set; }
    }

    public class BusinessDocumentResponse
    {
        public Guid Id { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
        public string? DocumentUrl { get; set; }
        public DateTime UploadDate { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        public long? FileSize { get; set; }
        public string? FileType { get; set; }
        public bool HasValidUrl { get; set; }
    }

    public class BusinessContactResponse
    {
        public Guid Id { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string? EmailAddress { get; set; }
        public string? Relationship { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? Notes { get; set; }
        public bool HasValidEmail { get; set; }
        public bool HasValidPhone { get; set; }
    }

    public class TownshipEnterpriseListResponse
    {
        public List<TownshipEnterpriseResponse> Enterprises { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class TownshipEnterpriseFilterRequest
    {
        public string? BusinessName { get; set; }
        public BusinessType? BusinessType { get; set; }
        public string? Township { get; set; }
        public string? Province { get; set; }
        public bool? IsRegistered { get; set; }
        public bool? IsActive { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
    }
} 
