using Microsoft.AspNetCore.Mvc;
using POS.Domain.AggregatesModel.StoreAggregate;
using POS.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoresController : ControllerBase
{
    private readonly IStoreRepository _storeRepository;

    public StoresController(IStoreRepository storeRepository)
    {
        _storeRepository = storeRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Store>>> GetStores()
    {
        var stores = await _storeRepository.GetAllAsync();
        return Ok(stores);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Store>> GetStore(string id)
    {
        var store = await _storeRepository.GetByIdAsync(id);
        if (store == null)
        {
            return NotFound();
        }
        return Ok(store);
    }

    [HttpPost]
    public async Task<ActionResult<Store>> CreateStore(Store store)
    {
        await _storeRepository.AddAsync(store);
        return CreatedAtAction(nameof(GetStore), new { id = store.Id }, store);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStore(string id, Store store)
    {
        if (id != store.Id)
        {
            return BadRequest();
        }

        var exists = await _storeRepository.ExistsAsync(id);
        if (!exists)
        {
            return NotFound();
        }

        await _storeRepository.UpdateAsync(store);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStore(string id)
    {
        var store = await _storeRepository.GetByIdAsync(id);
        if (store == null)
        {
            return NotFound();
        }

        await _storeRepository.DeleteAsync(store);
        return NoContent();
    }
} 
