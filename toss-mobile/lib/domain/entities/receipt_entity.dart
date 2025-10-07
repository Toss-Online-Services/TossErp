import 'package:equatable/equatable.dart';
import 'sale_entity.dart';
import 'payment_entity.dart';

enum ReceiptType { sale, return, refund, invoice }

class ReceiptEntity extends Equatable {
  final int? id;
  final String receiptNumber;
  final ReceiptType type;
  final int? saleId;
  final SaleEntity? sale;
  final List<PaymentEntity>? payments;
  final String? companyName;
  final String? companyAddress;
  final String? companyPhone;
  final String? companyEmail;
  final String? companyTaxId;
  final String? companyLogoUrl;
  final String? customerName;
  final String? customerPhone;
  final String? customerEmail;
  final String? customerAddress;
  final String? cashierName;
  final String? storeName;
  final String? storeAddress;
  final DateTime receiptDate;
  final String? footerMessage;
  final String? qrCodeData;
  final bool isPrinted;
  final bool isEmailed;
  final DateTime createdAt;
  final Map<String, dynamic>? metadata;

  const ReceiptEntity({
    this.id,
    required this.receiptNumber,
    this.type = ReceiptType.sale,
    this.saleId,
    this.sale,
    this.payments,
    this.companyName,
    this.companyAddress,
    this.companyPhone,
    this.companyEmail,
    this.companyTaxId,
    this.companyLogoUrl,
    this.customerName,
    this.customerPhone,
    this.customerEmail,
    this.customerAddress,
    this.cashierName,
    this.storeName,
    this.storeAddress,
    required this.receiptDate,
    this.footerMessage,
    this.qrCodeData,
    this.isPrinted = false,
    this.isEmailed = false,
    required this.createdAt,
    this.metadata,
  });

  ReceiptEntity copyWith({
    int? id,
    String? receiptNumber,
    ReceiptType? type,
    int? saleId,
    SaleEntity? sale,
    List<PaymentEntity>? payments,
    String? companyName,
    String? companyAddress,
    String? companyPhone,
    String? companyEmail,
    String? companyTaxId,
    String? companyLogoUrl,
    String? customerName,
    String? customerPhone,
    String? customerEmail,
    String? customerAddress,
    String? cashierName,
    String? storeName,
    String? storeAddress,
    DateTime? receiptDate,
    String? footerMessage,
    String? qrCodeData,
    bool? isPrinted,
    bool? isEmailed,
    DateTime? createdAt,
    Map<String, dynamic>? metadata,
  }) {
    return ReceiptEntity(
      id: id ?? this.id,
      receiptNumber: receiptNumber ?? this.receiptNumber,
      type: type ?? this.type,
      saleId: saleId ?? this.saleId,
      sale: sale ?? this.sale,
      payments: payments ?? this.payments,
      companyName: companyName ?? this.companyName,
      companyAddress: companyAddress ?? this.companyAddress,
      companyPhone: companyPhone ?? this.companyPhone,
      companyEmail: companyEmail ?? this.companyEmail,
      companyTaxId: companyTaxId ?? this.companyTaxId,
      companyLogoUrl: companyLogoUrl ?? this.companyLogoUrl,
      customerName: customerName ?? this.customerName,
      customerPhone: customerPhone ?? this.customerPhone,
      customerEmail: customerEmail ?? this.customerEmail,
      customerAddress: customerAddress ?? this.customerAddress,
      cashierName: cashierName ?? this.cashierName,
      storeName: storeName ?? this.storeName,
      storeAddress: storeAddress ?? this.storeAddress,
      receiptDate: receiptDate ?? this.receiptDate,
      footerMessage: footerMessage ?? this.footerMessage,
      qrCodeData: qrCodeData ?? this.qrCodeData,
      isPrinted: isPrinted ?? this.isPrinted,
      isEmailed: isEmailed ?? this.isEmailed,
      createdAt: createdAt ?? this.createdAt,
      metadata: metadata ?? this.metadata,
    );
  }

  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'receiptNumber': receiptNumber,
      'type': type.name,
      'saleId': saleId,
      'sale': sale?.toMap(),
      'payments': payments?.map((p) => p.toMap()).toList(),
      'companyName': companyName,
      'companyAddress': companyAddress,
      'companyPhone': companyPhone,
      'companyEmail': companyEmail,
      'companyTaxId': companyTaxId,
      'companyLogoUrl': companyLogoUrl,
      'customerName': customerName,
      'customerPhone': customerPhone,
      'customerEmail': customerEmail,
      'customerAddress': customerAddress,
      'cashierName': cashierName,
      'storeName': storeName,
      'storeAddress': storeAddress,
      'receiptDate': receiptDate.toIso8601String(),
      'footerMessage': footerMessage,
      'qrCodeData': qrCodeData,
      'isPrinted': isPrinted,
      'isEmailed': isEmailed,
      'createdAt': createdAt.toIso8601String(),
      'metadata': metadata,
    };
  }

  factory ReceiptEntity.fromMap(Map<String, dynamic> map) {
    return ReceiptEntity(
      id: map['id'],
      receiptNumber: map['receiptNumber'] ?? '',
      type: ReceiptType.values.firstWhere(
        (e) => e.name == map['type'],
        orElse: () => ReceiptType.sale,
      ),
      saleId: map['saleId'],
      sale: map['sale'] != null ? SaleEntity.fromMap(map['sale']) : null,
      payments: (map['payments'] as List<dynamic>?)
          ?.map((p) => PaymentEntity.fromMap(p))
          .toList(),
      companyName: map['companyName'],
      companyAddress: map['companyAddress'],
      companyPhone: map['companyPhone'],
      companyEmail: map['companyEmail'],
      companyTaxId: map['companyTaxId'],
      companyLogoUrl: map['companyLogoUrl'],
      customerName: map['customerName'],
      customerPhone: map['customerPhone'],
      customerEmail: map['customerEmail'],
      customerAddress: map['customerAddress'],
      cashierName: map['cashierName'],
      storeName: map['storeName'],
      storeAddress: map['storeAddress'],
      receiptDate: DateTime.parse(map['receiptDate']),
      footerMessage: map['footerMessage'],
      qrCodeData: map['qrCodeData'],
      isPrinted: map['isPrinted'] ?? false,
      isEmailed: map['isEmailed'] ?? false,
      createdAt: DateTime.parse(map['createdAt']),
      metadata: map['metadata'],
    );
  }

  @override
  List<Object?> get props => [
        id,
        receiptNumber,
        type,
        saleId,
        sale,
        payments,
        companyName,
        companyAddress,
        companyPhone,
        companyEmail,
        companyTaxId,
        companyLogoUrl,
        customerName,
        customerPhone,
        customerEmail,
        customerAddress,
        cashierName,
        storeName,
        storeAddress,
        receiptDate,
        footerMessage,
        qrCodeData,
        isPrinted,
        isEmailed,
        createdAt,
        metadata,
      ];
}
