using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TossErp.Application.Services;
using TossErp.Application.DTOs;

namespace TossErp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StokvelController : ControllerBase
    {
        private readonly IStokvelService _stokvelService;

        public StokvelController(IStokvelService stokvelService)
        {
            _stokvelService = stokvelService ?? throw new ArgumentNullException(nameof(stokvelService));
        }

        /// <summary>
        /// Creates a new stokvel
        /// </summary>
        /// <param name="request">The stokvel creation request</param>
        /// <returns>The created stokvel details</returns>
        [HttpPost]
        public async Task<ActionResult<StokvelResponse>> CreateStokvel([FromBody] CreateStokvelRequest request)
        {
            try
            {
                var result = await _stokvelService.CreateStokvelAsync(request);
                return CreatedAtAction(nameof(GetStokvel), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while creating the stokvel." });
            }
        }

        /// <summary>
        /// Gets a stokvel by ID
        /// </summary>
        /// <param name="id">The stokvel ID</param>
        /// <returns>The stokvel details</returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<StokvelResponse>> GetStokvel(Guid id)
        {
            try
            {
                var result = await _stokvelService.GetStokvelByIdAsync(id);
                if (result == null)
                    return NotFound(new { error = "Stokvel not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving the stokvel." });
            }
        }

        /// <summary>
        /// Updates a stokvel
        /// </summary>
        /// <param name="request">The stokvel update request</param>
        /// <returns>The updated stokvel details</returns>
        [HttpPut]
        public async Task<ActionResult<StokvelResponse>> UpdateStokvel([FromBody] UpdateStokvelRequest request)
        {
            try
            {
                var result = await _stokvelService.UpdateStokvelAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the stokvel." });
            }
        }

        /// <summary>
        /// Activates a stokvel
        /// </summary>
        /// <param name="id">The stokvel ID</param>
        /// <returns>The activated stokvel details</returns>
        [HttpPost("{id:guid}/activate")]
        public async Task<ActionResult<StokvelResponse>> ActivateStokvel(Guid id)
        {
            try
            {
                var result = await _stokvelService.ActivateStokvelAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while activating the stokvel." });
            }
        }

        /// <summary>
        /// Deactivates a stokvel
        /// </summary>
        /// <param name="id">The stokvel ID</param>
        /// <returns>The deactivated stokvel details</returns>
        [HttpPost("{id:guid}/deactivate")]
        public async Task<ActionResult<StokvelResponse>> DeactivateStokvel(Guid id)
        {
            try
            {
                var result = await _stokvelService.DeactivateStokvelAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deactivating the stokvel." });
            }
        }

        /// <summary>
        /// Gets a list of stokvels with filtering and pagination
        /// </summary>
        /// <param name="filter">The filter criteria</param>
        /// <returns>The list of stokvels</returns>
        [HttpGet]
        public async Task<ActionResult<StokvelListResponse>> GetStokvels([FromQuery] StokvelFilterRequest filter)
        {
            try
            {
                var result = await _stokvelService.GetStokvelsAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving the stokvels." });
            }
        }

        /// <summary>
        /// Adds a member to a stokvel
        /// </summary>
        /// <param name="request">The member addition request</param>
        /// <returns>The added member details</returns>
        [HttpPost("members")]
        public async Task<ActionResult<StokvelMemberResponse>> AddMember([FromBody] AddStokvelMemberRequest request)
        {
            try
            {
                var result = await _stokvelService.AddMemberAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while adding the member." });
            }
        }

        /// <summary>
        /// Removes a member from a stokvel
        /// </summary>
        /// <param name="stokvelId">The stokvel ID</param>
        /// <param name="memberId">The member ID</param>
        /// <param name="exitDate">The exit date</param>
        /// <param name="reason">The exit reason</param>
        /// <returns>No content on success</returns>
        [HttpDelete("{stokvelId:guid}/members/{memberId:guid}")]
        public async Task<ActionResult> RemoveMember(Guid stokvelId, Guid memberId, [FromQuery] DateTime exitDate, [FromQuery] string? reason = null)
        {
            try
            {
                await _stokvelService.RemoveMemberAsync(stokvelId, memberId, exitDate, reason);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while removing the member." });
            }
        }

        /// <summary>
        /// Records a contribution to a stokvel
        /// </summary>
        /// <param name="request">The contribution request</param>
        /// <returns>The recorded contribution details</returns>
        [HttpPost("contributions")]
        public async Task<ActionResult<StokvelContributionResponse>> RecordContribution([FromBody] RecordContributionRequest request)
        {
            try
            {
                var result = await _stokvelService.RecordContributionAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while recording the contribution." });
            }
        }

        /// <summary>
        /// Processes a payout from a stokvel
        /// </summary>
        /// <param name="request">The payout request</param>
        /// <returns>The processed payout details</returns>
        [HttpPost("payouts")]
        public async Task<ActionResult<StokvelPayoutResponse>> ProcessPayout([FromBody] ProcessPayoutRequest request)
        {
            try
            {
                var result = await _stokvelService.ProcessPayoutAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while processing the payout." });
            }
        }

        /// <summary>
        /// Schedules a meeting for a stokvel
        /// </summary>
        /// <param name="stokvelId">The stokvel ID</param>
        /// <param name="meetingTitle">The meeting title</param>
        /// <param name="meetingDate">The meeting date</param>
        /// <param name="location">The meeting location</param>
        /// <param name="agenda">The meeting agenda</param>
        /// <returns>The scheduled meeting details</returns>
        [HttpPost("{stokvelId:guid}/meetings")]
        public async Task<ActionResult<StokvelMeetingResponse>> ScheduleMeeting(
            Guid stokvelId,
            [FromQuery] string meetingTitle,
            [FromQuery] DateTime meetingDate,
            [FromQuery] string? location = null,
            [FromQuery] string? agenda = null)
        {
            try
            {
                var result = await _stokvelService.ScheduleMeetingAsync(stokvelId, meetingTitle, meetingDate, location, agenda);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while scheduling the meeting." });
            }
        }

        /// <summary>
        /// Gets the total contributions for a stokvel
        /// </summary>
        /// <param name="stokvelId">The stokvel ID</param>
        /// <returns>The total contributions amount</returns>
        [HttpGet("{stokvelId:guid}/contributions/total")]
        public async Task<ActionResult<decimal>> GetTotalContributions(Guid stokvelId)
        {
            try
            {
                var result = await _stokvelService.GetTotalContributionsAsync(stokvelId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while getting the total contributions." });
            }
        }

        /// <summary>
        /// Gets the total payouts for a stokvel
        /// </summary>
        /// <param name="stokvelId">The stokvel ID</param>
        /// <returns>The total payouts amount</returns>
        [HttpGet("{stokvelId:guid}/payouts/total")]
        public async Task<ActionResult<decimal>> GetTotalPayouts(Guid stokvelId)
        {
            try
            {
                var result = await _stokvelService.GetTotalPayoutsAsync(stokvelId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while getting the total payouts." });
            }
        }

        /// <summary>
        /// Gets the current balance for a stokvel
        /// </summary>
        /// <param name="stokvelId">The stokvel ID</param>
        /// <returns>The current balance amount</returns>
        [HttpGet("{stokvelId:guid}/balance")]
        public async Task<ActionResult<decimal>> GetCurrentBalance(Guid stokvelId)
        {
            try
            {
                var result = await _stokvelService.GetCurrentBalanceAsync(stokvelId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while getting the current balance." });
            }
        }

        /// <summary>
        /// Gets a member's contribution total
        /// </summary>
        /// <param name="stokvelId">The stokvel ID</param>
        /// <param name="memberId">The member ID</param>
        /// <returns>The member's contribution total</returns>
        [HttpGet("{stokvelId:guid}/members/{memberId:guid}/contributions")]
        public async Task<ActionResult<decimal>> GetMemberContributionTotal(Guid stokvelId, Guid memberId)
        {
            try
            {
                var result = await _stokvelService.GetMemberContributionTotalAsync(stokvelId, memberId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while getting the member contribution total." });
            }
        }

        /// <summary>
        /// Gets a member's payout total
        /// </summary>
        /// <param name="stokvelId">The stokvel ID</param>
        /// <param name="memberId">The member ID</param>
        /// <returns>The member's payout total</returns>
        [HttpGet("{stokvelId:guid}/members/{memberId:guid}/payouts")]
        public async Task<ActionResult<decimal>> GetMemberPayoutTotal(Guid stokvelId, Guid memberId)
        {
            try
            {
                var result = await _stokvelService.GetMemberPayoutTotalAsync(stokvelId, memberId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while getting the member payout total." });
            }
        }

        /// <summary>
        /// Gets a member's balance
        /// </summary>
        /// <param name="stokvelId">The stokvel ID</param>
        /// <param name="memberId">The member ID</param>
        /// <returns>The member's balance</returns>
        [HttpGet("{stokvelId:guid}/members/{memberId:guid}/balance")]
        public async Task<ActionResult<decimal>> GetMemberBalance(Guid stokvelId, Guid memberId)
        {
            try
            {
                var result = await _stokvelService.GetMemberBalanceAsync(stokvelId, memberId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while getting the member balance." });
            }
        }

        /// <summary>
        /// Gets the active member count for a stokvel
        /// </summary>
        /// <param name="stokvelId">The stokvel ID</param>
        /// <returns>The active member count</returns>
        [HttpGet("{stokvelId:guid}/members/count")]
        public async Task<ActionResult<int>> GetActiveMemberCount(Guid stokvelId)
        {
            try
            {
                var result = await _stokvelService.GetActiveMemberCountAsync(stokvelId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while getting the member count." });
            }
        }

        /// <summary>
        /// Checks if a stokvel has minimum members
        /// </summary>
        /// <param name="stokvelId">The stokvel ID</param>
        /// <param name="minimumMembers">The minimum number of members</param>
        /// <returns>True if the stokvel has minimum members</returns>
        [HttpGet("{stokvelId:guid}/members/minimum")]
        public async Task<ActionResult<bool>> HasMinimumMembers(Guid stokvelId, [FromQuery] int minimumMembers = 5)
        {
            try
            {
                var result = await _stokvelService.HasMinimumMembersAsync(stokvelId, minimumMembers);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while checking minimum members." });
            }
        }

        /// <summary>
        /// Checks if a stokvel is full
        /// </summary>
        /// <param name="stokvelId">The stokvel ID</param>
        /// <returns>True if the stokvel is full</returns>
        [HttpGet("{stokvelId:guid}/full")]
        public async Task<ActionResult<bool>> IsFull(Guid stokvelId)
        {
            try
            {
                var result = await _stokvelService.IsFullAsync(stokvelId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while checking if the stokvel is full." });
            }
        }

        /// <summary>
        /// Gets members in rotation order for a stokvel
        /// </summary>
        /// <param name="stokvelId">The stokvel ID</param>
        /// <returns>The members in rotation order</returns>
        [HttpGet("{stokvelId:guid}/members/rotation")]
        public async Task<ActionResult<List<StokvelMemberResponse>>> GetMembersInRotationOrder(Guid stokvelId)
        {
            try
            {
                var result = await _stokvelService.GetMembersInRotationOrderAsync(stokvelId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while getting members in rotation order." });
            }
        }

        /// <summary>
        /// Gets detailed balance information for a member
        /// </summary>
        /// <param name="stokvelId">The stokvel ID</param>
        /// <param name="memberId">The member ID</param>
        /// <returns>The member's detailed balance information</returns>
        [HttpGet("{stokvelId:guid}/members/{memberId:guid}/balance-details")]
        public async Task<ActionResult<MemberBalanceResponse>> GetMemberBalanceDetails(Guid stokvelId, Guid memberId)
        {
            try
            {
                var result = await _stokvelService.GetMemberBalanceDetailsAsync(stokvelId, memberId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while getting the member balance details." });
            }
        }
    }
} 
