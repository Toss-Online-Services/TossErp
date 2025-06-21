using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TossErp.Application.DTOs;
using TossErp.Application.Services;
using TossErp.Domain.AggregatesModel.StokvelAggregate;
using TossErp.Domain.Enums;
using TossErp.Domain.SeedWork;

namespace TossErp.Application.Services
{
    public class StokvelService : IStokvelService
    {
        private readonly IStokvelRepository _stokvelRepository;
        private readonly IUnitOfWork _unitOfWork;

        public StokvelService(IStokvelRepository stokvelRepository, IUnitOfWork unitOfWork)
        {
            _stokvelRepository = stokvelRepository ?? throw new ArgumentNullException(nameof(stokvelRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<StokvelResponseDTO> CreateStokvelAsync(CreateStokvelRequestDTO request)
        {
            // Validate request
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Stokvel name is required");

            if (string.IsNullOrWhiteSpace(request.Description))
                throw new ArgumentException("Description is required");

            // Check if stokvel name already exists
            if (await _stokvelRepository.ExistsByNameAsync(request.Name))
                throw new InvalidOperationException($"Stokvel with name '{request.Name}' already exists");

            // Create stokvel with basic settings
            var stokvel = new Stokvel(
                request.Name,
                request.Description,
                Enum.Parse<StokvelType>(request.StokvelType),
                new ContributionSettings(request.ContributionAmount, request.ContributionFrequency),
                new PayoutSettings(),
                new MeetingSettings()
            );

            var createdStokvel = await _stokvelRepository.AddAsync(stokvel);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponseDTO(createdStokvel);
        }

        public async Task<StokvelResponseDTO> GetStokvelByIdAsync(Guid id)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(id);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{id}' not found");

            return MapToResponseDTO(stokvel);
        }

        public async Task<IEnumerable<StokvelResponseDTO>> GetAllStokvelsAsync()
        {
            var stokvels = await _stokvelRepository.GetAllAsync();
            return stokvels.Select(MapToResponseDTO);
        }

        public async Task<StokvelResponseDTO> UpdateStokvelAsync(UpdateStokvelRequestDTO request)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(request.Id);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{request.Id}' not found");

            // Update basic information
            if (!string.IsNullOrWhiteSpace(request.Description))
                stokvel.UpdateDescription(request.Description);

            if (request.ContributionAmount > 0)
            {
                var contributionSettings = new ContributionSettings(request.ContributionAmount, request.ContributionFrequency);
                stokvel.UpdateContributionSettings(contributionSettings);
            }

            _stokvelRepository.Update(stokvel);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponseDTO(stokvel);
        }

        public async Task DeleteStokvelAsync(Guid id)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(id);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{id}' not found");

            _stokvelRepository.Delete(stokvel);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<StokvelResponseDTO>> GetStokvelsByTypeAsync(string stokvelType)
        {
            var type = Enum.Parse<StokvelType>(stokvelType);
            var stokvels = await _stokvelRepository.GetByStokvelTypeAsync(type);
            return stokvels.Select(MapToResponseDTO);
        }

        public async Task<IEnumerable<StokvelResponseDTO>> GetStokvelsByContributionAmountRangeAsync(decimal minAmount, decimal maxAmount)
        {
            var stokvels = await _stokvelRepository.GetByContributionAmountRangeAsync(minAmount, maxAmount);
            return stokvels.Select(MapToResponseDTO);
        }

        public async Task<IEnumerable<StokvelResponseDTO>> GetStokvelsByContributionFrequencyAsync(string frequency)
        {
            var stokvels = await _stokvelRepository.GetByContributionFrequencyAsync(frequency);
            return stokvels.Select(MapToResponseDTO);
        }

        public async Task<IEnumerable<StokvelResponseDTO>> GetActiveStokvelsAsync()
        {
            var stokvels = await _stokvelRepository.GetActiveAsync();
            return stokvels.Select(MapToResponseDTO);
        }

        public async Task<IEnumerable<StokvelResponseDTO>> SearchStokvelsAsync(string searchTerm)
        {
            var stokvels = await _stokvelRepository.SearchAsync(searchTerm);
            return stokvels.Select(MapToResponseDTO);
        }

        public async Task<StokvelResponseDTO> ActivateStokvelAsync(Guid id)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(id);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{id}' not found");

            stokvel.Activate();
            _stokvelRepository.Update(stokvel);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponseDTO(stokvel);
        }

        public async Task<StokvelResponseDTO> DeactivateStokvelAsync(Guid id)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(id);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{id}' not found");

            stokvel.Deactivate();
            _stokvelRepository.Update(stokvel);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponseDTO(stokvel);
        }

        public async Task<StokvelMemberResponseDTO> AddMemberAsync(Guid stokvelId, AddStokvelMemberRequestDTO request)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(stokvelId);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{stokvelId}' not found");

            var member = stokvel.AddMember(
                request.FirstName,
                request.LastName,
                request.IdNumber,
                request.PhoneNumber,
                request.Email,
                request.Address
            );

            _stokvelRepository.Update(stokvel);
            await _unitOfWork.SaveChangesAsync();

            return MapToMemberResponseDTO(member);
        }

        public async Task<StokvelMemberResponseDTO> UpdateMemberAsync(Guid stokvelId, Guid memberId, UpdateStokvelMemberRequestDTO request)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(stokvelId);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{stokvelId}' not found");

            var member = stokvel.UpdateMember(
                memberId,
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.Email,
                request.Address
            );

            _stokvelRepository.Update(stokvel);
            await _unitOfWork.SaveChangesAsync();

            return MapToMemberResponseDTO(member);
        }

        public async Task RemoveMemberAsync(Guid stokvelId, Guid memberId)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(stokvelId);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{stokvelId}' not found");

            stokvel.RemoveMember(memberId);
            _stokvelRepository.Update(stokvel);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<StokvelContributionResponseDTO> RecordContributionAsync(Guid stokvelId, RecordContributionRequestDTO request)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(stokvelId);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{stokvelId}' not found");

            var contribution = stokvel.RecordContribution(
                request.MemberId,
                request.Amount,
                request.Date,
                request.Reference
            );

            _stokvelRepository.Update(stokvel);
            await _unitOfWork.SaveChangesAsync();

            return MapToContributionResponseDTO(contribution);
        }

        public async Task<StokvelPayoutResponseDTO> ProcessPayoutAsync(Guid stokvelId, ProcessPayoutRequestDTO request)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(stokvelId);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{stokvelId}' not found");

            var payout = stokvel.ProcessPayout(
                request.MemberId,
                request.Amount,
                request.Date,
                request.Reference
            );

            _stokvelRepository.Update(stokvel);
            await _unitOfWork.SaveChangesAsync();

            return MapToPayoutResponseDTO(payout);
        }

        public async Task<StokvelMeetingResponseDTO> ScheduleMeetingAsync(Guid stokvelId, ScheduleStokvelMeetingRequestDTO request)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(stokvelId);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{stokvelId}' not found");

            var meeting = stokvel.ScheduleMeeting(
                request.MeetingTitle,
                request.MeetingDate,
                request.Location,
                request.Agenda
            );

            _stokvelRepository.Update(stokvel);
            await _unitOfWork.SaveChangesAsync();

            return MapToMeetingResponseDTO(meeting);
        }

        public async Task<StokvelFinancialSummaryDTO> GetFinancialSummaryAsync(Guid stokvelId)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(stokvelId);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{stokvelId}' not found");

            var totalContributions = stokvel.GetTotalContributions();
            var totalPayouts = stokvel.GetTotalPayouts();
            var currentBalance = totalContributions - totalPayouts;
            var memberCount = stokvel.GetActiveMemberCount();

            return new StokvelFinancialSummaryDTO
            {
                TotalContributions = totalContributions,
                TotalPayouts = totalPayouts,
                CurrentBalance = currentBalance,
                MemberCount = memberCount
            };
        }

        public async Task<StokvelMemberFinancialDTO> GetMemberFinancialSummaryAsync(Guid stokvelId, Guid memberId)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(stokvelId);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{stokvelId}' not found");

            var member = stokvel.GetMember(memberId);
            if (member == null)
                throw new InvalidOperationException($"Member with ID '{memberId}' not found in stokvel");

            var contributionTotal = stokvel.GetMemberContributionTotal(memberId);
            var payoutTotal = stokvel.GetMemberPayoutTotal(memberId);
            var balance = contributionTotal - payoutTotal;

            var summary = new StokvelMemberFinancialDTO
            {
                MemberId = memberId,
                MemberName = $"{member.FirstName} {member.LastName}",
                TotalContributions = contributionTotal,
                TotalPayouts = payoutTotal,
                CurrentBalance = balance,
                ContributionCount = stokvel.GetMemberContributionCount(memberId),
                PayoutCount = stokvel.GetMemberPayoutCount(memberId),
                LastContributionDate = stokvel.GetLastContributionDate(memberId),
                LastPayoutDate = stokvel.GetLastPayoutDate(memberId),
                IsInGoodStanding = balance >= 0,
                OutstandingAmount = balance < 0 ? Math.Abs(balance) : 0
            };

            return summary;
        }

        public async Task<StokvelStatisticsDTO> GetStokvelStatisticsAsync()
        {
            var allStokvels = await _stokvelRepository.GetAllAsync();
            var activeStokvels = allStokvels.Where(s => s.IsActive).ToList();

            var totalContributions = activeStokvels.Sum(s => s.GetTotalContributions());
            var totalPayouts = activeStokvels.Sum(s => s.GetTotalPayouts());
            var totalMembers = activeStokvels.Sum(s => s.GetActiveMemberCount());

            return new StokvelStatisticsDTO
            {
                TotalStokvels = allStokvels.Count(),
                ActiveStokvels = activeStokvels.Count(),
                TotalMembers = totalMembers,
                TotalContributions = totalContributions,
                TotalPayouts = totalPayouts,
                MeetingsThisMonth = 0, // TODO: Implement meeting counting
                TopTownships = new List<string>() // TODO: Implement township analysis
            };
        }

        public async Task<decimal> GetTotalContributionsAsync(Guid stokvelId)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(stokvelId);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{stokvelId}' not found");

            return stokvel.GetTotalContributions();
        }

        public async Task<decimal> GetTotalPayoutsAsync(Guid stokvelId)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(stokvelId);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{stokvelId}' not found");

            return stokvel.GetTotalPayouts();
        }

        public async Task<decimal> GetCurrentBalanceAsync(Guid stokvelId)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(stokvelId);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{stokvelId}' not found");

            return stokvel.GetTotalContributions() - stokvel.GetTotalPayouts();
        }

        public async Task<decimal> GetMemberContributionTotalAsync(Guid stokvelId, Guid memberId)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(stokvelId);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{stokvelId}' not found");

            return stokvel.GetMemberContributionTotal(memberId);
        }

        public async Task<decimal> GetMemberPayoutTotalAsync(Guid stokvelId, Guid memberId)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(stokvelId);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{stokvelId}' not found");

            return stokvel.GetMemberPayoutTotal(memberId);
        }

        public async Task<decimal> GetMemberBalanceAsync(Guid stokvelId, Guid memberId)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(stokvelId);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{stokvelId}' not found");

            return stokvel.GetMemberContributionTotal(memberId) - stokvel.GetMemberPayoutTotal(memberId);
        }

        public async Task<int> GetActiveMemberCountAsync(Guid stokvelId)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(stokvelId);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{stokvelId}' not found");

            return stokvel.GetActiveMemberCount();
        }

        public async Task<bool> HasMinimumMembersAsync(Guid stokvelId, int minimumMembers = 5)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(stokvelId);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{stokvelId}' not found");

            return stokvel.GetActiveMemberCount() >= minimumMembers;
        }

        public async Task<bool> IsFullAsync(Guid stokvelId)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(stokvelId);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{stokvelId}' not found");

            return stokvel.IsFull();
        }

        public async Task<List<StokvelMemberResponseDTO>> GetMembersInRotationOrderAsync(Guid stokvelId)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(stokvelId);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{stokvelId}' not found");

            var members = stokvel.GetMembersInRotationOrder();
            return members.Select(MapToMemberResponseDTO).ToList();
        }

        public async Task<MemberBalanceResponse> GetMemberBalanceDetailsAsync(Guid stokvelId, Guid memberId)
        {
            var stokvel = await _stokvelRepository.GetByIdAsync(stokvelId);
            if (stokvel == null)
                throw new InvalidOperationException($"Stokvel with ID '{stokvelId}' not found");

            var member = stokvel.GetMember(memberId);
            if (member == null)
                throw new InvalidOperationException($"Member with ID '{memberId}' not found in stokvel");

            var contributionTotal = stokvel.GetMemberContributionTotal(memberId);
            var payoutTotal = stokvel.GetMemberPayoutTotal(memberId);
            var balance = contributionTotal - payoutTotal;

            return new MemberBalanceResponse
            {
                MemberId = memberId,
                MemberName = $"{member.FirstName} {member.LastName}",
                ContributionTotal = contributionTotal,
                PayoutTotal = payoutTotal,
                Balance = balance,
                ContributionCount = stokvel.GetMemberContributionCount(memberId),
                PayoutCount = stokvel.GetMemberPayoutCount(memberId)
            };
        }

        private static StokvelResponseDTO MapToResponseDTO(Stokvel stokvel)
        {
            return new StokvelResponseDTO
            {
                Id = stokvel.Id,
                Name = stokvel.Name,
                Description = stokvel.Description,
                StokvelType = stokvel.StokvelType.ToString(),
                ContributionAmount = stokvel.ContributionSettings.Amount,
                ContributionFrequency = stokvel.ContributionSettings.Frequency,
                MinimumMembers = stokvel.MinimumMembers,
                MaxMembers = stokvel.MaxMembers,
                Status = stokvel.IsActive ? "Active" : "Inactive",
                CreatedAt = stokvel.CreatedAt,
                UpdatedAt = stokvel.LastModifiedAt,
                Members = stokvel.Members.Select(MapToMemberResponseDTO).ToList(),
                Contributions = stokvel.Contributions.Select(MapToContributionResponseDTO).ToList(),
                Payouts = stokvel.Payouts.Select(MapToPayoutResponseDTO).ToList(),
                Meetings = stokvel.Meetings.Select(MapToMeetingResponseDTO).ToList()
            };
        }

        private static StokvelMemberResponseDTO MapToMemberResponseDTO(StokvelMember member)
        {
            return new StokvelMemberResponseDTO
            {
                Id = member.Id,
                FirstName = member.FirstName,
                LastName = member.LastName,
                IdNumber = member.IdNumber,
                PhoneNumber = member.PhoneNumber,
                Email = member.Email,
                Address = member.Address,
                RotationOrder = member.RotationOrder,
                Status = member.IsActive ? "Active" : "Inactive",
                JoinDate = member.JoinDate
            };
        }

        private static StokvelContributionResponseDTO MapToContributionResponseDTO(StokvelContribution contribution)
        {
            return new StokvelContributionResponseDTO
            {
                Id = contribution.Id,
                MemberId = contribution.MemberId,
                Amount = contribution.Amount,
                Date = contribution.ContributionDate,
                Reference = contribution.ReferenceNumber ?? string.Empty,
                Status = contribution.IsConfirmed ? "Confirmed" : "Pending"
            };
        }

        private static StokvelPayoutResponseDTO MapToPayoutResponseDTO(StokvelPayout payout)
        {
            return new StokvelPayoutResponseDTO
            {
                Id = payout.Id,
                MemberId = payout.MemberId,
                Amount = payout.Amount,
                Date = payout.PayoutDate,
                Reference = payout.ReferenceNumber ?? string.Empty,
                Status = payout.IsProcessed ? "Processed" : "Pending"
            };
        }

        private static StokvelMeetingResponseDTO MapToMeetingResponseDTO(StokvelMeeting meeting)
        {
            return new StokvelMeetingResponseDTO
            {
                Id = meeting.Id,
                MeetingTitle = meeting.MeetingTitle,
                MeetingDate = meeting.MeetingDate,
                Location = meeting.Location,
                Agenda = meeting.Agenda,
                Status = meeting.IsCompleted ? "Completed" : "Scheduled",
                CreatedAt = meeting.CreatedAt,
                LastModifiedAt = meeting.LastModifiedAt,
                Attendees = new List<string>(), // TODO: Implement attendees tracking
                Minutes = new List<string>() // TODO: Implement minutes tracking
            };
        }
    }
} 
