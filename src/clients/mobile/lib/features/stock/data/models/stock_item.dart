import 'package:freezed_annotation/freezed_annotation.dart';

part 'stock_item.freezed.dart';
part 'stock_item.g.dart';

@freezed
class StockItem with _$StockItem {
  const factory StockItem({
    required String id,
    required String name,
    required String sku,
    required String description,
    required double price,
    required double costPrice,
    required int quantity,
    required int minQuantity,
    required String category,
    required String status,
    String? imageUrl,
    String? location,
    String? barcode,
    Map<String, dynamic>? metadata,
    required DateTime createdAt,
    required DateTime updatedAt,
  }) = _StockItem;

  factory StockItem.fromJson(Map<String, Object?> json) => _$StockItemFromJson(json);
}

@freezed
class StockOverview with _$StockOverview {
  const factory StockOverview({
    required int totalItems,
    required int lowStockItems,
    required int outOfStockItems,
    required double totalValue,
    required int totalCategories,
    required List<CategorySummary> categorySummary,
  }) = _StockOverview;

  factory StockOverview.fromJson(Map<String, Object?> json) => _$StockOverviewFromJson(json);
}

@freezed
class CategorySummary with _$CategorySummary {
  const factory CategorySummary({
    required String category,
    required int itemCount,
    required double totalValue,
  }) = _CategorySummary;

  factory CategorySummary.fromJson(Map<String, Object?> json) => _$CategorySummaryFromJson(json);
}

@freezed
class StockMovement with _$StockMovement {
  const factory StockMovement({
    required String id,
    required String itemId,
    required String itemName,
    required String type, // 'IN' or 'OUT'
    required int quantity,
    required String reason,
    String? reference,
    String? notes,
    required DateTime date,
    required String userId,
    required DateTime createdAt,
  }) = _StockMovement;

  factory StockMovement.fromJson(Map<String, Object?> json) => _$StockMovementFromJson(json);
}

@freezed
class CreateStockItemRequest with _$CreateStockItemRequest {
  const factory CreateStockItemRequest({
    required String name,
    required String sku,
    required String description,
    required double price,
    required double costPrice,
    required int quantity,
    required int minQuantity,
    required String category,
    String? imageUrl,
    String? location,
    String? barcode,
    Map<String, dynamic>? metadata,
  }) = _CreateStockItemRequest;

  factory CreateStockItemRequest.fromJson(Map<String, Object?> json) => _$CreateStockItemRequestFromJson(json);
}

@freezed
class StockAdjustmentRequest with _$StockAdjustmentRequest {
  const factory StockAdjustmentRequest({
    required String itemId,
    required String type, // 'IN' or 'OUT'
    required int quantity,
    required String reason,
    String? reference,
    String? notes,
  }) = _StockAdjustmentRequest;

  factory StockAdjustmentRequest.fromJson(Map<String, Object?> json) => _$StockAdjustmentRequestFromJson(json);
}

enum StockStatus {
  @JsonValue('in-stock')
  inStock,
  @JsonValue('low-stock')
  lowStock,
  @JsonValue('out-of-stock')
  outOfStock,
}

enum MovementType {
  @JsonValue('IN')
  stockIn,
  @JsonValue('OUT')
  stockOut,
}

extension StockStatusExtension on StockStatus {
  String get displayName {
    switch (this) {
      case StockStatus.inStock:
        return 'In Stock';
      case StockStatus.lowStock:
        return 'Low Stock';
      case StockStatus.outOfStock:
        return 'Out of Stock';
    }
  }

  String get statusCode {
    switch (this) {
      case StockStatus.inStock:
        return 'in-stock';
      case StockStatus.lowStock:
        return 'low-stock';
      case StockStatus.outOfStock:
        return 'out-of-stock';
    }
  }
}

extension MovementTypeExtension on MovementType {
  String get displayName {
    switch (this) {
      case MovementType.stockIn:
        return 'Stock In';
      case MovementType.stockOut:
        return 'Stock Out';
    }
  }

  String get code {
    switch (this) {
      case MovementType.stockIn:
        return 'IN';
      case MovementType.stockOut:
        return 'OUT';
    }
  }
}

