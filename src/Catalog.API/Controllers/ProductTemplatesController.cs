using Catalog.API.Services;
using Catalog.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductTemplatesController : ControllerBase
{
    private readonly IProductTemplateService _productTemplateService;

    public ProductTemplatesController(IProductTemplateService productTemplateService)
    {
        _productTemplateService = productTemplateService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductTemplate>>> GetProductTemplates(
        [FromQuery] bool showHidden = false,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = int.MaxValue)
    {
        var templates = await _productTemplateService.GetAllProductTemplatesAsync(showHidden, pageIndex, pageSize);
        return Ok(templates);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductTemplate>> GetProductTemplate(int id)
    {
        var template = await _productTemplateService.GetProductTemplateByIdAsync(id);
        if (template == null)
        {
            return NotFound();
        }
        return Ok(template);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductTemplate>>> SearchProductTemplates(
        [FromQuery] string name,
        [FromQuery] bool showHidden = false,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = int.MaxValue)
    {
        var templates = await _productTemplateService.GetProductTemplatesByNameAsync(name, showHidden, pageIndex, pageSize);
        return Ok(templates);
    }

    [HttpGet("{id}/products")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsByTemplate(
        int id,
        [FromQuery] bool showHidden = false,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = int.MaxValue)
    {
        var products = await _productTemplateService.GetProductsByTemplateIdAsync(id, showHidden, pageIndex, pageSize);
        return Ok(products);
    }

    [HttpGet("{id}/products/count")]
    public async Task<ActionResult<int>> GetProductCountByTemplate(
        int id,
        [FromQuery] bool showHidden = false)
    {
        var count = await _productTemplateService.GetProductCountByTemplateIdAsync(id, showHidden);
        return Ok(count);
    }

    [HttpPost]
    public async Task<ActionResult<ProductTemplate>> CreateProductTemplate(ProductTemplate template)
    {
        var createdTemplate = await _productTemplateService.InsertProductTemplateAsync(template);
        return CreatedAtAction(nameof(GetProductTemplate), new { id = createdTemplate.Id }, createdTemplate);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProductTemplate(int id, ProductTemplate template)
    {
        if (id != template.Id)
        {
            return BadRequest();
        }

        var existingTemplate = await _productTemplateService.GetProductTemplateByIdAsync(id);
        if (existingTemplate == null)
        {
            return NotFound();
        }

        await _productTemplateService.UpdateProductTemplateAsync(template);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProductTemplate(int id)
    {
        var template = await _productTemplateService.GetProductTemplateByIdAsync(id);
        if (template == null)
        {
            return NotFound();
        }

        await _productTemplateService.DeleteProductTemplateAsync(template);
        return NoContent();
    }

    [HttpDelete("batch")]
    public async Task<IActionResult> DeleteProductTemplates([FromBody] IList<ProductTemplate> templates)
    {
        await _productTemplateService.DeleteProductTemplatesAsync(templates);
        return NoContent();
    }
} 
