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
    public class TownshipEnterpriseController : ControllerBase
    {
        private readonly ITownshipEnterpriseService _townshipEnterpriseService;

        public TownshipEnterpriseController(ITownshipEnterpriseService townshipEnterpriseService)
        {
            _townshipEnterpriseService = townshipEnterpriseService ?? throw new ArgumentNullException(nameof(townshipEnterpriseService));
        }

        /// <summary>
        /// Creates a new township enterprise
        /// </summary>
        /// <param name="request">The enterprise creation request</param>
        /// <returns>The created enterprise details</returns>
        [HttpPost]
        public async Task<ActionResult<TownshipEnterpriseResponse>> CreateTownshipEnterprise([FromBody] CreateTownshipEnterpriseRequest request)
        {
            try
            {
                var result = await _townshipEnterpriseService.CreateTownshipEnterpriseAsync(request);
                return CreatedAtAction(nameof(GetTownshipEnterprise), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while creating the township enterprise." });
            }
        }

        /// <summary>
        /// Gets a township enterprise by ID
        /// </summary>
        /// <param name="id">The enterprise ID</param>
        /// <returns>The enterprise details</returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TownshipEnterpriseResponse>> GetTownshipEnterprise(Guid id)
        {
            try
            {
                var result = await _townshipEnterpriseService.GetTownshipEnterpriseByIdAsync(id);
                if (result == null)
                    return NotFound(new { error = "Township enterprise not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving the township enterprise." });
            }
        }

        /// <summary>
        /// Updates a township enterprise
        /// </summary>
        /// <param name="request">The enterprise update request</param>
        /// <returns>The updated enterprise details</returns>
        [HttpPut]
        public async Task<ActionResult<TownshipEnterpriseResponse>> UpdateTownshipEnterprise([FromBody] UpdateTownshipEnterpriseRequest request)
        {
            try
            {
                var result = await _townshipEnterpriseService.UpdateTownshipEnterpriseAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the township enterprise." });
            }
        }

        /// <summary>
        /// Registers a township enterprise
        /// </summary>
        /// <param name="request">The registration request</param>
        /// <returns>The registered enterprise details</returns>
        [HttpPost("register")]
        public async Task<ActionResult<TownshipEnterpriseResponse>> RegisterTownshipEnterprise([FromBody] RegisterTownshipEnterpriseRequest request)
        {
            try
            {
                var result = await _townshipEnterpriseService.RegisterTownshipEnterpriseAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while registering the township enterprise." });
            }
        }

        /// <summary>
        /// Activates a township enterprise
        /// </summary>
        /// <param name="id">The enterprise ID</param>
        /// <returns>The activated enterprise details</returns>
        [HttpPost("{id:guid}/activate")]
        public async Task<ActionResult<TownshipEnterpriseResponse>> ActivateTownshipEnterprise(Guid id)
        {
            try
            {
                var result = await _townshipEnterpriseService.ActivateTownshipEnterpriseAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while activating the township enterprise." });
            }
        }

        /// <summary>
        /// Deactivates a township enterprise
        /// </summary>
        /// <param name="id">The enterprise ID</param>
        /// <returns>The deactivated enterprise details</returns>
        [HttpPost("{id:guid}/deactivate")]
        public async Task<ActionResult<TownshipEnterpriseResponse>> DeactivateTownshipEnterprise(Guid id)
        {
            try
            {
                var result = await _townshipEnterpriseService.DeactivateTownshipEnterpriseAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deactivating the township enterprise." });
            }
        }

        /// <summary>
        /// Gets a list of township enterprises with filtering and pagination
        /// </summary>
        /// <param name="filter">The filter criteria</param>
        /// <returns>The list of enterprises</returns>
        [HttpGet]
        public async Task<ActionResult<TownshipEnterpriseListResponse>> GetTownshipEnterprises([FromQuery] TownshipEnterpriseFilterRequest filter)
        {
            try
            {
                var result = await _townshipEnterpriseService.GetTownshipEnterprisesAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving the township enterprises." });
            }
        }

        /// <summary>
        /// Adds a license to a township enterprise
        /// </summary>
        /// <param name="enterpriseId">The enterprise ID</param>
        /// <param name="licenseType">The license type</param>
        /// <param name="licenseNumber">The license number</param>
        /// <param name="issueDate">The issue date</param>
        /// <param name="expiryDate">The expiry date</param>
        /// <param name="issuingAuthority">The issuing authority</param>
        /// <param name="notes">Additional notes</param>
        /// <returns>The added license details</returns>
        [HttpPost("{enterpriseId:guid}/licenses")]
        public async Task<ActionResult<BusinessLicenseResponse>> AddLicense(
            Guid enterpriseId,
            [FromQuery] string licenseType,
            [FromQuery] string licenseNumber,
            [FromQuery] DateTime issueDate,
            [FromQuery] DateTime expiryDate,
            [FromQuery] string? issuingAuthority = null,
            [FromQuery] string? notes = null)
        {
            try
            {
                var result = await _townshipEnterpriseService.AddLicenseAsync(enterpriseId, licenseType, licenseNumber, issueDate, expiryDate, issuingAuthority, notes);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while adding the license." });
            }
        }

        /// <summary>
        /// Adds a document to a township enterprise
        /// </summary>
        /// <param name="enterpriseId">The enterprise ID</param>
        /// <param name="documentType">The document type</param>
        /// <param name="documentName">The document name</param>
        /// <param name="documentUrl">The document URL</param>
        /// <param name="description">The document description</param>
        /// <param name="fileSize">The file size</param>
        /// <param name="fileType">The file type</param>
        /// <returns>The added document details</returns>
        [HttpPost("{enterpriseId:guid}/documents")]
        public async Task<ActionResult<BusinessDocumentResponse>> AddDocument(
            Guid enterpriseId,
            [FromQuery] string documentType,
            [FromQuery] string documentName,
            [FromQuery] string? documentUrl = null,
            [FromQuery] string? description = null,
            [FromQuery] long? fileSize = null,
            [FromQuery] string? fileType = null)
        {
            try
            {
                var result = await _townshipEnterpriseService.AddDocumentAsync(enterpriseId, documentType, documentName, documentUrl, description, fileSize, fileType);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while adding the document." });
            }
        }

        /// <summary>
        /// Adds a contact to a township enterprise
        /// </summary>
        /// <param name="enterpriseId">The enterprise ID</param>
        /// <param name="contactName">The contact name</param>
        /// <param name="contactNumber">The contact number</param>
        /// <param name="emailAddress">The email address</param>
        /// <param name="relationship">The relationship</param>
        /// <param name="notes">Additional notes</param>
        /// <returns>The added contact details</returns>
        [HttpPost("{enterpriseId:guid}/contacts")]
        public async Task<ActionResult<BusinessContactResponse>> AddContact(
            Guid enterpriseId,
            [FromQuery] string contactName,
            [FromQuery] string contactNumber,
            [FromQuery] string? emailAddress = null,
            [FromQuery] string? relationship = null,
            [FromQuery] string? notes = null)
        {
            try
            {
                var result = await _townshipEnterpriseService.AddContactAsync(enterpriseId, contactName, contactNumber, emailAddress, relationship, notes);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while adding the contact." });
            }
        }

        /// <summary>
        /// Removes a contact from a township enterprise
        /// </summary>
        /// <param name="enterpriseId">The enterprise ID</param>
        /// <param name="contactId">The contact ID</param>
        /// <returns>No content on success</returns>
        [HttpDelete("{enterpriseId:guid}/contacts/{contactId:guid}")]
        public async Task<ActionResult> RemoveContact(Guid enterpriseId, Guid contactId)
        {
            try
            {
                await _townshipEnterpriseService.RemoveContactAsync(enterpriseId, contactId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while removing the contact." });
            }
        }

        /// <summary>
        /// Checks if a township enterprise has a valid license
        /// </summary>
        /// <param name="enterpriseId">The enterprise ID</param>
        /// <param name="licenseType">The license type</param>
        /// <returns>True if the license is valid</returns>
        [HttpGet("{enterpriseId:guid}/licenses/{licenseType}/valid")]
        public async Task<ActionResult<bool>> HasValidLicense(Guid enterpriseId, string licenseType)
        {
            try
            {
                var result = await _townshipEnterpriseService.HasValidLicenseAsync(enterpriseId, licenseType);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while checking the license validity." });
            }
        }

        /// <summary>
        /// Checks if a township enterprise is in a specific township
        /// </summary>
        /// <param name="enterpriseId">The enterprise ID</param>
        /// <param name="townshipName">The township name</param>
        /// <returns>True if the enterprise is in the township</returns>
        [HttpGet("{enterpriseId:guid}/township/{townshipName}")]
        public async Task<ActionResult<bool>> IsInTownship(Guid enterpriseId, string townshipName)
        {
            try
            {
                var result = await _townshipEnterpriseService.IsInTownshipAsync(enterpriseId, townshipName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while checking the township location." });
            }
        }

        /// <summary>
        /// Checks if a township enterprise is in a specific province
        /// </summary>
        /// <param name="enterpriseId">The enterprise ID</param>
        /// <param name="provinceName">The province name</param>
        /// <returns>True if the enterprise is in the province</returns>
        [HttpGet("{enterpriseId:guid}/province/{provinceName}")]
        public async Task<ActionResult<bool>> IsInProvince(Guid enterpriseId, string provinceName)
        {
            try
            {
                var result = await _townshipEnterpriseService.IsInProvinceAsync(enterpriseId, provinceName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while checking the province location." });
            }
        }
    }
} 
