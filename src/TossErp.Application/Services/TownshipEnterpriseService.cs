using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TossErp.Application.DTOs;
using TossErp.Domain.AggregatesModel.TownshipEnterpriseAggregate;
using TossErp.Domain.SeedWork;

namespace TossErp.Application.Services
{
    public class TownshipEnterpriseService : ITownshipEnterpriseService
    {
        private readonly ITownshipEnterpriseRepository _townshipEnterpriseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TownshipEnterpriseService(ITownshipEnterpriseRepository townshipEnterpriseRepository, IUnitOfWork unitOfWork)
        {
            _townshipEnterpriseRepository = townshipEnterpriseRepository ?? throw new ArgumentNullException(nameof(townshipEnterpriseRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<TownshipEnterpriseResponse> CreateTownshipEnterpriseAsync(CreateTownshipEnterpriseRequest request)
        {
            // Validate request
            if (string.IsNullOrWhiteSpace(request.BusinessName))
                throw new ArgumentException("Business name is required.");

            if (request.OwnerId == Guid.Empty)
                throw new ArgumentException("Owner ID is required.");

            // Check if business name already exists
            if (await _townshipEnterpriseRepository.ExistsByNameAsync(request.BusinessName))
                throw new ArgumentException($"A business with the name '{request.BusinessName}' already exists.");

            // Create address value object
            var address = new Address(
                request.Address.StreetAddress,
                request.Address.Township,
                request.Address.Province,
                request.Address.PostalCode,
                request.Address.Landmark
            );

            // Create contact info value object
            var contactInfo = new ContactInfo(
                request.ContactInfo.PhoneNumber,
                request.ContactInfo.EmailAddress,
                request.ContactInfo.WhatsAppNumber,
                request.ContactInfo.Website
            );

            // Create township enterprise
            var townshipEnterprise = new TownshipEnterprise(
                request.BusinessName,
                request.TradingName,
                request.BusinessType,
                request.BusinessDescription,
                address,
                contactInfo,
                request.OwnerId,
                request.EstablishedDate
            );

            // Add to repository
            await _townshipEnterpriseRepository.AddAsync(townshipEnterprise);
            await _unitOfWork.SaveChangesAsync();

            // Return response
            return MapToResponse(townshipEnterprise);
        }

        public async Task<TownshipEnterpriseResponse?> GetTownshipEnterpriseByIdAsync(Guid id)
        {
            var townshipEnterprise = await _townshipEnterpriseRepository.GetByIdAsync(id);
            if (townshipEnterprise == null)
                return null;

            return MapToResponse(townshipEnterprise);
        }

        public async Task<TownshipEnterpriseResponse> UpdateTownshipEnterpriseAsync(UpdateTownshipEnterpriseRequest request)
        {
            var townshipEnterprise = await _townshipEnterpriseRepository.GetByIdAsync(request.Id);
            if (townshipEnterprise == null)
                throw new ArgumentException($"Township enterprise with ID {request.Id} not found.");

            // Update properties
            if (!string.IsNullOrWhiteSpace(request.TradingName))
                townshipEnterprise.UpdateTradingName(request.TradingName);

            if (!string.IsNullOrWhiteSpace(request.BusinessDescription))
                townshipEnterprise.UpdateBusinessDescription(request.BusinessDescription);

            // Update address
            if (request.Address != null)
            {
                var address = new Address(
                    request.Address.StreetAddress,
                    request.Address.Township,
                    request.Address.Province,
                    request.Address.PostalCode,
                    request.Address.Landmark
                );
                townshipEnterprise.UpdateAddress(address);
            }

            // Update contact info
            if (request.ContactInfo != null)
            {
                var contactInfo = new ContactInfo(
                    request.ContactInfo.PhoneNumber,
                    request.ContactInfo.EmailAddress,
                    request.ContactInfo.WhatsAppNumber,
                    request.ContactInfo.Website
                );
                townshipEnterprise.UpdateContactInfo(contactInfo);
            }

            // Save changes
            _townshipEnterpriseRepository.Update(townshipEnterprise);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(townshipEnterprise);
        }

        public async Task<TownshipEnterpriseResponse> RegisterTownshipEnterpriseAsync(RegisterTownshipEnterpriseRequest request)
        {
            var townshipEnterprise = await _townshipEnterpriseRepository.GetByIdAsync(request.Id);
            if (townshipEnterprise == null)
                throw new ArgumentException($"Township enterprise with ID {request.Id} not found.");

            townshipEnterprise.Register(request.RegistrationNumber, request.TaxNumber);

            _townshipEnterpriseRepository.Update(townshipEnterprise);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(townshipEnterprise);
        }

        public async Task<TownshipEnterpriseResponse> ActivateTownshipEnterpriseAsync(Guid id)
        {
            var townshipEnterprise = await _townshipEnterpriseRepository.GetByIdAsync(id);
            if (townshipEnterprise == null)
                throw new ArgumentException($"Township enterprise with ID {id} not found.");

            townshipEnterprise.Activate();

            _townshipEnterpriseRepository.Update(townshipEnterprise);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(townshipEnterprise);
        }

        public async Task<TownshipEnterpriseResponse> DeactivateTownshipEnterpriseAsync(Guid id)
        {
            var townshipEnterprise = await _townshipEnterpriseRepository.GetByIdAsync(id);
            if (townshipEnterprise == null)
                throw new ArgumentException($"Township enterprise with ID {id} not found.");

            townshipEnterprise.Deactivate();

            _townshipEnterpriseRepository.Update(townshipEnterprise);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(townshipEnterprise);
        }

        public async Task<TownshipEnterpriseListResponse> GetTownshipEnterprisesAsync(TownshipEnterpriseFilterRequest filter)
        {
            var query = await _townshipEnterpriseRepository.GetAllAsync();
            var enterprises = query.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.BusinessName))
                enterprises = enterprises.Where(e => e.BusinessName.Contains(filter.BusinessName, StringComparison.OrdinalIgnoreCase));

            if (filter.BusinessType.HasValue)
                enterprises = enterprises.Where(e => e.BusinessType == filter.BusinessType.Value);

            if (!string.IsNullOrWhiteSpace(filter.Township))
                enterprises = enterprises.Where(e => e.Address.Township.Contains(filter.Township, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(filter.Province))
                enterprises = enterprises.Where(e => e.Address.Province.Contains(filter.Province, StringComparison.OrdinalIgnoreCase));

            if (filter.IsRegistered.HasValue)
                enterprises = enterprises.Where(e => e.IsRegistered == filter.IsRegistered.Value);

            if (filter.IsActive.HasValue)
                enterprises = enterprises.Where(e => e.IsActive == filter.IsActive.Value);

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                enterprises = filter.SortBy.ToLower() switch
                {
                    "businessname" => filter.SortDescending ? enterprises.OrderByDescending(e => e.BusinessName) : enterprises.OrderBy(e => e.BusinessName),
                    "businesstype" => filter.SortDescending ? enterprises.OrderByDescending(e => e.BusinessType) : enterprises.OrderBy(e => e.BusinessType),
                    "township" => filter.SortDescending ? enterprises.OrderByDescending(e => e.Address.Township) : enterprises.OrderBy(e => e.Address.Township),
                    "province" => filter.SortDescending ? enterprises.OrderByDescending(e => e.Address.Province) : enterprises.OrderBy(e => e.Address.Province),
                    "createdat" => filter.SortDescending ? enterprises.OrderByDescending(e => e.CreatedAt) : enterprises.OrderBy(e => e.CreatedAt),
                    _ => filter.SortDescending ? enterprises.OrderByDescending(e => e.CreatedAt) : enterprises.OrderBy(e => e.CreatedAt)
                };
            }
            else
            {
                enterprises = enterprises.OrderByDescending(e => e.CreatedAt);
            }

            // Apply pagination
            var totalCount = enterprises.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);
            var pagedEnterprises = enterprises
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return new TownshipEnterpriseListResponse
            {
                Enterprises = pagedEnterprises.Select(MapToResponse).ToList(),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = totalPages
            };
        }

        public async Task<BusinessLicenseResponse> AddLicenseAsync(Guid enterpriseId, string licenseType, string licenseNumber, DateTime issueDate, DateTime expiryDate, string? issuingAuthority = null, string? notes = null)
        {
            var townshipEnterprise = await _townshipEnterpriseRepository.GetByIdAsync(enterpriseId);
            if (townshipEnterprise == null)
                throw new ArgumentException($"Township enterprise with ID {enterpriseId} not found.");

            var license = townshipEnterprise.AddLicense(licenseType, licenseNumber, issueDate, expiryDate, issuingAuthority, notes);

            _townshipEnterpriseRepository.Update(townshipEnterprise);
            await _unitOfWork.SaveChangesAsync();

            return MapToLicenseResponse(license);
        }

        public async Task<BusinessDocumentResponse> AddDocumentAsync(Guid enterpriseId, string documentType, string documentName, string? documentUrl = null, string? description = null, long? fileSize = null, string? fileType = null)
        {
            var townshipEnterprise = await _townshipEnterpriseRepository.GetByIdAsync(enterpriseId);
            if (townshipEnterprise == null)
                throw new ArgumentException($"Township enterprise with ID {enterpriseId} not found.");

            var document = townshipEnterprise.AddDocument(documentType, documentName, documentUrl, description, fileSize, fileType);

            _townshipEnterpriseRepository.Update(townshipEnterprise);
            await _unitOfWork.SaveChangesAsync();

            return MapToDocumentResponse(document);
        }

        public async Task<BusinessContactResponse> AddContactAsync(Guid enterpriseId, string contactName, string contactNumber, string? emailAddress = null, string? relationship = null, string? notes = null)
        {
            var townshipEnterprise = await _townshipEnterpriseRepository.GetByIdAsync(enterpriseId);
            if (townshipEnterprise == null)
                throw new ArgumentException($"Township enterprise with ID {enterpriseId} not found.");

            var contact = townshipEnterprise.AddContact(contactName, contactNumber, emailAddress, relationship, notes);

            _townshipEnterpriseRepository.Update(townshipEnterprise);
            await _unitOfWork.SaveChangesAsync();

            return MapToContactResponse(contact);
        }

        public async Task RemoveContactAsync(Guid enterpriseId, Guid contactId)
        {
            var townshipEnterprise = await _townshipEnterpriseRepository.GetByIdAsync(enterpriseId);
            if (townshipEnterprise == null)
                throw new ArgumentException($"Township enterprise with ID {enterpriseId} not found.");

            townshipEnterprise.RemoveContact(contactId);

            _townshipEnterpriseRepository.Update(townshipEnterprise);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> HasValidLicenseAsync(Guid enterpriseId, string licenseType)
        {
            return await _townshipEnterpriseRepository.HasValidLicenseAsync(enterpriseId, licenseType);
        }

        public async Task<bool> IsInTownshipAsync(Guid enterpriseId, string townshipName)
        {
            return await _townshipEnterpriseRepository.IsInTownshipAsync(enterpriseId, townshipName);
        }

        public async Task<bool> IsInProvinceAsync(Guid enterpriseId, string provinceName)
        {
            return await _townshipEnterpriseRepository.IsInProvinceAsync(enterpriseId, provinceName);
        }

        #region Private Mapping Methods

        private static TownshipEnterpriseResponse MapToResponse(TownshipEnterprise enterprise)
        {
            return new TownshipEnterpriseResponse
            {
                Id = enterprise.Id,
                BusinessName = enterprise.BusinessName,
                TradingName = enterprise.TradingName,
                BusinessType = enterprise.BusinessType,
                BusinessDescription = enterprise.BusinessDescription,
                Address = new AddressDto
                {
                    StreetAddress = enterprise.Address.StreetAddress,
                    Township = enterprise.Address.Township,
                    Province = enterprise.Address.Province,
                    PostalCode = enterprise.Address.PostalCode,
                    Landmark = enterprise.Address.Landmark
                },
                ContactInfo = new ContactInfoDto
                {
                    PhoneNumber = enterprise.ContactInfo.PhoneNumber,
                    EmailAddress = enterprise.ContactInfo.EmailAddress,
                    WhatsAppNumber = enterprise.ContactInfo.WhatsAppNumber,
                    Website = enterprise.ContactInfo.Website
                },
                OwnerId = enterprise.OwnerId,
                IsRegistered = enterprise.IsRegistered,
                RegistrationNumber = enterprise.RegistrationNumber,
                TaxNumber = enterprise.TaxNumber,
                IsActive = enterprise.IsActive,
                EstablishedDate = enterprise.EstablishedDate,
                CreatedAt = enterprise.CreatedAt,
                LastModifiedAt = enterprise.LastModifiedAt,
                Licenses = enterprise.Licenses.Select(MapToLicenseResponse).ToList(),
                Documents = enterprise.Documents.Select(MapToDocumentResponse).ToList(),
                Contacts = enterprise.Contacts.Select(MapToContactResponse).ToList()
            };
        }

        private static BusinessLicenseResponse MapToLicenseResponse(BusinessLicense license)
        {
            return new BusinessLicenseResponse
            {
                Id = license.Id,
                LicenseType = license.LicenseType,
                LicenseNumber = license.LicenseNumber,
                IssueDate = license.IssueDate,
                ExpiryDate = license.ExpiryDate,
                IsActive = license.IsActive,
                IssuingAuthority = license.IssuingAuthority,
                Notes = license.Notes,
                IsValid = license.IsValid,
                IsExpired = license.IsExpired,
                IsExpiringSoon = license.IsExpiringSoon()
            };
        }

        private static BusinessDocumentResponse MapToDocumentResponse(BusinessDocument document)
        {
            return new BusinessDocumentResponse
            {
                Id = document.Id,
                DocumentType = document.DocumentType,
                DocumentName = document.DocumentName,
                DocumentUrl = document.DocumentUrl,
                UploadDate = document.UploadDate,
                IsActive = document.IsActive,
                Description = document.Description,
                FileSize = document.FileSize,
                FileType = document.FileType,
                HasValidUrl = !string.IsNullOrEmpty(document.DocumentUrl)
            };
        }

        private static BusinessContactResponse MapToContactResponse(BusinessContact contact)
        {
            return new BusinessContactResponse
            {
                Id = contact.Id,
                ContactName = contact.ContactName,
                ContactNumber = contact.ContactNumber,
                EmailAddress = contact.EmailAddress,
                Relationship = contact.Relationship,
                IsActive = contact.IsActive,
                CreatedAt = contact.CreatedAt,
                LastModifiedAt = contact.LastModifiedAt,
                Notes = contact.Notes,
                HasValidEmail = !string.IsNullOrEmpty(contact.EmailAddress),
                HasValidPhone = !string.IsNullOrEmpty(contact.ContactNumber)
            };
        }

        #endregion
    }
} 
