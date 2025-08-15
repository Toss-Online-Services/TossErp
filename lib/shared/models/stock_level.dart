import 'package:json_annotation/json_annotation.dart';

part 'stock_level.g.dart';

@JsonSerializable()
class StockLevel {
  final String id;
  final String itemId;
  final String warehouseId;
  final String? binId;
  final double quantity;
  final double reservedQuantity;
  final double unitCost;
  final DateTime? lastMovementDate;
  final DateTime? lastUpdated;

  const StockLevel({
    required this.id,
    required this.itemId,
    required this.warehouseId,
    this.binId,
    this.quantity = 0,
    this.reservedQuantity = 0,
    this.unitCost = 0,
    this.lastMovementDate,
    this.lastUpdated,
  });

  factory StockLevel.fromJson(Map<String, dynamic> json) => _$StockLevelFromJson(json);
  Map<String, dynamic> toJson() => _$StockLevelToJson(this);

  factory StockLevel.fromMap(Map<String, dynamic> map) {
    return StockLevel(
      id: map['id'] ?? '',
      itemId: map['item_id'] ?? '',
      warehouseId: map['warehouse_id'] ?? '',
      binId: map['bin_id'],
      quantity: map['quantity']?.toDouble() ?? 0.0,
      reservedQuantity: map['reserved_quantity']?.toDouble() ?? 0.0,
      unitCost: map['unit_cost']?.toDouble() ?? 0.0,
      lastMovementDate: map['last_movement_date'] != null ? DateTime.parse(map['last_movement_date']) : null,
      lastUpdated: map['last_updated'] != null ? DateTime.parse(map['last_updated']) : null,
    );
  }

  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'item_id': itemId,
      'warehouse_id': warehouseId,
      'bin_id': binId,
      'quantity': quantity,
      'reserved_quantity': reservedQuantity,
      'unit_cost': unitCost,
      'last_movement_date': lastMovementDate?.toIso8601String(),
      'last_updated': lastUpdated?.toIso8601String(),
    };
  }

  double get availableQuantity => quantity - reservedQuantity;

  StockLevel copyWith({
    String? id,
    String? itemId,
    String? warehouseId,
    String? binId,
    double? quantity,
    double? reservedQuantity,
    double? unitCost,
    DateTime? lastMovementDate,
    DateTime? lastUpdated,
  }) {
    return StockLevel(
      id: id ?? this.id,
      itemId: itemId ?? this.itemId,
      warehouseId: warehouseId ?? this.warehouseId,
      binId: binId ?? this.binId,
      quantity: quantity ?? this.quantity,
      reservedQuantity: reservedQuantity ?? this.reservedQuantity,
      unitCost: unitCost ?? this.unitCost,
      lastMovementDate: lastMovementDate ?? this.lastMovementDate,
      lastUpdated: lastUpdated ?? this.lastUpdated,
    );
  }

  @override
  String toString() {
    return 'StockLevel(id: $id, itemId: $itemId, warehouseId: $warehouseId, quantity: $quantity, availableQuantity: $availableQuantity)';
  }

  @override
  bool operator ==(Object other) {
    if (identical(this, other)) return true;
    return other is StockLevel && other.id == id;
  }

  @override
  int get hashCode {
    return id.hashCode;
  }
}
