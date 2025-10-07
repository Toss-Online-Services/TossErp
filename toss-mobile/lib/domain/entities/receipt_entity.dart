import 'package:equatable/equatable.dart';

enum ReceiptType { sale, returned, refund, invoice, layaway, quote, giftCard, loyalty, voidTransaction }
enum ReceiptFormat { thermal, pos58, pos80, a4 }
enum DeliveryMethod { print, email, sms, whatsapp, all }
enum ReceiptStatus { pending, printed, emailed, sms, failed }

class ReceiptLineItem extends Equatable {
  final String? id;
  final String productName;
  final String? productSku;
  final String? sku; // Alias for productSku
  final int quantity;
  final double unitPrice;
  final double lineTotal;
  final double totalPrice; // Alias for lineTotal
  final double? discount;
  final double? tax;

  const ReceiptLineItem({
    this.id,
    required this.productName,
    this.productSku,
    this.sku,
    required this.quantity,
    required this.unitPrice,
    required this.lineTotal,
    required this.totalPrice,
    this.discount,
    this.tax,
  });

  @override
  List<Object?> get props => [id, productName, productSku, sku, quantity, unitPrice, lineTotal, totalPrice, discount, tax];
}

class ReceiptTotals extends Equatable {
  final double subtotal;
  final double totalDiscount;
  final double totalTax;
  final double total;
  final double amountPaid;
  final double change;
  final String? currency;

  const ReceiptTotals({
    required this.subtotal,
    required this.totalDiscount,
    required this.totalTax,
    required this.total,
    required this.amountPaid,
    required this.change,
    this.currency,
  });

  @override
  List<Object?> get props => [subtotal, totalDiscount, totalTax, total, amountPaid, change, currency];
}

class ReceiptPayment extends Equatable {
  final String method;
  final String? reference;
  final double amount;

  const ReceiptPayment({
    required this.method,
    this.reference,
    required this.amount,
  });

  @override
  List<Object?> get props => [method, reference, amount];
}

class ReceiptCustomer extends Equatable {
  final String? id;
  final String? name;
  final String? phone;
  final String? email;
  final String? address;
  final String? loyaltyNumber;
  final int? loyaltyPoints;

  const ReceiptCustomer({
    this.id,
    this.name,
    this.phone,
    this.email,
    this.address,
    this.loyaltyNumber,
    this.loyaltyPoints,
  });

  @override
  List<Object?> get props => [id, name, phone, email, address, loyaltyNumber, loyaltyPoints];
}

class ReceiptSettings extends Equatable {
  final String businessName;
  final String? businessAddress;
  final String? businessPhone;
  final String? businessEmail;
  final String? taxNumber;
  final String? footerMessage;
  final bool showBarcode;
  final int paperWidth;
  final bool printCustomerInfo;

  const ReceiptSettings({
    required this.businessName,
    this.businessAddress,
    this.businessPhone,
    this.businessEmail,
    this.taxNumber,
    this.footerMessage,
    this.showBarcode = true,
    this.paperWidth = 58,
    this.printCustomerInfo = true,
  });

  @override
  List<Object?> get props => [businessName, businessAddress, businessPhone, businessEmail, taxNumber, footerMessage, showBarcode, paperWidth, printCustomerInfo];
}

class ReceiptDelivery extends Equatable {
  final String? id;
  final String receiptId;
  final DeliveryMethod method;
  final ReceiptStatus status;
  final DateTime? sentAt;
  final String? errorMessage;

  const ReceiptDelivery({
    this.id,
    required this.receiptId,
    required this.method,
    required this.status,
    this.sentAt,
    this.errorMessage,
  });

  @override
  List<Object?> get props => [id, receiptId, method, status, sentAt, errorMessage];
}

class PrinterConfig extends Equatable {
  final String id;
  final String name;
  final String connectionType; // 'bluetooth', 'wifi', 'usb'
  final String type; // Alias for connectionType
  final String address;
  final bool isDefault;
  final Map<String, dynamic> settings;

  const PrinterConfig({
    required this.id,
    required this.name,
    required this.connectionType,
    required this.type,
    required this.address,
    this.isDefault = false,
    this.settings = const {},
  });

  @override
  List<Object?> get props => [id, name, connectionType, type, address, isDefault, settings];
}

class ReceiptEntity extends Equatable {
  final int? id;
  final String receiptNumber;
  final ReceiptType type;
  final ReceiptFormat format;
  final String? transactionId;
  final String? cashierId;
  final String? locationId;
  final String? originalReceiptId;
  final String? customerId;
  final bool isReprint;
  final Map<String, dynamic>? receiptData;
  final ReceiptSettings settings;
  final List<ReceiptLineItem> lineItems;
  final ReceiptTotals totals;
  final ReceiptPayment? payment;
  final ReceiptCustomer? customer;
  final DateTime receiptDate;
  final DateTime createdAt;
  final List<ReceiptDelivery> deliveries;

  const ReceiptEntity({
    this.id,
    required this.receiptNumber,
    this.type = ReceiptType.sale,
    this.format = ReceiptFormat.thermal,
    this.transactionId,
    this.cashierId,
    this.locationId,
    this.originalReceiptId,
    this.customerId,
    this.isReprint = false,
    this.receiptData,
    required this.settings,
    required this.lineItems,
    required this.totals,
    this.payment,
    this.customer,
    required this.receiptDate,
    required this.createdAt,
    this.deliveries = const [],
  });

  ReceiptEntity copyWith({
    int? id,
    String? receiptNumber,
    ReceiptType? type,
    ReceiptFormat? format,
    String? transactionId,
    String? cashierId,
    String? locationId,
    String? originalReceiptId,
    String? customerId,
    bool? isReprint,
    Map<String, dynamic>? receiptData,
    ReceiptSettings? settings,
    List<ReceiptLineItem>? lineItems,
    ReceiptTotals? totals,
    ReceiptPayment? payment,
    ReceiptCustomer? customer,
    DateTime? receiptDate,
    DateTime? createdAt,
    List<ReceiptDelivery>? deliveries,
  }) {
    return ReceiptEntity(
      id: id ?? this.id,
      receiptNumber: receiptNumber ?? this.receiptNumber,
      type: type ?? this.type,
      format: format ?? this.format,
      transactionId: transactionId ?? this.transactionId,
      cashierId: cashierId ?? this.cashierId,
      locationId: locationId ?? this.locationId,
      originalReceiptId: originalReceiptId ?? this.originalReceiptId,
      customerId: customerId ?? this.customerId,
      isReprint: isReprint ?? this.isReprint,
      receiptData: receiptData ?? this.receiptData,
      settings: settings ?? this.settings,
      lineItems: lineItems ?? this.lineItems,
      totals: totals ?? this.totals,
      payment: payment ?? this.payment,
      customer: customer ?? this.customer,
      receiptDate: receiptDate ?? this.receiptDate,
      createdAt: createdAt ?? this.createdAt,
      deliveries: deliveries ?? this.deliveries,
    );
  }

  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'receiptNumber': receiptNumber,
      'type': type.name,
      'format': format.name,
      'transactionId': transactionId,
      'cashierId': cashierId,
      'locationId': locationId,
      'originalReceiptId': originalReceiptId,
      'customerId': customerId,
      'isReprint': isReprint,
      'receiptData': receiptData,
      'settings': {
        'businessName': settings.businessName,
        'businessAddress': settings.businessAddress,
        'businessPhone': settings.businessPhone,
        'businessEmail': settings.businessEmail,
        'taxNumber': settings.taxNumber,
        'footerMessage': settings.footerMessage,
        'showBarcode': settings.showBarcode,
      },
      'lineItems': lineItems.map((item) => {
        'productName': item.productName,
        'productSku': item.productSku,
        'quantity': item.quantity,
        'unitPrice': item.unitPrice,
        'lineTotal': item.lineTotal,
        'discount': item.discount,
        'tax': item.tax,
      }).toList(),
      'totals': {
        'subtotal': totals.subtotal,
        'totalDiscount': totals.totalDiscount,
        'totalTax': totals.totalTax,
        'total': totals.total,
        'amountPaid': totals.amountPaid,
        'change': totals.change,
      },
      'payment': payment != null ? {
        'method': payment!.method,
        'reference': payment!.reference,
        'amount': payment!.amount,
      } : null,
      'customer': customer != null ? {
        'name': customer!.name,
        'phone': customer!.phone,
        'email': customer!.email,
        'address': customer!.address,
      } : null,
      'receiptDate': receiptDate.toIso8601String(),
      'createdAt': createdAt.toIso8601String(),
      'deliveries': deliveries.map((delivery) => {
        'receiptId': delivery.receiptId,
        'method': delivery.method.name,
        'status': delivery.status.name,
        'sentAt': delivery.sentAt?.toIso8601String(),
        'errorMessage': delivery.errorMessage,
      }).toList(),
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
      format: ReceiptFormat.values.firstWhere(
        (e) => e.name == map['format'],
        orElse: () => ReceiptFormat.thermal,
      ),
      transactionId: map['transactionId'],
      cashierId: map['cashierId'],
      locationId: map['locationId'],
      originalReceiptId: map['originalReceiptId'],
      customerId: map['customerId'],
      isReprint: map['isReprint'] ?? false,
      receiptData: map['receiptData'],
      settings: ReceiptSettings(
        businessName: map['settings']['businessName'] ?? '',
        businessAddress: map['settings']['businessAddress'],
        businessPhone: map['settings']['businessPhone'],
        businessEmail: map['settings']['businessEmail'],
        taxNumber: map['settings']['taxNumber'],
        footerMessage: map['settings']['footerMessage'],
        showBarcode: map['settings']['showBarcode'] ?? true,
      ),
      lineItems: (map['lineItems'] as List<dynamic>?)
          ?.map((item) => ReceiptLineItem(
                productName: item['productName'] ?? '',
                productSku: item['productSku'],
                quantity: item['quantity'] ?? 0,
                unitPrice: (item['unitPrice'] ?? 0.0).toDouble(),
                lineTotal: (item['lineTotal'] ?? 0.0).toDouble(),
                totalPrice: (item['lineTotal'] ?? 0.0).toDouble(),
                discount: item['discount']?.toDouble(),
                tax: item['tax']?.toDouble(),
              ))
          .toList() ?? [],
      totals: ReceiptTotals(
        subtotal: (map['totals']['subtotal'] ?? 0.0).toDouble(),
        totalDiscount: (map['totals']['totalDiscount'] ?? 0.0).toDouble(),
        totalTax: (map['totals']['totalTax'] ?? 0.0).toDouble(),
        total: (map['totals']['total'] ?? 0.0).toDouble(),
        amountPaid: (map['totals']['amountPaid'] ?? 0.0).toDouble(),
        change: (map['totals']['change'] ?? 0.0).toDouble(),
      ),
      payment: map['payment'] != null ? ReceiptPayment(
        method: map['payment']['method'] ?? '',
        reference: map['payment']['reference'],
        amount: (map['payment']['amount'] ?? 0.0).toDouble(),
      ) : null,
      customer: map['customer'] != null ? ReceiptCustomer(
        name: map['customer']['name'],
        phone: map['customer']['phone'],
        email: map['customer']['email'],
        address: map['customer']['address'],
      ) : null,
      receiptDate: DateTime.parse(map['receiptDate']),
      createdAt: DateTime.parse(map['createdAt']),
      deliveries: (map['deliveries'] as List<dynamic>?)
          ?.map((delivery) => ReceiptDelivery(
                receiptId: delivery['receiptId'] ?? '',
                method: DeliveryMethod.values.firstWhere(
                  (e) => e.name == delivery['method'],
                  orElse: () => DeliveryMethod.print,
                ),
                status: ReceiptStatus.values.firstWhere(
                  (e) => e.name == delivery['status'],
                  orElse: () => ReceiptStatus.pending,
                ),
                sentAt: delivery['sentAt'] != null ? DateTime.parse(delivery['sentAt']) : null,
                errorMessage: delivery['errorMessage'],
              ))
          .toList() ?? [],
    );
  }

  @override
  List<Object?> get props => [
    id,
    receiptNumber,
    type,
    format,
    transactionId,
    cashierId,
    locationId,
    originalReceiptId,
    customerId,
    isReprint,
    receiptData,
    settings,
    lineItems,
    totals,
    payment,
    customer,
    receiptDate,
    createdAt,
    deliveries,
  ];
}