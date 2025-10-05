import 'package:equatable/equatable.dart';

enum MovementType { sale, purchase, adjustment, transfer, returned, waste, production }
enum MovementReason { 
  sale, 
  purchase, 
  adjustment, 
  damaged, 
  expired, 
  theft, 
  transfer, 
  returned, 
  promotion, 
  inventoryCount,
  other 
}

class InventoryMovementEntity extends Equatable {
  final int? id;
  final int productId;
  final int? batchId;
  final MovementType type;
  final MovementReason reason;
  final int quantity;
  final int unitPrice; // in cents
  final int totalValue; // in cents
  final int? referenceId; // transaction ID, transfer ID, etc.
  final String? referenceType; // 'transaction', 'transfer', 'adjustment'
  final String? notes;
  final int? fromLocationId;
  final int? toLocationId;
  final String? createdById;
  final DateTime createdAt;
  final String? updatedAt;

  const InventoryMovementEntity({
    this.id,
    required this.productId,
    this.batchId,
    required this.type,
    required this.reason,
    required this.quantity,
    required this.unitPrice,
    required this.totalValue,
    this.referenceId,
    this.referenceType,
    this.notes,
    this.fromLocationId,
    this.toLocationId,
    this.createdById,
    required this.createdAt,
    this.updatedAt,
  });

  InventoryMovementEntity copyWith({
    int? id,
    int? productId,
    int? batchId,
    MovementType? type,
    MovementReason? reason,
    int? quantity,
    int? unitPrice,
    int? totalValue,
    int? referenceId,
    String? referenceType,
    String? notes,
    int? fromLocationId,
    int? toLocationId,
    String? createdById,
    DateTime? createdAt,
    String? updatedAt,
  }) {
    return InventoryMovementEntity(
      id: id ?? this.id,
      productId: productId ?? this.productId,
      batchId: batchId ?? this.batchId,
      type: type ?? this.type,
      reason: reason ?? this.reason,
      quantity: quantity ?? this.quantity,
      unitPrice: unitPrice ?? this.unitPrice,
      totalValue: totalValue ?? this.totalValue,
      referenceId: referenceId ?? this.referenceId,
      referenceType: referenceType ?? this.referenceType,
      notes: notes ?? this.notes,
      fromLocationId: fromLocationId ?? this.fromLocationId,
      toLocationId: toLocationId ?? this.toLocationId,
      createdById: createdById ?? this.createdById,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  bool get isIncoming => [MovementType.purchase, MovementType.returned, MovementType.adjustment].contains(type) && quantity > 0;
  bool get isOutgoing => [MovementType.sale, MovementType.transfer, MovementType.waste, MovementType.adjustment].contains(type) && quantity < 0;

  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'productId': productId,
      'batchId': batchId,
      'type': type.name,
      'reason': reason.name,
      'quantity': quantity,
      'unitPrice': unitPrice,
      'totalValue': totalValue,
      'referenceId': referenceId,
      'referenceType': referenceType,
      'notes': notes,
      'fromLocationId': fromLocationId,
      'toLocationId': toLocationId,
      'createdById': createdById,
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt,
    };
  }

  @override
  List<Object?> get props => [
    id,
    productId,
    batchId,
    type,
    reason,
    quantity,
    unitPrice,
    totalValue,
    referenceId,
    referenceType,
    notes,
    fromLocationId,
    toLocationId,
    createdById,
    createdAt,
    updatedAt,
  ];
}
