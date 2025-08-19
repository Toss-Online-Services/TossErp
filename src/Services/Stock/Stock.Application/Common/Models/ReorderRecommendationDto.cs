namespace TossErp.Stock.Application.Common.Models;

public record ReorderRecommendationDto(
    Guid ItemId,
    string ItemCode,
    decimal CurrentStock,
    decimal RecommendedReorderQty,
    string Reason);


