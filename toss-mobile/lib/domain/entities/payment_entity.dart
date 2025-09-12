import 'package:equatable/equatable.dart';

enum PaymentMethod { 
  cash, 
  card, 
  mobileWallet, 
  qrPayment, 
  nfc, 
  bankTransfer, 
  storeCredit, 
  loyaltyPoints,
  mobileMoney,
  voucher,
  creditAccount,
  other 
}

enum PaymentStatus { pending, processing, completed, failed, refunded, partiallyRefunded, cancelled }

// Mobile money providers
enum MobileMoneyProvider {
  mtn,
  vodafone,
  airteltigo,
  telecel,
  other
}

// Card types
enum CardType {
  visa,
  mastercard,
  amex,
  discover,
  local,
  unknown
}

// Transaction result
enum TransactionResult {
  success,
  declined,
  networkError,
  insufficientFunds,
  invalidCard,
  timeout,
  cancelled,
  unknown
}

class PaymentEntity extends Equatable {
  final int? id;
  final int transactionId;
  final PaymentMethod method;
  final PaymentStatus status;
  final int amount; // in cents
  final String currency;
  final String? reference; // Payment gateway reference
  final String? authCode; // Authorization code
  final String? gatewayTransactionId;
  final Map<String, dynamic>? metadata; // Additional payment data
  final String? failureReason;
  final double? processingFee;
  final String? customerId;
  final DateTime createdAt;
  final DateTime? completedAt;
  final String? createdById;
  final String? updatedAt;

  const PaymentEntity({
    this.id,
    required this.transactionId,
    required this.method,
    this.status = PaymentStatus.pending,
    required this.amount,
    this.currency = 'GHS',
    this.reference,
    this.authCode,
    this.gatewayTransactionId,
    this.metadata,
    this.failureReason,
    this.processingFee,
    this.customerId,
    required this.createdAt,
    this.completedAt,
    this.createdById,
    this.updatedAt,
  });

  // Legacy constructor for backward compatibility
  PaymentEntity.legacy({
    this.id,
    required this.transactionId,
    required String methodString,
    required this.amount,
    this.reference,
    String? createdAtString,
    this.updatedAt,
  }) : method = _parsePaymentMethod(methodString),
       status = PaymentStatus.completed,
       currency = 'GHS',
       authCode = null,
       gatewayTransactionId = null,
       metadata = null,
       failureReason = null,
       processingFee = null,
       customerId = null,
       createdAt = DateTime.tryParse(createdAtString ?? '') ?? DateTime.now(),
       completedAt = null,
       createdById = null;

  static PaymentMethod _parsePaymentMethod(String methodString) {
    switch (methodString.toLowerCase()) {
      case 'cash': return PaymentMethod.cash;
      case 'card': return PaymentMethod.card;
      case 'mobile': return PaymentMethod.mobileWallet;
      case 'qr': return PaymentMethod.qrPayment;
      case 'nfc': return PaymentMethod.nfc;
      case 'transfer': return PaymentMethod.bankTransfer;
      case 'credit': return PaymentMethod.storeCredit;
      case 'points': return PaymentMethod.loyaltyPoints;
      case 'momo': return PaymentMethod.mobileMoney;
      case 'voucher': return PaymentMethod.voucher;
      case 'account': return PaymentMethod.creditAccount;
      default: return PaymentMethod.other;
    }
  }

  PaymentEntity copyWith({
    int? id,
    int? transactionId,
    PaymentMethod? method,
    PaymentStatus? status,
    int? amount,
    String? currency,
    String? reference,
    String? authCode,
    String? gatewayTransactionId,
    Map<String, dynamic>? metadata,
    String? failureReason,
    double? processingFee,
    String? customerId,
    DateTime? createdAt,
    DateTime? completedAt,
    String? createdById,
    String? updatedAt,
  }) {
    return PaymentEntity(
      id: id ?? this.id,
      transactionId: transactionId ?? this.transactionId,
      method: method ?? this.method,
      status: status ?? this.status,
      amount: amount ?? this.amount,
      currency: currency ?? this.currency,
      reference: reference ?? this.reference,
      authCode: authCode ?? this.authCode,
      gatewayTransactionId: gatewayTransactionId ?? this.gatewayTransactionId,
      metadata: metadata ?? this.metadata,
      failureReason: failureReason ?? this.failureReason,
      processingFee: processingFee ?? this.processingFee,
      customerId: customerId ?? this.customerId,
      createdAt: createdAt ?? this.createdAt,
      completedAt: completedAt ?? this.completedAt,
      createdById: createdById ?? this.createdById,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  bool get isSuccessful => status == PaymentStatus.completed;
  bool get isPending => status == PaymentStatus.pending;
  bool get isProcessing => status == PaymentStatus.processing;
  bool get hasFailed => status == PaymentStatus.failed;
  bool get isRefunded => status == PaymentStatus.refunded || status == PaymentStatus.partiallyRefunded;
  
  String get displayName {
    switch (method) {
      case PaymentMethod.cash:
        return 'Cash';
      case PaymentMethod.card:
        return 'Card';
      case PaymentMethod.mobileWallet:
        return 'Mobile Wallet';
      case PaymentMethod.qrPayment:
        return 'QR Payment';
      case PaymentMethod.nfc:
        return 'Contactless';
      case PaymentMethod.bankTransfer:
        return 'Bank Transfer';
      case PaymentMethod.storeCredit:
        return 'Store Credit';
      case PaymentMethod.loyaltyPoints:
        return 'Loyalty Points';
      case PaymentMethod.mobileMoney:
        return 'Mobile Money';
      case PaymentMethod.voucher:
        return 'Voucher';
      case PaymentMethod.creditAccount:
        return 'Credit Account';
      case PaymentMethod.other:
        return 'Other';
    }
  }

  // Legacy property for backward compatibility
  String get methodString => method.name;

  @override
  List<Object?> get props => [
    id,
    transactionId,
    method,
    status,
    amount,
    currency,
    reference,
    authCode,
    gatewayTransactionId,
    metadata,
    failureReason,
    processingFee,
    customerId,
    createdAt,
    completedAt,
    createdById,
    updatedAt,
  ];
}

// Enhanced payment transaction for complex payments
class PaymentTransaction extends Equatable {
  final String id;
  final String orderId;
  final PaymentMethod method;
  final double amount;
  final String currency;
  final PaymentStatus status;
  final DateTime createdAt;
  final DateTime? completedAt;
  final String? reference;
  final String? authorizationCode;
  final String? gatewayTransactionId;
  final Map<String, dynamic> metadata;
  final String? failureReason;
  final double? processingFee;
  final String? customerId;

  const PaymentTransaction({
    required this.id,
    required this.orderId,
    required this.method,
    required this.amount,
    required this.currency,
    required this.status,
    required this.createdAt,
    this.completedAt,
    this.reference,
    this.authorizationCode,
    this.gatewayTransactionId,
    this.metadata = const {},
    this.failureReason,
    this.processingFee,
    this.customerId,
  });

  @override
  List<Object?> get props => [
        id,
        orderId,
        method,
        amount,
        currency,
        status,
        createdAt,
        completedAt,
        reference,
        authorizationCode,
        gatewayTransactionId,
        metadata,
        failureReason,
        processingFee,
        customerId,
      ];

  PaymentTransaction copyWith({
    String? id,
    String? orderId,
    PaymentMethod? method,
    double? amount,
    String? currency,
    PaymentStatus? status,
    DateTime? createdAt,
    DateTime? completedAt,
    String? reference,
    String? authorizationCode,
    String? gatewayTransactionId,
    Map<String, dynamic>? metadata,
    String? failureReason,
    double? processingFee,
    String? customerId,
  }) {
    return PaymentTransaction(
      id: id ?? this.id,
      orderId: orderId ?? this.orderId,
      method: method ?? this.method,
      amount: amount ?? this.amount,
      currency: currency ?? this.currency,
      status: status ?? this.status,
      createdAt: createdAt ?? this.createdAt,
      completedAt: completedAt ?? this.completedAt,
      reference: reference ?? this.reference,
      authorizationCode: authorizationCode ?? this.authorizationCode,
      gatewayTransactionId: gatewayTransactionId ?? this.gatewayTransactionId,
      metadata: metadata ?? this.metadata,
      failureReason: failureReason ?? this.failureReason,
      processingFee: processingFee ?? this.processingFee,
      customerId: customerId ?? this.customerId,
    );
  }
}

// Split payment support
class SplitPayment extends Equatable {
  final String id;
  final String orderId;
  final double totalAmount;
  final String currency;
  final List<PaymentTransaction> transactions;
  final PaymentStatus overallStatus;
  final DateTime createdAt;
  final DateTime? completedAt;

  const SplitPayment({
    required this.id,
    required this.orderId,
    required this.totalAmount,
    required this.currency,
    required this.transactions,
    required this.overallStatus,
    required this.createdAt,
    this.completedAt,
  });

  double get totalPaid {
    return transactions
        .where((t) => t.status == PaymentStatus.completed)
        .fold(0.0, (sum, t) => sum + t.amount);
  }

  double get remainingAmount {
    return totalAmount - totalPaid;
  }

  bool get isFullyPaid => remainingAmount <= 0.01; // Account for floating point precision

  @override
  List<Object?> get props => [
        id,
        orderId,
        totalAmount,
        currency,
        transactions,
        overallStatus,
        createdAt,
        completedAt,
      ];

  SplitPayment copyWith({
    String? id,
    String? orderId,
    double? totalAmount,
    String? currency,
    List<PaymentTransaction>? transactions,
    PaymentStatus? overallStatus,
    DateTime? createdAt,
    DateTime? completedAt,
  }) {
    return SplitPayment(
      id: id ?? this.id,
      orderId: orderId ?? this.orderId,
      totalAmount: totalAmount ?? this.totalAmount,
      currency: currency ?? this.currency,
      transactions: transactions ?? this.transactions,
      overallStatus: overallStatus ?? this.overallStatus,
      createdAt: createdAt ?? this.createdAt,
      completedAt: completedAt ?? this.completedAt,
    );
  }
}

// Mobile money transaction details
class MobileMoneyTransaction extends Equatable {
  final String phoneNumber;
  final MobileMoneyProvider provider;
  final String? voucher;
  final String? reference;

  const MobileMoneyTransaction({
    required this.phoneNumber,
    required this.provider,
    this.voucher,
    this.reference,
  });

  @override
  List<Object?> get props => [phoneNumber, provider, voucher, reference];

  Map<String, dynamic> toMap() {
    return {
      'phoneNumber': phoneNumber,
      'provider': provider.toString(),
      'voucher': voucher,
      'reference': reference,
    };
  }

  factory MobileMoneyTransaction.fromMap(Map<String, dynamic> map) {
    return MobileMoneyTransaction(
      phoneNumber: map['phoneNumber'] ?? '',
      provider: MobileMoneyProvider.values.firstWhere(
        (e) => e.toString() == map['provider'],
        orElse: () => MobileMoneyProvider.other,
      ),
      voucher: map['voucher'],
      reference: map['reference'],
    );
  }
}

// Card transaction details
class CardTransaction extends Equatable {
  final String? cardNumber; // Last 4 digits only
  final CardType cardType;
  final String? cardHolderName;
  final String? authorizationCode;
  final bool isContactless;
  final String? terminalId;

  const CardTransaction({
    this.cardNumber,
    required this.cardType,
    this.cardHolderName,
    this.authorizationCode,
    this.isContactless = false,
    this.terminalId,
  });

  @override
  List<Object?> get props => [
        cardNumber,
        cardType,
        cardHolderName,
        authorizationCode,
        isContactless,
        terminalId,
      ];

  Map<String, dynamic> toMap() {
    return {
      'cardNumber': cardNumber,
      'cardType': cardType.toString(),
      'cardHolderName': cardHolderName,
      'authorizationCode': authorizationCode,
      'isContactless': isContactless,
      'terminalId': terminalId,
    };
  }

  factory CardTransaction.fromMap(Map<String, dynamic> map) {
    return CardTransaction(
      cardNumber: map['cardNumber'],
      cardType: CardType.values.firstWhere(
        (e) => e.toString() == map['cardType'],
        orElse: () => CardType.unknown,
      ),
      cardHolderName: map['cardHolderName'],
      authorizationCode: map['authorizationCode'],
      isContactless: map['isContactless'] ?? false,
      terminalId: map['terminalId'],
    );
  }
}

// Payment gateway configuration
class PaymentGatewayConfig extends Equatable {
  final String id;
  final String name;
  final String gatewayType; // paystack, stripe, flutterwave, etc.
  final Map<String, String> credentials;
  final List<PaymentMethod> supportedMethods;
  final bool isEnabled;
  final double? minimumAmount;
  final double? maximumAmount;
  final List<String> supportedCurrencies;

  const PaymentGatewayConfig({
    required this.id,
    required this.name,
    required this.gatewayType,
    required this.credentials,
    required this.supportedMethods,
    this.isEnabled = true,
    this.minimumAmount,
    this.maximumAmount,
    this.supportedCurrencies = const ['GHS'],
  });

  @override
  List<Object?> get props => [
        id,
        name,
        gatewayType,
        credentials,
        supportedMethods,
        isEnabled,
        minimumAmount,
        maximumAmount,
        supportedCurrencies,
      ];

  PaymentGatewayConfig copyWith({
    String? id,
    String? name,
    String? gatewayType,
    Map<String, String>? credentials,
    List<PaymentMethod>? supportedMethods,
    bool? isEnabled,
    double? minimumAmount,
    double? maximumAmount,
    List<String>? supportedCurrencies,
  }) {
    return PaymentGatewayConfig(
      id: id ?? this.id,
      name: name ?? this.name,
      gatewayType: gatewayType ?? this.gatewayType,
      credentials: credentials ?? this.credentials,
      supportedMethods: supportedMethods ?? this.supportedMethods,
      isEnabled: isEnabled ?? this.isEnabled,
      minimumAmount: minimumAmount ?? this.minimumAmount,
      maximumAmount: maximumAmount ?? this.maximumAmount,
      supportedCurrencies: supportedCurrencies ?? this.supportedCurrencies,
    );
  }
}

// Payment result
class PaymentResult extends Equatable {
  final TransactionResult result;
  final PaymentTransaction? transaction;
  final String? message;
  final String? reference;
  final Map<String, dynamic> data;

  const PaymentResult({
    required this.result,
    this.transaction,
    this.message,
    this.reference,
    this.data = const {},
  });

  bool get isSuccess => result == TransactionResult.success;
  bool get isFailure => !isSuccess;

  @override
  List<Object?> get props => [result, transaction, message, reference, data];

  PaymentResult copyWith({
    TransactionResult? result,
    PaymentTransaction? transaction,
    String? message,
    String? reference,
    Map<String, dynamic>? data,
  }) {
    return PaymentResult(
      result: result ?? this.result,
      transaction: transaction ?? this.transaction,
      message: message ?? this.message,
      reference: reference ?? this.reference,
      data: data ?? this.data,
    );
  }
}


