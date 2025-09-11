import 'package:equatable/equatable.dart';

enum InventoryValuationMethod { fifo, lifo, average }

class InventoryBatchEntity extends Equatable {
  final int? id;
  final int productId;
  final String? batchNumber;
  final String? lotNumber;
  final String? serialNumber;
  final int quantity;
  final int purchasePrice; // in cents
  final int sellingPrice; // in cents
  final String? supplierId;
  final DateTime? expiryDate;
  final DateTime? manufacturedDate;
  final DateTime receivedDate;
  final bool isActive;
  final String? createdById;
  final String? createdAt;
  final String? updatedAt;

  const InventoryBatchEntity({
    this.id,
    required this.productId,
    this.batchNumber,
    this.lotNumber,
    this.serialNumber,
    required this.quantity,
    required this.purchasePrice,
    required this.sellingPrice,
    this.supplierId,
    this.expiryDate,
    this.manufacturedDate,
    required this.receivedDate,
    this.isActive = true,
    this.createdById,
    this.createdAt,
    this.updatedAt,
  });

  InventoryBatchEntity copyWith({
    int? id,
    int? productId,
    String? batchNumber,
    String? lotNumber,
    String? serialNumber,
    int? quantity,
    int? purchasePrice,
    int? sellingPrice,
    String? supplierId,
    DateTime? expiryDate,
    DateTime? manufacturedDate,
    DateTime? receivedDate,
    bool? isActive,
    String? createdById,
    String? createdAt,
    String? updatedAt,
  }) {
    return InventoryBatchEntity(
      id: id ?? this.id,
      productId: productId ?? this.productId,
      batchNumber: batchNumber ?? this.batchNumber,
      lotNumber: lotNumber ?? this.lotNumber,
      serialNumber: serialNumber ?? this.serialNumber,
      quantity: quantity ?? this.quantity,
      purchasePrice: purchasePrice ?? this.purchasePrice,
      sellingPrice: sellingPrice ?? this.sellingPrice,
      supplierId: supplierId ?? this.supplierId,
      expiryDate: expiryDate ?? this.expiryDate,
      manufacturedDate: manufacturedDate ?? this.manufacturedDate,
      receivedDate: receivedDate ?? this.receivedDate,
      isActive: isActive ?? this.isActive,
      createdById: createdById ?? this.createdById,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  bool get isExpired => expiryDate != null && DateTime.now().isAfter(expiryDate!);
  
  bool get isExpiringSoon {
    if (expiryDate == null) return false;
    final daysUntilExpiry = expiryDate!.difference(DateTime.now()).inDays;
    return daysUntilExpiry <= 30 && daysUntilExpiry >= 0;
  }

  @override
  List<Object?> get props => [
    id,
    productId,
    batchNumber,
    lotNumber,
    serialNumber,
    quantity,
    purchasePrice,
    sellingPrice,
    supplierId,
    expiryDate,
    manufacturedDate,
    receivedDate,
    isActive,
    createdById,
    createdAt,
    updatedAt,
  ];
}
