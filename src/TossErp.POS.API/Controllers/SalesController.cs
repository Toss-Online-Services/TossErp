using Microsoft.AspNetCore.Mvc;
using TossErp.POS.Domain.AggregatesModel.SaleAggregate;
using TossErp.POS.Domain.Enums;
using TossErp.POS.Infrastructure.Repositories;
using TossErp.Domain.SeedWork;
using TossErp.POS.API.Services;
using Microsoft.AspNetCore.Authorization;
using TossErp.POS.Application.DTOs;
using TossErp.Shared.DTOs;

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
        public async Task<ActionResult<CommonResponseDto<List<SaleListDto>>>> GetSales(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] SaleStatus? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var sales = await _salesService.GetSalesAsync(startDate, endDate, page, pageSize);
                var saleListDtos = sales.Select(s => new SaleListDto
                {
                    Id = (int)s.Id.GetHashCode(), // Convert Guid to int hash
                    SaleNumber = s.SaleNumber,
                    SaleDate = s.CreatedAt,
                    TotalAmount = s.TotalAmount,
                    Status = s.Status,
                    CustomerName = s.CustomerName,
                    PaymentMethod = s.PaymentMethod,
                    ItemCount = s.Items.Count
                }).ToList();
                
                return Ok(new CommonResponseDto<List<SaleListDto>>
                {
                    Success = true,
                    Data = saleListDtos,
                    Message = "Sales retrieved successfully"
                });
            }
            catch (Exception)
            {
                return BadRequest(new CommonResponseDto<List<SaleListDto>>
                {
                    Success = false,
                    Message = "Error retrieving sales"
                });
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

        [HttpPost("create-sale")]
        public async Task<IActionResult> CreateSale([FromBody] TossErp.POS.API.DTOs.CreateSaleDto request)
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
                        itemDto.DiscountAmount
                    );
                }

                // Update notes if provided
                if (!string.IsNullOrEmpty(updateSaleDto.CustomerName))
                {
                    sale.UpdateNotes(updateSaleDto.CustomerName);
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
                    (PaymentMethod)Enum.Parse(typeof(PaymentMethod), completeSaleDto.PaymentMethod),
                    completeSaleDto.AmountPaid,
                    completeSaleDto.ReceiptEmail ?? ""
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

                if (sale.Status != SaleStatus.Draft)
                {
                    return BadRequest("Only draft sales can be cancelled");
                }

                sale.Cancel(cancelSaleDto.Reason);

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
                var sales = await _saleRepository.GetByCustomerIdAsync(customerId);
                return Ok(sales.Select(MapToDto).ToList());
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
                var sales = await _saleRepository.GetByCashierIdAsync(cashierId);
                return Ok(sales.Select(MapToDto).ToList());
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
                return Ok(sales.Select(MapToDto).ToList());
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
                var sales = await _saleRepository.GetSalesByDateAsync(date);
                var totalSales = sales.Count;
                var totalRevenue = sales.Where(s => s.Status == SaleStatus.Completed).Sum(s => s.TotalAmount);
                var totalItems = sales.Sum(s => s.Items.Count);

                return Ok(new
                {
                    Date = date,
                    TotalSales = totalSales,
                    TotalRevenue = totalRevenue,
                    TotalItems = totalItems,
                    Sales = sales.Select(MapToDto).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving daily sales report for date {Date}", date);
                return StatusCode(500, "An error occurred while retrieving the daily sales report");
            }
        }

        [HttpGet("daily-summary")]
        public async Task<IActionResult> GetDailySummary([FromQuery] DateTime date)
        {
            try
            {
                var sales = await _saleRepository.GetSalesByDateAsync(date);
                var summary = new DailySummaryDto
                {
                    Date = date,
                    TotalSales = sales.Count,
                    TotalRevenue = sales.Where(s => s.Status == SaleStatus.Completed).Sum(s => s.TotalAmount),
                    TotalTax = sales.Sum(s => s.TaxAmount),
                    TotalDiscount = sales.Sum(s => s.DiscountAmount),
                    ItemsSold = sales.Sum(s => s.Items.Count)
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving daily summary for date {Date}", date);
                return StatusCode(500, "An error occurred while retrieving the daily summary");
            }
        }

        [HttpGet("reports/sales")]
        public async Task<IActionResult> GetSalesReport([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                var sales = await _saleRepository.GetSalesByDateRangeAsync(fromDate, toDate);
                var report = new SalesReportDto
                {
                    StartDate = fromDate,
                    EndDate = toDate,
                    TotalSales = sales.Count,
                    TotalRevenue = sales.Where(s => s.Status == SaleStatus.Completed).Sum(s => s.TotalAmount),
                    TotalTax = sales.Sum(s => s.TaxAmount),
                    TotalDiscount = sales.Sum(s => s.DiscountAmount),
                    ItemsSold = sales.Sum(s => s.Items.Count)
                };

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sales report from {FromDate} to {ToDate}", fromDate, toDate);
                return StatusCode(500, "An error occurred while retrieving the sales report");
            }
        }

        private static SaleDto MapToDto(Sale sale)
        {
            return new SaleDto
            {
                Id = sale.Id,
                SaleNumber = sale.SaleNumber,
                SaleDate = sale.SaleDate,
                TotalAmount = sale.TotalAmount,
                TaxAmount = sale.TaxAmount,
                DiscountAmount = sale.DiscountAmount,
                FinalAmount = sale.FinalAmount,
                Status = sale.Status.ToString(),
                CustomerName = sale.CustomerName,
                CustomerPhone = sale.CustomerPhone,
                PaymentMethod = sale.PaymentMethod.ToString(),
                Items = sale.Items.Select(item => new SaleItemDto
                {
                    Id = item.Id,
                    SaleId = item.SaleId,
                    ItemId = item.ItemId,
                    ItemName = item.ItemName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    DiscountAmount = item.DiscountAmount,
                    FinalPrice = item.FinalPrice
                }).ToList()
            };
        }
    }
} 
