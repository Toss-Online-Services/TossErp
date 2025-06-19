using Microsoft.AspNetCore.Mvc;
using TossErp.POS.API.DTOs;
using TossErp.POS.Domain.AggregatesModel.SaleAggregate;
using TossErp.POS.Domain.Enums;
using TossErp.POS.Infrastructure.Repositories;
using TossErp.Domain.SeedWork;
using TossErp.Shared.DTOs;
using TossErp.POS.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace TossErp.POS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SalesController : ControllerBase
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SalesController> _logger;
        private readonly ISalesService _salesService;

        public SalesController(ISaleRepository saleRepository, IUnitOfWork unitOfWork, ILogger<SalesController> logger, ISalesService salesService)
        {
            _saleRepository = saleRepository ?? throw new ArgumentNullException(nameof(saleRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _salesService = salesService;
        }

        [HttpGet]
        public async Task<ActionResult<SaleListDto>> GetSales(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] SaleStatus? status = null,
            [FromQuery] Guid? customerId = null,
            [FromQuery] Guid? cashierId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var sales = await _saleRepository.GetAllAsync();

                // Apply filters
                if (status.HasValue)
                {
                    sales = sales.Where(s => s.Status == status.Value);
                }

                if (customerId.HasValue)
                {
                    sales = sales.Where(s => s.CustomerId == customerId.Value);
                }

                if (cashierId.HasValue)
                {
                    sales = sales.Where(s => s.CashierId == cashierId.Value);
                }

                if (startDate.HasValue)
                {
                    sales = sales.Where(s => s.SaleDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    sales = sales.Where(s => s.SaleDate <= endDate.Value);
                }

                var totalCount = sales.Count();
                var totalSalesAmount = sales.Where(s => s.Status == SaleStatus.Completed).Sum(s => s.TotalAmount);

                var pagedSales = sales
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(MapToDto)
                    .ToList();

                var result = new SaleListDto
                {
                    Sales = pagedSales,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalSalesAmount = totalSalesAmount
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sales");
                return StatusCode(500, "An error occurred while retrieving sales");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SaleDto>> GetSale(Guid id)
        {
            try
            {
                var sale = await _saleRepository.GetByIdAsync(id);
                if (sale == null)
                {
                    return NotFound($"Sale with ID {id} not found");
                }

                return Ok(MapToDto(sale));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sale with ID {SaleId}", id);
                return StatusCode(500, "An error occurred while retrieving the sale");
            }
        }

        [HttpGet("number/{saleNumber}")]
        public async Task<ActionResult<SaleDto>> GetSaleByNumber(string saleNumber)
        {
            try
            {
                var sale = await _saleRepository.GetBySaleNumberAsync(saleNumber);
                if (sale == null)
                {
                    return NotFound($"Sale with number '{saleNumber}' not found");
                }

                return Ok(MapToDto(sale));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sale with number {SaleNumber}", saleNumber);
                return StatusCode(500, "An error occurred while retrieving the sale");
            }
        }

        [HttpPost]
        public async Task<ActionResult<SaleDto>> CreateSale([FromBody] CreateSaleDto createSaleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!createSaleDto.Items.Any())
                {
                    return BadRequest("Sale must have at least one item");
                }

                var sale = new Sale(
                    Guid.NewGuid().ToString("N")[..8].ToUpper(), // Generate sale number
                    createSaleDto.CustomerId,
                    createSaleDto.CashierId,
                    createSaleDto.SaleType
                );

                // Add items to sale
                foreach (var itemDto in createSaleDto.Items)
                {
                    sale.AddItem(
                        itemDto.ItemId,
                        itemDto.ItemName ?? "Unknown Item", // Add item name
                        itemDto.Quantity,
                        itemDto.UnitPrice,
                        itemDto.DiscountPercentage
                    );
                }

                // Set notes if provided
                if (!string.IsNullOrEmpty(createSaleDto.Notes))
                {
                    sale.UpdateNotes(createSaleDto.Notes);
                }

                await _saleRepository.AddAsync(sale);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Created sale with ID {SaleId}", sale.Id);

                return CreatedAtAction(nameof(GetSale), new { id = sale.Id }, MapToDto(sale));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sale");
                return StatusCode(500, "An error occurred while creating the sale");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SaleDto>> UpdateSale(Guid id, [FromBody] UpdateSaleDto updateSaleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var sale = await _saleRepository.GetByIdAsync(id);
                if (sale == null)
                {
                    return NotFound($"Sale with ID {id} not found");
                }

                if (sale.Status != SaleStatus.Draft)
                {
                    return BadRequest("Only draft sales can be updated");
                }

                // Clear existing items and add new ones
                sale.ClearItems();

                foreach (var itemDto in updateSaleDto.Items)
                {
                    sale.AddItem(
                        itemDto.ItemId,
                        itemDto.ItemName ?? "Unknown Item", // Add item name
                        itemDto.Quantity,
                        itemDto.UnitPrice,
                        itemDto.DiscountPercentage
                    );
                }

                // Update notes if provided
                if (!string.IsNullOrEmpty(updateSaleDto.Notes))
                {
                    sale.UpdateNotes(updateSaleDto.Notes);
                }

                _saleRepository.Update(sale);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Updated sale with ID {SaleId}", id);

                return Ok(MapToDto(sale));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating sale with ID {SaleId}", id);
                return StatusCode(500, "An error occurred while updating the sale");
            }
        }

        [HttpPost("{id}/complete")]
        public async Task<ActionResult<SaleDto>> CompleteSale(Guid id, [FromBody] CompleteSaleDto completeSaleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var sale = await _saleRepository.GetByIdAsync(id);
                if (sale == null)
                {
                    return NotFound($"Sale with ID {id} not found");
                }

                if (sale.Status != SaleStatus.Draft)
                {
                    return BadRequest("Only draft sales can be completed");
                }

                sale.Complete(
                    completeSaleDto.PaymentMethod,
                    completeSaleDto.AmountPaid,
                    completeSaleDto.ReferenceNumber
                );

                _saleRepository.Update(sale);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Completed sale with ID {SaleId}", id);

                return Ok(MapToDto(sale));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing sale with ID {SaleId}", id);
                return StatusCode(500, "An error occurred while completing the sale");
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<SaleDto>> CancelSale(Guid id, [FromBody] CancelSaleDto cancelSaleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var sale = await _saleRepository.GetByIdAsync(id);
                if (sale == null)
                {
                    return NotFound($"Sale with ID {id} not found");
                }

                if (sale.Status == SaleStatus.Cancelled)
                {
                    return BadRequest("Sale is already cancelled");
                }

                sale.Cancel(cancelSaleDto.CancellationReason);

                _saleRepository.Update(sale);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Cancelled sale with ID {SaleId}", id);

                return Ok(MapToDto(sale));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling sale with ID {SaleId}", id);
                return StatusCode(500, "An error occurred while cancelling the sale");
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<List<SaleDto>>> GetSalesByCustomer(Guid customerId)
        {
            try
            {
                var sales = await _saleRepository.GetSalesByCustomerAsync(customerId);
                var saleDtos = sales.Select(MapToDto).ToList();

                return Ok(saleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sales for customer {CustomerId}", customerId);
                return StatusCode(500, "An error occurred while retrieving customer sales");
            }
        }

        [HttpGet("cashier/{cashierId}")]
        public async Task<ActionResult<List<SaleDto>>> GetSalesByCashier(Guid cashierId)
        {
            try
            {
                var sales = await _saleRepository.GetSalesByCashierAsync(cashierId);
                var saleDtos = sales.Select(MapToDto).ToList();

                return Ok(saleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sales for cashier {CashierId}", cashierId);
                return StatusCode(500, "An error occurred while retrieving cashier sales");
            }
        }

        [HttpGet("completed")]
        public async Task<ActionResult<List<SaleDto>>> GetCompletedSales()
        {
            try
            {
                var sales = await _saleRepository.GetCompletedSalesAsync();
                var saleDtos = sales.Select(MapToDto).ToList();

                return Ok(saleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving completed sales");
                return StatusCode(500, "An error occurred while retrieving completed sales");
            }
        }

        [HttpGet("reports/daily")]
        public async Task<ActionResult<object>> GetDailySalesReport([FromQuery] DateTime date)
        {
            try
            {
                var startDate = date.Date;
                var endDate = startDate.AddDays(1).AddSeconds(-1);

                var sales = await _saleRepository.GetSalesByDateRangeAsync(startDate, endDate);
                var completedSales = sales.Where(s => s.Status == SaleStatus.Completed).ToList();

                var report = new
                {
                    Date = date.Date,
                    TotalSales = completedSales.Count,
                    TotalAmount = completedSales.Sum(s => s.TotalAmount),
                    AverageTicketValue = completedSales.Any() ? completedSales.Average(s => s.TotalAmount) : 0,
                    Sales = completedSales.Select(MapToDto).ToList()
                };

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating daily sales report for {Date}", date);
                return StatusCode(500, "An error occurred while generating the report");
            }
        }

        [HttpPost("create-sale")]
        public async Task<IActionResult> CreateSale([FromBody] CreateSaleDto request)
        {
            try
            {
                var sale = await _salesService.CreateSaleAsync(request);
                return CreatedAtAction(nameof(GetSale), new { id = sale.Id }, sale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sale");
                return BadRequest(new { message = "Failed to create sale", error = ex.Message });
            }
        }

        [HttpGet("daily-summary")]
        public async Task<IActionResult> GetDailySummary([FromQuery] DateTime date)
        {
            try
            {
                var summary = await _salesService.GetDailySummaryAsync(date);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving daily summary for {Date}", date);
                return StatusCode(500, new { message = "Failed to retrieve daily summary", error = ex.Message });
            }
        }

        [HttpGet("reports/sales")]
        public async Task<IActionResult> GetSalesReport([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                var report = await _salesService.GetSalesReportAsync(fromDate, toDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating sales report");
                return StatusCode(500, new { message = "Failed to generate sales report", error = ex.Message });
            }
        }

        private static SaleDto MapToDto(Sale sale)
        {
            return new SaleDto
            {
                Id = sale.Id,
                SaleNumber = sale.SaleNumber,
                CustomerId = sale.CustomerId,
                CashierId = sale.CashierId,
                Status = sale.Status,
                SaleType = sale.SaleType,
                SubTotal = sale.SubTotal,
                TaxAmount = sale.TaxAmount,
                DiscountAmount = sale.DiscountAmount,
                TotalAmount = sale.TotalAmount,
                AmountPaid = sale.AmountPaid,
                ChangeAmount = sale.ChangeAmount,
                PaymentMethod = sale.PaymentMethod,
                ReferenceNumber = sale.ReferenceNumber,
                Notes = sale.Notes,
                SaleDate = sale.SaleDate,
                CompletedAt = sale.CompletedAt,
                CancelledAt = sale.CancelledAt,
                CancellationReason = sale.CancellationReason,
                Items = sale.SaleItems.Select(si => new SaleItemDto
                {
                    Id = si.Id,
                    ItemId = si.ItemId,
                    ItemName = si.ItemName,
                    ItemCode = si.ItemCode,
                    Quantity = si.Quantity,
                    UnitPrice = si.UnitPrice,
                    DiscountAmount = si.DiscountAmount,
                    DiscountPercentage = si.DiscountPercentage ?? 0,
                    TotalAmount = si.TotalAmount
                }).ToList(),
                Payments = sale.Payments.Select(sp => new SalePaymentDto
                {
                    Id = sp.Id,
                    PaymentMethod = sp.PaymentMethod,
                    Amount = sp.Amount,
                    ReferenceNumber = sp.ReferenceNumber,
                    PaymentDate = sp.PaymentDate
                }).ToList(),
                CustomerName = "Customer Name", // TODO: Get from customer service
                CashierName = "Cashier Name"    // TODO: Get from user service
            };
        }
    }
} 
