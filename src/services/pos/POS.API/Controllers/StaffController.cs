using Microsoft.AspNetCore.Mvc;
using POS.Domain.AggregatesModel.StaffAggregate;
using POS.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StaffController : ControllerBase
{
    private readonly IStaffRepository _staffRepository;

    public StaffController(IStaffRepository staffRepository)
    {
        _staffRepository = staffRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Staff>>> GetStaff()
    {
        var staff = await _staffRepository.GetAllAsync();
        return Ok(staff);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Staff>> GetStaffMember(string id)
    {
        var staffMember = await _staffRepository.GetByIdAsync(id);
        if (staffMember == null)
        {
            return NotFound();
        }
        return Ok(staffMember);
    }

    [HttpPost]
    public async Task<ActionResult<Staff>> CreateStaffMember(Staff staff)
    {
        await _staffRepository.AddAsync(staff);
        return CreatedAtAction(nameof(GetStaffMember), new { id = staff.Id }, staff);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStaffMember(string id, Staff staff)
    {
        if (id != staff.Id)
        {
            return BadRequest();
        }

        var exists = await _staffRepository.ExistsAsync(id);
        if (!exists)
        {
            return NotFound();
        }

        await _staffRepository.UpdateAsync(staff);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStaffMember(string id)
    {
        var staff = await _staffRepository.GetByIdAsync(id);
        if (staff == null)
        {
            return NotFound();
        }

        await _staffRepository.DeleteAsync(staff);
        return NoContent();
    }
} 
