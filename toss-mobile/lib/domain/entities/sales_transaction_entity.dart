import 'package:equatable/equatable.dart';
import 'payment_entity.dart';

enum SalesTransactionStatus { 
  draft, 
  hold, 
  active, 
  completed, 
  cancelled, 
  refunded, 
  partiallyRefunded 
}

enum SalesTransactionType { sale, returned, refund, exchange, layaway }

class SalesTransactionEntity extends Equatable {
  final int? id;
  final String transactionNumber;
  final SalesTransactionType type;
  final SalesTransactionStatus status;
  final String? customerId;
  final int subtotal; // in cents
  final int taxAmount; // in cents
  final int discountAmount; // in cents
  final int loyaltyPointsUsed;
  final int loyaltyPointsEarned;
  final int total; // in cents
  final int amountPaid; // in cents
  final int changeAmount; // in cents
  final List<SalesTransactionItemEntity> items;
  final List<PaymentEntity> payments;
  final String? notes;
  final Map<String, dynamic>? metadata;
  final int? originalTransactionId; // For returns/refunds
  final DateTime createdAt;
  final String? createdById;
  final String? updatedAt;

  const SalesTransactionEntity({
    this.id,
    required this.transactionNumber,
    this.type = SalesTransactionType.sale,
    this.status = SalesTransactionStatus.draft,
    this.customerId,
    required this.subtotal,
    this.taxAmount = 0,
    this.discountAmount = 0,
    this.loyaltyPointsUsed = 0,
    this.loyaltyPointsEarned = 0,
    required this.total,
    this.amountPaid = 0,
    this.changeAmount = 0,
    this.items = const [],
    this.payments = const [],
    this.notes,
    this.metadata,
    this.originalTransactionId,
    required this.createdAt,
    this.createdById,
    this.updatedAt,
  });

  SalesTransactionEntity copyWith({
    int? id,
    String? transactionNumber,
    SalesTransactionType? type,
    SalesTransactionStatus? status,
    String? customerId,
    int? subtotal,
    int? taxAmount,
    int? discountAmount,
    int? loyaltyPointsUsed,
    int? loyaltyPointsEarned,
    int? total,
    int? amountPaid,
    int? changeAmount,
    List<SalesTransactionItemEntity>? items,
    List<PaymentEntity>? payments,
    String? notes,
    Map<String, dynamic>? metadata,
    int? originalTransactionId,
    DateTime? createdAt,
    String? createdById,
    String? updatedAt,
  }) {
    return SalesTransactionEntity(
      id: id ?? this.id,
      transactionNumber: transactionNumber ?? this.transactionNumber,
      type: type ?? this.type,
      status: status ?? this.status,
      customerId: customerId ?? this.customerId,
      subtotal: subtotal ?? this.subtotal,
      taxAmount: taxAmount ?? this.taxAmount,
      discountAmount: discountAmount ?? this.discountAmount,
      loyaltyPointsUsed: loyaltyPointsUsed ?? this.loyaltyPointsUsed,
      loyaltyPointsEarned: loyaltyPointsEarned ?? this.loyaltyPointsEarned,
      total: total ?? this.total,
      amountPaid: amountPaid ?? this.amountPaid,
      changeAmount: changeAmount ?? this.changeAmount,
      items: items ?? this.items,
      payments: payments ?? this.payments,
      notes: notes ?? this.notes,
      metadata: metadata ?? this.metadata,
      originalTransactionId: originalTransactionId ?? this.originalTransactionId,
      createdAt: createdAt ?? this.createdAt,
      createdById: createdById ?? this.createdById,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  // Helper methods
  bool get isCompleted => status == SalesTransactionStatus.completed;
  bool get isOnHold => status == SalesTransactionStatus.hold;
  bool get isDraft => status == SalesTransactionStatus.draft;
  bool get isCancelled => status == SalesTransactionStatus.cancelled;
  bool get hasCustomer => customerId != null;
  
  int get itemCount => items.fold(0, (sum, item) => sum + item.quantity);
  int get remainingAmount => total - amountPaid;
  bool get isFullyPaid => amountPaid >= total;
  bool get hasPayments => payments.isNotEmpty;
  
  double get totalTaxPercentage => subtotal > 0 ? (taxAmount / subtotal) * 100 : 0;
  double get totalDiscountPercentage => subtotal > 0 ? (discountAmount / subtotal) * 100 : 0;

  @override
  List<Object?> get props => [
    id,
    transactionNumber,
    type,
    status,
    customerId,
    subtotal,
    taxAmount,
    discountAmount,
    loyaltyPointsUsed,
    loyaltyPointsEarned,
    total,
    amountPaid,
    changeAmount,
    items,
    payments,
    notes,
    metadata,
    originalTransactionId,
    createdAt,
    createdById,
    updatedAt,
  ];
}

class SalesTransactionItemEntity extends Equatable {
  final int? id;
  final int transactionId;
  final int productId;
  final int? variantId;
  final String productName;
  final String? variantName;
  final int quantity;
  final int unitPrice; // in cents
  final int totalPrice; // in cents
  final int discountAmount; // in cents
  final String? notes;
  final Map<String, dynamic>? metadata;

  const SalesTransactionItemEntity({
    this.id,
    required this.transactionId,
    required this.productId,
    this.variantId,
    required this.productName,
    this.variantName,
    required this.quantity,
    required this.unitPrice,
    required this.totalPrice,
    this.discountAmount = 0,
    this.notes,
    this.metadata,
  });

  SalesTransactionItemEntity copyWith({
    int? id,
    int? transactionId,
    int? productId,
    int? variantId,
    String? productName,
    String? variantName,
    int? quantity,
    int? unitPrice,
    int? totalPrice,
    int? discountAmount,
    String? notes,
    Map<String, dynamic>? metadata,
  }) {
    return SalesTransactionItemEntity(
      id: id ?? this.id,
      transactionId: transactionId ?? this.transactionId,
      productId: productId ?? this.productId,
      variantId: variantId ?? this.variantId,
      productName: productName ?? this.productName,
      variantName: variantName ?? this.variantName,
      quantity: quantity ?? this.quantity,
      unitPrice: unitPrice ?? this.unitPrice,
      totalPrice: totalPrice ?? this.totalPrice,
      discountAmount: discountAmount ?? this.discountAmount,
      notes: notes ?? this.notes,
      metadata: metadata ?? this.metadata,
    );
  }

  int get netPrice => totalPrice - discountAmount;
  String get displayName => variantName != null ? '$productName ($variantName)' : productName;

  @override
  List<Object?> get props => [
    id,
    transactionId,
    productId,
    variantId,
    productName,
    variantName,
    quantity,
    unitPrice,
    totalPrice,
    discountAmount,
    notes,
    metadata,
  ];
}
