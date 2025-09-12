import 'package:equatable/equatable.dart';

enum ReceiptType { sale, refund, layaway, quote, giftCard, loyalty, voidTransaction }
enum ReceiptFormat { thermal, a4, pos58, pos80 }
enum ReceiptStatus { pending, printed, emailed, sms, failed }
enum DeliveryMethod { print, email, sms, whatsapp, all }

class ReceiptEntity extends Equatable {
  final String id;
  final String receiptNumber;
  final ReceiptType type;
  final ReceiptFormat format;
  final String transactionId;
  final String? customerId;
  final String cashierId;
  final String locationId;
  final Map<String, dynamic> receiptData; // Transaction data for receipt
  final List<ReceiptLineItem> lineItems;
  final ReceiptTotals totals;
  final ReceiptCustomer? customer;
  final ReceiptPayment? payment;
  final ReceiptSettings settings;
  final DateTime createdAt;
  final DateTime? printedAt;
  final List<ReceiptDelivery> deliveries; // Track all delivery attempts
  final Map<String, dynamic>? customFields;
  final String? notes;
  final bool isReprint;
  final String? originalReceiptId;

  const ReceiptEntity({
    required this.id,
    required this.receiptNumber,
    required this.type,
    this.format = ReceiptFormat.thermal,
    required this.transactionId,
    this.customerId,
    required this.cashierId,
    required this.locationId,
    required this.receiptData,
    required this.lineItems,
    required this.totals,
    this.customer,
    this.payment,
    required this.settings,
    required this.createdAt,
    this.printedAt,
    this.deliveries = const [],
    this.customFields,
    this.notes,
    this.isReprint = false,
    this.originalReceiptId,
  });

  @override
  List<Object?> get props => [
    id,
    receiptNumber,
    type,
    format,
    transactionId,
    customerId,
    cashierId,
    locationId,
    receiptData,
    lineItems,
    totals,
    customer,
    payment,
    settings,
    createdAt,
    printedAt,
    deliveries,
    customFields,
    notes,
    isReprint,
    originalReceiptId,
  ];
}

class ReceiptLineItem extends Equatable {
  final String id;
  final String productId;
  final String productName;
  final String? sku;
  final int quantity;
  final double unitPrice;
  final double totalPrice;
  final double? discount;
  final double? tax;
  final Map<String, dynamic>? metadata;

  const ReceiptLineItem({
    required this.id,
    required this.productId,
    required this.productName,
    this.sku,
    required this.quantity,
    required this.unitPrice,
    required this.totalPrice,
    this.discount,
    this.tax,
    this.metadata,
  });

  @override
  List<Object?> get props => [
    id,
    productId,
    productName,
    sku,
    quantity,
    unitPrice,
    totalPrice,
    discount,
    tax,
    metadata,
  ];
}

class ReceiptTotals extends Equatable {
  final double subtotal;
  final double totalDiscount;
  final double totalTax;
  final double total;
  final double amountPaid;
  final double change;
  final String currency;

  const ReceiptTotals({
    required this.subtotal,
    required this.totalDiscount,
    required this.totalTax,
    required this.total,
    required this.amountPaid,
    required this.change,
    this.currency = 'GHS',
  });

  @override
  List<Object?> get props => [
    subtotal,
    totalDiscount,
    totalTax,
    total,
    amountPaid,
    change,
    currency,
  ];
}

class ReceiptCustomer extends Equatable {
  final String id;
  final String? name;
  final String? phone;
  final String? email;
  final String? loyaltyNumber;
  final int? loyaltyPoints;
  final String? address;

  const ReceiptCustomer({
    required this.id,
    this.name,
    this.phone,
    this.email,
    this.loyaltyNumber,
    this.loyaltyPoints,
    this.address,
  });

  @override
  List<Object?> get props => [
    id,
    name,
    phone,
    email,
    loyaltyNumber,
    loyaltyPoints,
    address,
  ];
}

class ReceiptPayment extends Equatable {
  final String method;
  final double amount;
  final String? reference;
  final String? authCode;
  final String? gatewayTransactionId;

  const ReceiptPayment({
    required this.method,
    required this.amount,
    this.reference,
    this.authCode,
    this.gatewayTransactionId,
  });

  @override
  List<Object?> get props => [
    method,
    amount,
    reference,
    authCode,
    gatewayTransactionId,
  ];
}

class ReceiptSettings extends Equatable {
  final String businessName;
  final String? businessAddress;
  final String? businessPhone;
  final String? businessEmail;
  final String? businessWebsite;
  final String? taxNumber;
  final String? logoPath;
  final bool showLogo;
  final bool showBarcode;
  final bool showQrCode;
  final String? footerMessage;
  final String? headerMessage;
  final Map<String, dynamic>? customFields;
  final int paperWidth; // in mm
  final String fontFamily;
  final int fontSize;
  final bool printCustomerInfo;
  final bool printItemDetails;
  final bool printTaxDetails;
  final bool printPaymentDetails;

  const ReceiptSettings({
    required this.businessName,
    this.businessAddress,
    this.businessPhone,
    this.businessEmail,
    this.businessWebsite,
    this.taxNumber,
    this.logoPath,
    this.showLogo = false,
    this.showBarcode = true,
    this.showQrCode = false,
    this.footerMessage,
    this.headerMessage,
    this.customFields,
    this.paperWidth = 58,
    this.fontFamily = 'Monospace',
    this.fontSize = 12,
    this.printCustomerInfo = true,
    this.printItemDetails = true,
    this.printTaxDetails = true,
    this.printPaymentDetails = true,
  });

  @override
  List<Object?> get props => [
    businessName,
    businessAddress,
    businessPhone,
    businessEmail,
    businessWebsite,
    taxNumber,
    logoPath,
    showLogo,
    showBarcode,
    showQrCode,
    footerMessage,
    headerMessage,
    customFields,
    paperWidth,
    fontFamily,
    fontSize,
    printCustomerInfo,
    printItemDetails,
    printTaxDetails,
    printPaymentDetails,
  ];
}

class ReceiptDelivery extends Equatable {
  final String id;
  final DeliveryMethod method;
  final ReceiptStatus status;
  final String? destination; // email, phone, printer name
  final DateTime attemptedAt;
  final DateTime? completedAt;
  final String? errorMessage;
  final Map<String, dynamic>? metadata;

  const ReceiptDelivery({
    required this.id,
    required this.method,
    required this.status,
    this.destination,
    required this.attemptedAt,
    this.completedAt,
    this.errorMessage,
    this.metadata,
  });

  @override
  List<Object?> get props => [
    id,
    method,
    status,
    destination,
    attemptedAt,
    completedAt,
    errorMessage,
    metadata,
  ];
}

// Printer Configuration Entity
class PrinterConfig extends Equatable {
  final String id;
  final String name;
  final String type; // 'thermal', 'network', 'bluetooth', 'usb'
  final String connectionString; // IP address, MAC address, or device path
  final int paperWidth; // in mm (58, 80, etc.)
  final ReceiptFormat defaultFormat;
  final bool isDefault;
  final bool isActive;
  final Map<String, dynamic> settings; // Printer-specific settings
  final DateTime createdAt;
  final DateTime? lastUsedAt;

  const PrinterConfig({
    required this.id,
    required this.name,
    required this.type,
    required this.connectionString,
    this.paperWidth = 58,
    this.defaultFormat = ReceiptFormat.thermal,
    this.isDefault = false,
    this.isActive = true,
    this.settings = const {},
    required this.createdAt,
    this.lastUsedAt,
  });

  @override
  List<Object?> get props => [
    id,
    name,
    type,
    connectionString,
    paperWidth,
    defaultFormat,
    isDefault,
    isActive,
    settings,
    createdAt,
    lastUsedAt,
  ];
}

// Receipt Template Entity
class ReceiptTemplate extends Equatable {
  final String id;
  final String name;
  final ReceiptType type;
  final ReceiptFormat format;
  final String template; // Template content with placeholders
  final Map<String, dynamic> settings;
  final bool isDefault;
  final bool isActive;
  final DateTime createdAt;
  final DateTime? updatedAt;

  const ReceiptTemplate({
    required this.id,
    required this.name,
    required this.type,
    required this.format,
    required this.template,
    this.settings = const {},
    this.isDefault = false,
    this.isActive = true,
    required this.createdAt,
    this.updatedAt,
  });

  @override
  List<Object?> get props => [
    id,
    name,
    type,
    format,
    template,
    settings,
    isDefault,
    isActive,
    createdAt,
    updatedAt,
  ];
}
