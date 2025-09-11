import 'package:equatable/equatable.dart';

enum PurchaseOrderStatus { draft, pending, ordered, partiallyReceived, received, cancelled }

class PurchaseOrderEntity extends Equatable {
  final int? id;
  final String orderNumber;
  final int supplierId;
  final PurchaseOrderStatus status;
  final DateTime orderDate;
  final DateTime? expectedDate;
  final DateTime? receivedDate;
  final int totalAmount; // in cents
  final int? taxAmount; // in cents
  final int? discountAmount; // in cents
  final String? notes;
  final List<PurchaseOrderItemEntity> items;
  final String? createdById;
  final String? createdAt;
  final String? updatedAt;

  const PurchaseOrderEntity({
    this.id,
    required this.orderNumber,
    required this.supplierId,
    this.status = PurchaseOrderStatus.draft,
    required this.orderDate,
    this.expectedDate,
    this.receivedDate,
    required this.totalAmount,
    this.taxAmount,
    this.discountAmount,
    this.notes,
    this.items = const [],
    this.createdById,
    this.createdAt,
    this.updatedAt,
  });

  PurchaseOrderEntity copyWith({
    int? id,
    String? orderNumber,
    int? supplierId,
    PurchaseOrderStatus? status,
    DateTime? orderDate,
    DateTime? expectedDate,
    DateTime? receivedDate,
    int? totalAmount,
    int? taxAmount,
    int? discountAmount,
    String? notes,
    List<PurchaseOrderItemEntity>? items,
    String? createdById,
    String? createdAt,
    String? updatedAt,
  }) {
    return PurchaseOrderEntity(
      id: id ?? this.id,
      orderNumber: orderNumber ?? this.orderNumber,
      supplierId: supplierId ?? this.supplierId,
      status: status ?? this.status,
      orderDate: orderDate ?? this.orderDate,
      expectedDate: expectedDate ?? this.expectedDate,
      receivedDate: receivedDate ?? this.receivedDate,
      totalAmount: totalAmount ?? this.totalAmount,
      taxAmount: taxAmount ?? this.taxAmount,
      discountAmount: discountAmount ?? this.discountAmount,
      notes: notes ?? this.notes,
      items: items ?? this.items,
      createdById: createdById ?? this.createdById,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  int get netAmount => totalAmount - (taxAmount ?? 0) - (discountAmount ?? 0);
  bool get isOverdue => expectedDate != null && DateTime.now().isAfter(expectedDate!) && status != PurchaseOrderStatus.received;

  @override
  List<Object?> get props => [
    id,
    orderNumber,
    supplierId,
    status,
    orderDate,
    expectedDate,
    receivedDate,
    totalAmount,
    taxAmount,
    discountAmount,
    notes,
    items,
    createdById,
    createdAt,
    updatedAt,
  ];
}

class PurchaseOrderItemEntity extends Equatable {
  final int? id;
  final int purchaseOrderId;
  final int productId;
  final int quantity;
  final int unitPrice; // in cents
  final int totalPrice; // in cents
  final int quantityReceived;
  final String? notes;

  const PurchaseOrderItemEntity({
    this.id,
    required this.purchaseOrderId,
    required this.productId,
    required this.quantity,
    required this.unitPrice,
    required this.totalPrice,
    this.quantityReceived = 0,
    this.notes,
  });

  PurchaseOrderItemEntity copyWith({
    int? id,
    int? purchaseOrderId,
    int? productId,
    int? quantity,
    int? unitPrice,
    int? totalPrice,
    int? quantityReceived,
    String? notes,
  }) {
    return PurchaseOrderItemEntity(
      id: id ?? this.id,
      purchaseOrderId: purchaseOrderId ?? this.purchaseOrderId,
      productId: productId ?? this.productId,
      quantity: quantity ?? this.quantity,
      unitPrice: unitPrice ?? this.unitPrice,
      totalPrice: totalPrice ?? this.totalPrice,
      quantityReceived: quantityReceived ?? this.quantityReceived,
      notes: notes ?? this.notes,
    );
  }

  int get quantityPending => quantity - quantityReceived;
  bool get isFullyReceived => quantityReceived >= quantity;

  @override
  List<Object?> get props => [
    id,
    purchaseOrderId,
    productId,
    quantity,
    unitPrice,
    totalPrice,
    quantityReceived,
    notes,
  ];
}
