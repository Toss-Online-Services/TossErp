import 'package:equatable/equatable.dart';

enum SaleStatus { draft, completed, cancelled, refunded, partiallyRefunded }
enum SaleType { regular, returned, exchange }

class SaleItemEntity extends Equatable {
  final String? id;
  final int? productId;
  final String productName;
  final String? productSku;
  final int quantity;
  final int unitPrice; // Price in cents
  final int discount; // Discount amount in cents
  final int taxAmount; // Tax amount in cents
  final int lineTotal; // Total for this line in cents
  final String? notes;
  final Map<String, dynamic>? metadata;

  const SaleItemEntity({
    this.id,
    this.productId,
    required this.productName,
    this.productSku,
    required this.quantity,
    required this.unitPrice,
    this.discount = 0,
    this.taxAmount = 0,
    required this.lineTotal,
    this.notes,
    this.metadata,
  });

  SaleItemEntity copyWith({
    String? id,
    int? productId,
    String? productName,
    String? productSku,
    int? quantity,
    int? unitPrice,
    int? discount,
    int? taxAmount,
    int? lineTotal,
    String? notes,
    Map<String, dynamic>? metadata,
  }) {
    return SaleItemEntity(
      id: id ?? this.id,
      productId: productId ?? this.productId,
      productName: productName ?? this.productName,
      productSku: productSku ?? this.productSku,
      quantity: quantity ?? this.quantity,
      unitPrice: unitPrice ?? this.unitPrice,
      discount: discount ?? this.discount,
      taxAmount: taxAmount ?? this.taxAmount,
      lineTotal: lineTotal ?? this.lineTotal,
      notes: notes ?? this.notes,
      metadata: metadata ?? this.metadata,
    );
  }

  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'productId': productId,
      'productName': productName,
      'productSku': productSku,
      'quantity': quantity,
      'unitPrice': unitPrice,
      'discount': discount,
      'taxAmount': taxAmount,
      'lineTotal': lineTotal,
      'notes': notes,
      'metadata': metadata,
    };
  }

  factory SaleItemEntity.fromMap(Map<String, dynamic> map) {
    return SaleItemEntity(
      id: map['id'],
      productId: map['productId'],
      productName: map['productName'] ?? '',
      productSku: map['productSku'],
      quantity: map['quantity'] ?? 0,
      unitPrice: map['unitPrice'] ?? 0,
      discount: map['discount'] ?? 0,
      taxAmount: map['taxAmount'] ?? 0,
      lineTotal: map['lineTotal'] ?? 0,
      notes: map['notes'],
      metadata: map['metadata'],
    );
  }

  @override
  List<Object?> get props => [
        id,
        productId,
        productName,
        productSku,
        quantity,
        unitPrice,
        discount,
        taxAmount,
        lineTotal,
        notes,
        metadata,
      ];
}

class SaleEntity extends Equatable {
  final int? id;
  final String saleNumber;
  final SaleStatus status;
  final SaleType type;
  final int? customerId;
  final String? customerName;
  final String? customerPhone;
  final String? customerEmail;
  final List<SaleItemEntity> items;
  final int subtotal; // Sum of all line totals in cents
  final int discountAmount; // Total discount in cents
  final int taxAmount; // Total tax in cents
  final int totalAmount; // Final total in cents
  final int? warehouseId;
  final String? warehouseName;
  final int? cashierId;
  final String? cashierName;
  final String? posDeviceId;
  final String? posDeviceName;
  final DateTime saleDate;
  final DateTime createdAt;
  final DateTime? updatedAt;
  final String? notes;
  final String? receiptNumber;
  final bool isPrinted;
  final bool isEmailSent;
  final int? loyaltyPointsEarned;
  final int? loyaltyPointsRedeemed;
  final Map<String, dynamic>? metadata;

  const SaleEntity({
    this.id,
    required this.saleNumber,
    this.status = SaleStatus.draft,
    this.type = SaleType.regular,
    this.customerId,
    this.customerName,
    this.customerPhone,
    this.customerEmail,
    required this.items,
    required this.subtotal,
    this.discountAmount = 0,
    this.taxAmount = 0,
    required this.totalAmount,
    this.warehouseId,
    this.warehouseName,
    this.cashierId,
    this.cashierName,
    this.posDeviceId,
    this.posDeviceName,
    required this.saleDate,
    required this.createdAt,
    this.updatedAt,
    this.notes,
    this.receiptNumber,
    this.isPrinted = false,
    this.isEmailSent = false,
    this.loyaltyPointsEarned,
    this.loyaltyPointsRedeemed,
    this.metadata,
  });

  SaleEntity copyWith({
    int? id,
    String? saleNumber,
    SaleStatus? status,
    SaleType? type,
    int? customerId,
    String? customerName,
    String? customerPhone,
    String? customerEmail,
    List<SaleItemEntity>? items,
    int? subtotal,
    int? discountAmount,
    int? taxAmount,
    int? totalAmount,
    int? warehouseId,
    String? warehouseName,
    int? cashierId,
    String? cashierName,
    String? posDeviceId,
    String? posDeviceName,
    DateTime? saleDate,
    DateTime? createdAt,
    DateTime? updatedAt,
    String? notes,
    String? receiptNumber,
    bool? isPrinted,
    bool? isEmailSent,
    int? loyaltyPointsEarned,
    int? loyaltyPointsRedeemed,
    Map<String, dynamic>? metadata,
  }) {
    return SaleEntity(
      id: id ?? this.id,
      saleNumber: saleNumber ?? this.saleNumber,
      status: status ?? this.status,
      type: type ?? this.type,
      customerId: customerId ?? this.customerId,
      customerName: customerName ?? this.customerName,
      customerPhone: customerPhone ?? this.customerPhone,
      customerEmail: customerEmail ?? this.customerEmail,
      items: items ?? this.items,
      subtotal: subtotal ?? this.subtotal,
      discountAmount: discountAmount ?? this.discountAmount,
      taxAmount: taxAmount ?? this.taxAmount,
      totalAmount: totalAmount ?? this.totalAmount,
      warehouseId: warehouseId ?? this.warehouseId,
      warehouseName: warehouseName ?? this.warehouseName,
      cashierId: cashierId ?? this.cashierId,
      cashierName: cashierName ?? this.cashierName,
      posDeviceId: posDeviceId ?? this.posDeviceId,
      posDeviceName: posDeviceName ?? this.posDeviceName,
      saleDate: saleDate ?? this.saleDate,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
      notes: notes ?? this.notes,
      receiptNumber: receiptNumber ?? this.receiptNumber,
      isPrinted: isPrinted ?? this.isPrinted,
      isEmailSent: isEmailSent ?? this.isEmailSent,
      loyaltyPointsEarned: loyaltyPointsEarned ?? this.loyaltyPointsEarned,
      loyaltyPointsRedeemed: loyaltyPointsRedeemed ?? this.loyaltyPointsRedeemed,
      metadata: metadata ?? this.metadata,
    );
  }

  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'saleNumber': saleNumber,
      'status': status.name,
      'type': type.name,
      'customerId': customerId,
      'customerName': customerName,
      'customerPhone': customerPhone,
      'customerEmail': customerEmail,
      'items': items.map((item) => item.toMap()).toList(),
      'subtotal': subtotal,
      'discountAmount': discountAmount,
      'taxAmount': taxAmount,
      'totalAmount': totalAmount,
      'warehouseId': warehouseId,
      'warehouseName': warehouseName,
      'cashierId': cashierId,
      'cashierName': cashierName,
      'posDeviceId': posDeviceId,
      'posDeviceName': posDeviceName,
      'saleDate': saleDate.toIso8601String(),
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt?.toIso8601String(),
      'notes': notes,
      'receiptNumber': receiptNumber,
      'isPrinted': isPrinted,
      'isEmailSent': isEmailSent,
      'loyaltyPointsEarned': loyaltyPointsEarned,
      'loyaltyPointsRedeemed': loyaltyPointsRedeemed,
      'metadata': metadata,
    };
  }

  factory SaleEntity.fromMap(Map<String, dynamic> map) {
    return SaleEntity(
      id: map['id'],
      saleNumber: map['saleNumber'] ?? '',
      status: SaleStatus.values.firstWhere(
        (e) => e.name == map['status'],
        orElse: () => SaleStatus.draft,
      ),
      type: SaleType.values.firstWhere(
        (e) => e.name == map['type'],
        orElse: () => SaleType.regular,
      ),
      customerId: map['customerId'],
      customerName: map['customerName'],
      customerPhone: map['customerPhone'],
      customerEmail: map['customerEmail'],
      items: (map['items'] as List<dynamic>?)
              ?.map((item) => SaleItemEntity.fromMap(item))
              .toList() ??
          [],
      subtotal: map['subtotal'] ?? 0,
      discountAmount: map['discountAmount'] ?? 0,
      taxAmount: map['taxAmount'] ?? 0,
      totalAmount: map['totalAmount'] ?? 0,
      warehouseId: map['warehouseId'],
      warehouseName: map['warehouseName'],
      cashierId: map['cashierId'],
      cashierName: map['cashierName'],
      posDeviceId: map['posDeviceId'],
      posDeviceName: map['posDeviceName'],
      saleDate: DateTime.parse(map['saleDate']),
      createdAt: DateTime.parse(map['createdAt']),
      updatedAt: map['updatedAt'] != null ? DateTime.parse(map['updatedAt']) : null,
      notes: map['notes'],
      receiptNumber: map['receiptNumber'],
      isPrinted: map['isPrinted'] ?? false,
      isEmailSent: map['isEmailSent'] ?? false,
      loyaltyPointsEarned: map['loyaltyPointsEarned'],
      loyaltyPointsRedeemed: map['loyaltyPointsRedeemed'],
      metadata: map['metadata'],
    );
  }

  @override
  List<Object?> get props => [
        id,
        saleNumber,
        status,
        type,
        customerId,
        customerName,
        customerPhone,
        customerEmail,
        items,
        subtotal,
        discountAmount,
        taxAmount,
        totalAmount,
        warehouseId,
        warehouseName,
        cashierId,
        cashierName,
        posDeviceId,
        posDeviceName,
        saleDate,
        createdAt,
        updatedAt,
        notes,
        receiptNumber,
        isPrinted,
        isEmailSent,
        loyaltyPointsEarned,
        loyaltyPointsRedeemed,
        metadata,
      ];
}

