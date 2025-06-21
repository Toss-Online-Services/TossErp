using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TossErp.Application.DTOs;
using TossErp.Application.Services;
using TossErp.Domain.AggregatesModel.CooperativeAggregate;
using TossErp.Domain.Enums;
using TossErp.Domain.SeedWork;

namespace TossErp.Application.Services
{
    public class CooperativeService : ICooperativeService
    {
        private readonly ICooperativeRepository _cooperativeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CooperativeService(ICooperativeRepository cooperativeRepository, IUnitOfWork unitOfWork)
        {
            _cooperativeRepository = cooperativeRepository ?? throw new ArgumentNullException(nameof(cooperativeRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<CooperativeResponse> CreateCooperativeAsync(CreateCooperativeRequest request)
        {
            // Validate request
            if (string.IsNullOrWhiteSpace(request.CooperativeName))
                throw new ArgumentException("Cooperative name is required");

            if (string.IsNullOrWhiteSpace(request.TradingName))
                throw new ArgumentException("Trading name is required");

            // Check if cooperative name already exists
            if (await _cooperativeRepository.ExistsByNameAsync(request.CooperativeName))
                throw new InvalidOperationException($"Cooperative with name '{request.CooperativeName}' already exists");

            // Create value objects
            var address = new Address(
                request.Address.StreetAddress,
                request.Address.Township,
                request.Address.Province,
                request.Address.PostalCode,
                request.Address.Landmark
            );

            var contactInfo = new ContactInfo(
                request.ContactInfo.PhoneNumber,
                request.ContactInfo.EmailAddress,
                request.ContactInfo.WhatsAppNumber,
                request.ContactInfo.Website
            );

            BankAccountDetails? bankAccountDetails = null;
            if (request.BankAccountDetails != null)
            {
                bankAccountDetails = new BankAccountDetails(
                    request.BankAccountDetails.BankName,
                    request.BankAccountDetails.AccountNumber,
                    request.BankAccountDetails.AccountType,
                    request.BankAccountDetails.BranchCode,
                    request.BankAccountDetails.AccountHolderName,
                    request.BankAccountDetails.SwiftCode
                );
            }

            // Create cooperative
            var cooperative = new Cooperative(
                request.CooperativeName,
                request.TradingName,
                request.CooperativeType,
                request.Description,
                address,
                contactInfo,
                bankAccountDetails,
                request.InitialShareValue,
                request.MinimumMembers,
                request.MaximumMembers,
                request.EstablishedDate
            );

            var createdCooperative = await _cooperativeRepository.AddAsync(cooperative);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(createdCooperative);
        }

        public async Task<CooperativeResponse> GetCooperativeByIdAsync(Guid id)
        {
            var cooperative = await _cooperativeRepository.GetByIdAsync(id);
            if (cooperative == null)
                throw new InvalidOperationException($"Cooperative with ID '{id}' not found");

            return MapToResponse(cooperative);
        }

        public async Task<IEnumerable<CooperativeResponse>> GetAllCooperativesAsync()
        {
            var cooperatives = await _cooperativeRepository.GetAllAsync();
            return cooperatives.Select(MapToResponse);
        }

        public async Task<IEnumerable<CooperativeResponse>> GetCooperativesByTypeAsync(CooperativeType cooperativeType)
        {
            var cooperatives = await _cooperativeRepository.GetByCooperativeTypeAsync(cooperativeType);
            return cooperatives.Select(MapToResponse);
        }

        public async Task<IEnumerable<CooperativeResponse>> GetCooperativesByTypeAsync(string cooperativeType)
        {
            var type = Enum.Parse<CooperativeType>(cooperativeType);
            var cooperatives = await _cooperativeRepository.GetByCooperativeTypeAsync(type);
            return cooperatives.Select(MapToResponse);
        }

        public async Task<IEnumerable<CooperativeResponse>> GetCooperativesByTownshipAsync(string township)
        {
            var cooperatives = await _cooperativeRepository.GetByTownshipAsync(township);
            return cooperatives.Select(MapToResponse);
        }

        public async Task<IEnumerable<CooperativeResponse>> GetCooperativesByProvinceAsync(string province)
        {
            var cooperatives = await _cooperativeRepository.GetByProvinceAsync(province);
            return cooperatives.Select(MapToResponse);
        }

        public async Task<IEnumerable<CooperativeResponse>> GetRegisteredCooperativesAsync()
        {
            var cooperatives = await _cooperativeRepository.GetRegisteredAsync();
            return cooperatives.Select(MapToResponse);
        }

        public async Task<IEnumerable<CooperativeResponse>> GetActiveCooperativesAsync()
        {
            var cooperatives = await _cooperativeRepository.GetActiveAsync();
            return cooperatives.Select(MapToResponse);
        }

        public async Task<IEnumerable<CooperativeResponse>> SearchCooperativesAsync(string searchTerm)
        {
            var cooperatives = await _cooperativeRepository.SearchAsync(searchTerm);
            return cooperatives.Select(MapToResponse);
        }

        public async Task<CooperativeResponse> UpdateCooperativeAsync(Guid id, UpdateCooperativeRequest request)
        {
            var cooperative = await _cooperativeRepository.GetByIdAsync(id);
            if (cooperative == null)
                throw new InvalidOperationException($"Cooperative with ID '{id}' not found");

            // Update basic information
            if (!string.IsNullOrWhiteSpace(request.TradingName))
                cooperative.UpdateTradingName(request.TradingName);

            if (!string.IsNullOrWhiteSpace(request.Description))
                cooperative.UpdateDescription(request.Description);

            if (request.Address != null)
            {
                var address = new Address(
                    request.Address.StreetAddress,
                    request.Address.Township,
                    request.Address.Province,
                    request.Address.PostalCode,
                    request.Address.Landmark
                );
                cooperative.UpdateAddress(address);
            }

            if (request.ContactInfo != null)
            {
                var contactInfo = new ContactInfo(
                    request.ContactInfo.PhoneNumber,
                    request.ContactInfo.EmailAddress,
                    request.ContactInfo.WhatsAppNumber,
                    request.ContactInfo.Website
                );
                cooperative.UpdateContactInfo(contactInfo);
            }

            if (request.BankAccountDetails != null)
            {
                var bankAccountDetails = new BankAccountDetails(
                    request.BankAccountDetails.BankName,
                    request.BankAccountDetails.AccountNumber,
                    request.BankAccountDetails.AccountType,
                    request.BankAccountDetails.BranchCode,
                    request.BankAccountDetails.AccountHolderName,
                    request.BankAccountDetails.SwiftCode
                );
                cooperative.UpdateBankAccountDetails(bankAccountDetails);
            }

            _cooperativeRepository.Update(cooperative);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(cooperative);
        }

        public async Task<CooperativeResponse> RegisterCooperativeAsync(Guid id)
        {
            var cooperative = await _cooperativeRepository.GetByIdAsync(id);
            if (cooperative == null)
                throw new InvalidOperationException($"Cooperative with ID '{id}' not found");

            cooperative.Register();
            _cooperativeRepository.Update(cooperative);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(cooperative);
        }

        public async Task<CooperativeResponse> ActivateCooperativeAsync(Guid id)
        {
            var cooperative = await _cooperativeRepository.GetByIdAsync(id);
            if (cooperative == null)
                throw new InvalidOperationException($"Cooperative with ID '{id}' not found");

            cooperative.Activate();
            _cooperativeRepository.Update(cooperative);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(cooperative);
        }

        public async Task<CooperativeResponse> DeactivateCooperativeAsync(Guid id)
        {
            var cooperative = await _cooperativeRepository.GetByIdAsync(id);
            if (cooperative == null)
                throw new InvalidOperationException($"Cooperative with ID '{id}' not found");

            cooperative.Deactivate();
            _cooperativeRepository.Update(cooperative);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(cooperative);
        }

        public async Task<CooperativeMemberResponse> AddMemberAsync(Guid cooperativeId, AddCooperativeMemberRequest request)
        {
            var cooperative = await _cooperativeRepository.GetByIdAsync(cooperativeId);
            if (cooperative == null)
                throw new InvalidOperationException($"Cooperative with ID '{cooperativeId}' not found");

            var member = cooperative.AddMember(
                request.FirstName,
                request.LastName,
                request.IdNumber,
                request.PhoneNumber,
                request.Email,
                request.ShareValue,
                request.Role
            );

            _cooperativeRepository.Update(cooperative);
            await _unitOfWork.SaveChangesAsync();

            return MapToMemberResponse(member);
        }

        public async Task<CooperativeMemberResponse> UpdateMemberAsync(Guid cooperativeId, Guid memberId, UpdateCooperativeMemberRequest request)
        {
            var cooperative = await _cooperativeRepository.GetByIdAsync(cooperativeId);
            if (cooperative == null)
                throw new InvalidOperationException($"Cooperative with ID '{cooperativeId}' not found");

            var member = cooperative.UpdateMember(
                memberId,
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.Email,
                request.Address
            );

            _cooperativeRepository.Update(cooperative);
            await _unitOfWork.SaveChangesAsync();

            return MapToMemberResponse(member);
        }

        public async Task RemoveMemberAsync(Guid cooperativeId, Guid memberId)
        {
            var cooperative = await _cooperativeRepository.GetByIdAsync(cooperativeId);
            if (cooperative == null)
                throw new InvalidOperationException($"Cooperative with ID '{cooperativeId}' not found");

            cooperative.RemoveMember(memberId);
            _cooperativeRepository.Update(cooperative);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<CooperativeDocumentResponse> AddDocumentAsync(Guid cooperativeId, AddCooperativeDocumentRequest request)
        {
            var cooperative = await _cooperativeRepository.GetByIdAsync(cooperativeId);
            if (cooperative == null)
                throw new InvalidOperationException($"Cooperative with ID '{cooperativeId}' not found");

            var document = cooperative.AddDocument(
                request.DocumentType,
                request.DocumentNumber,
                request.IssueDate,
                request.ExpiryDate,
                request.DocumentUrl
            );

            _cooperativeRepository.Update(cooperative);
            await _unitOfWork.SaveChangesAsync();

            return MapToDocumentResponse(document);
        }

        public async Task<CooperativeMeetingResponse> ScheduleMeetingAsync(Guid cooperativeId, ScheduleCooperativeMeetingRequest request)
        {
            var cooperative = await _cooperativeRepository.GetByIdAsync(cooperativeId);
            if (cooperative == null)
                throw new InvalidOperationException($"Cooperative with ID '{cooperativeId}' not found");

            var meeting = cooperative.ScheduleMeeting(
                request.MeetingType,
                request.Subject,
                request.Description,
                request.ScheduledDate,
                request.Location
            );

            _cooperativeRepository.Update(cooperative);
            await _unitOfWork.SaveChangesAsync();

            return MapToMeetingResponse(meeting);
        }

        public async Task DeleteCooperativeAsync(Guid id)
        {
            var cooperative = await _cooperativeRepository.GetByIdAsync(id);
            if (cooperative == null)
                throw new InvalidOperationException($"Cooperative with ID '{id}' not found");

            _cooperativeRepository.Delete(cooperative);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<CooperativeStatisticsResponse> GetCooperativeStatisticsAsync()
        {
            var cooperatives = await _cooperativeRepository.GetAllAsync();
            var activeCooperatives = cooperatives.Where(c => c.IsActive).ToList();
            var totalMembers = activeCooperatives.Sum(c => c.GetActiveMemberCount());
            var totalShareValue = activeCooperatives.Sum(c => c.GetTotalShareValue());
            var meetingsThisMonth = activeCooperatives.Sum(c => c.Meetings.Count(m => m.ScheduledDate.Month == DateTime.UtcNow.Month));
            var topTownships = activeCooperatives
                .GroupBy(c => c.Address.Township)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key)
                .ToList();

            return new CooperativeStatisticsResponse
            {
                TotalCooperatives = cooperatives.Count(),
                ActiveCooperatives = activeCooperatives.Count(),
                TotalMembers = totalMembers,
                TotalShareValue = totalShareValue,
                MeetingsThisMonth = meetingsThisMonth,
                TopTownships = topTownships
            };
        }

        private static CooperativeResponse MapToResponse(Cooperative cooperative)
        {
            return new CooperativeResponse
            {
                Id = cooperative.Id,
                CooperativeName = cooperative.CooperativeName,
                TradingName = cooperative.TradingName,
                CooperativeType = cooperative.CooperativeType,
                Description = cooperative.Description,
                Address = new AddressDto
                {
                    StreetAddress = cooperative.Address.StreetAddress,
                    Township = cooperative.Address.Township,
                    Province = cooperative.Address.Province,
                    PostalCode = cooperative.Address.PostalCode,
                    Landmark = cooperative.Address.Landmark
                },
                ContactInfo = new ContactInfoDto
                {
                    PhoneNumber = cooperative.ContactInfo.PhoneNumber,
                    EmailAddress = cooperative.ContactInfo.EmailAddress,
                    WhatsAppNumber = cooperative.ContactInfo.WhatsAppNumber,
                    Website = cooperative.ContactInfo.Website
                },
                BankAccountDetails = cooperative.BankAccountDetails != null ? new BankAccountDetailsDto
                {
                    BankName = cooperative.BankAccountDetails.BankName,
                    AccountNumber = cooperative.BankAccountDetails.AccountNumber,
                    AccountType = cooperative.BankAccountDetails.AccountType,
                    BranchCode = cooperative.BankAccountDetails.BranchCode,
                    AccountHolderName = cooperative.BankAccountDetails.AccountHolderName,
                    SwiftCode = cooperative.BankAccountDetails.SwiftCode
                } : null,
                InitialShareValue = cooperative.InitialShareValue,
                MinimumMembers = cooperative.MinimumMembers,
                MaximumMembers = cooperative.MaximumMembers,
                IsRegistered = cooperative.IsRegistered,
                RegistrationNumber = cooperative.RegistrationNumber,
                TaxNumber = cooperative.TaxNumber,
                IsActive = cooperative.IsActive,
                EstablishedDate = cooperative.EstablishedDate,
                CreatedAt = cooperative.CreatedAt,
                LastModifiedAt = cooperative.LastModifiedAt,
                Members = cooperative.Members.Select(MapToMemberResponse).ToList(),
                Documents = cooperative.Documents.Select(MapToDocumentResponse).ToList(),
                Meetings = cooperative.Meetings.Select(MapToMeetingResponse).ToList(),
                ActiveMemberCount = cooperative.GetActiveMemberCount(),
                TotalShareValue = cooperative.GetTotalShareValue(),
                HasMinimumMembers = cooperative.HasMinimumMembers()
            };
        }

        private static CooperativeMemberResponse MapToMemberResponse(CooperativeMember member)
        {
            return new CooperativeMemberResponse
            {
                Id = member.Id,
                FirstName = member.FirstName,
                LastName = member.LastName,
                IdNumber = member.IdNumber,
                PhoneNumber = member.PhoneNumber,
                Email = member.Email,
                Address = member.Address,
                ShareValue = member.ShareValue,
                Role = member.Role,
                JoinDate = member.JoinDate,
                ExitDate = member.ExitDate,
                ExitReason = member.ExitReason,
                IsActive = member.IsActive,
                CreatedAt = member.CreatedAt,
                LastModifiedAt = member.LastModifiedAt
            };
        }

        private static CooperativeDocumentResponse MapToDocumentResponse(CooperativeDocument document)
        {
            return new CooperativeDocumentResponse
            {
                Id = document.Id,
                DocumentType = document.DocumentType,
                DocumentNumber = document.DocumentNumber,
                IssueDate = document.IssueDate,
                ExpiryDate = document.ExpiryDate,
                DocumentUrl = document.DocumentUrl,
                IsActive = document.IsValid,
                CreatedAt = document.CreatedDate,
                LastModifiedAt = document.LastModifiedDate,
                IsExpired = document.IsExpired,
                IsExpiringSoon = document.IsExpiringSoon()
            };
        }

        private static CooperativeMeetingResponse MapToMeetingResponse(CooperativeMeeting meeting)
        {
            var now = DateTime.UtcNow;
            var isUpcoming = meeting.ScheduledDate > now;
            var isPast = meeting.ScheduledDate < now;
            var isToday = meeting.ScheduledDate.Date == now.Date;
            var timeUntilMeeting = meeting.ScheduledDate - now;

            return new CooperativeMeetingResponse
            {
                Id = meeting.Id,
                MeetingType = meeting.MeetingType,
                Subject = meeting.Subject,
                Description = meeting.Description,
                ScheduledDate = meeting.ScheduledDate,
                Location = meeting.Location,
                IsCompleted = meeting.IsCompleted,
                CreatedAt = meeting.CreatedDate,
                LastModifiedAt = meeting.LastModifiedDate,
                IsUpcoming = isUpcoming,
                IsPast = isPast,
                IsToday = isToday,
                TimeUntilMeeting = timeUntilMeeting
            };
        }
    }
} 
