import 'package:equatable/equatable.dart';

enum PaymentMethod { cash, card, mobileMoney, bankTransfer, other }
enum PaymentStatus { pending, completed, failed, cancelled, refunded }

class PaymentEntity extends Equatable {
  final int? id;
  final int? saleId;
  final String? saleNumber;
  final PaymentMethod method;
  final PaymentStatus status;
  final int amount; // Amount in cents
  final String? referenceNumber;
  final String? reference; // Alias for referenceNumber for receipt compatibility
  final String? transactionId;
  final String? cardLast4;
  final String? cardType; // Visa, Mastercard, etc.
  final String? mobileMoneyProvider; // M-Pesa, Airtel Money, etc.
  final String? mobileMoneyNumber;
  final String? bankName;
  final String? bankAccountNumber;
  final DateTime paymentDate;
  final DateTime createdAt;
  final DateTime? updatedAt;
  final String? notes;
  final int? processedById;
  final String? processedByName;
  final Map<String, dynamic>? metadata;

  const PaymentEntity({
    this.id,
    this.saleId,
    this.saleNumber,
    required this.method,
    this.status = PaymentStatus.pending,
    required this.amount,
    this.referenceNumber,
    this.reference,
    this.transactionId,
    this.cardLast4,
    this.cardType,
    this.mobileMoneyProvider,
    this.mobileMoneyNumber,
    this.bankName,
    this.bankAccountNumber,
    required this.paymentDate,
    required this.createdAt,
    this.updatedAt,
    this.notes,
    this.processedById,
    this.processedByName,
    this.metadata,
  });

  PaymentEntity copyWith({
    int? id,
    int? saleId,
    String? saleNumber,
    PaymentMethod? method,
    PaymentStatus? status,
    int? amount,
    String? referenceNumber,
    String? reference,
    String? transactionId,
    String? cardLast4,
    String? cardType,
    String? mobileMoneyProvider,
    String? mobileMoneyNumber,
    String? bankName,
    String? bankAccountNumber,
    DateTime? paymentDate,
    DateTime? createdAt,
    DateTime? updatedAt,
    String? notes,
    int? processedById,
    String? processedByName,
    Map<String, dynamic>? metadata,
  }) {
    return PaymentEntity(
      id: id ?? this.id,
      saleId: saleId ?? this.saleId,
      saleNumber: saleNumber ?? this.saleNumber,
      method: method ?? this.method,
      status: status ?? this.status,
      amount: amount ?? this.amount,
      referenceNumber: referenceNumber ?? this.referenceNumber,
      reference: reference ?? this.reference,
      transactionId: transactionId ?? this.transactionId,
      cardLast4: cardLast4 ?? this.cardLast4,
      cardType: cardType ?? this.cardType,
      mobileMoneyProvider: mobileMoneyProvider ?? this.mobileMoneyProvider,
      mobileMoneyNumber: mobileMoneyNumber ?? this.mobileMoneyNumber,
      bankName: bankName ?? this.bankName,
      bankAccountNumber: bankAccountNumber ?? this.bankAccountNumber,
      paymentDate: paymentDate ?? this.paymentDate,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
      notes: notes ?? this.notes,
      processedById: processedById ?? this.processedById,
      processedByName: processedByName ?? this.processedByName,
      metadata: metadata ?? this.metadata,
    );
  }

  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'saleId': saleId,
      'saleNumber': saleNumber,
      'method': method.name,
      'status': status.name,
      'amount': amount,
      'referenceNumber': referenceNumber,
      'reference': reference,
      'transactionId': transactionId,
      'cardLast4': cardLast4,
      'cardType': cardType,
      'mobileMoneyProvider': mobileMoneyProvider,
      'mobileMoneyNumber': mobileMoneyNumber,
      'bankName': bankName,
      'bankAccountNumber': bankAccountNumber,
      'paymentDate': paymentDate.toIso8601String(),
      'createdAt': createdAt.toIso8601String(),
      'updatedAt': updatedAt?.toIso8601String(),
      'notes': notes,
      'processedById': processedById,
      'processedByName': processedByName,
      'metadata': metadata,
    };
  }

  factory PaymentEntity.fromMap(Map<String, dynamic> map) {
    return PaymentEntity(
      id: map['id'],
      saleId: map['saleId'],
      saleNumber: map['saleNumber'],
      method: PaymentMethod.values.firstWhere(
        (e) => e.name == map['method'],
        orElse: () => PaymentMethod.cash,
      ),
      status: PaymentStatus.values.firstWhere(
        (e) => e.name == map['status'],
        orElse: () => PaymentStatus.pending,
      ),
      amount: map['amount'] ?? 0,
      referenceNumber: map['referenceNumber'],
      reference: map['reference'],
      transactionId: map['transactionId'],
      cardLast4: map['cardLast4'],
      cardType: map['cardType'],
      mobileMoneyProvider: map['mobileMoneyProvider'],
      mobileMoneyNumber: map['mobileMoneyNumber'],
      bankName: map['bankName'],
      bankAccountNumber: map['bankAccountNumber'],
      paymentDate: DateTime.parse(map['paymentDate']),
      createdAt: DateTime.parse(map['createdAt']),
      updatedAt: map['updatedAt'] != null ? DateTime.parse(map['updatedAt']) : null,
      notes: map['notes'],
      processedById: map['processedById'],
      processedByName: map['processedByName'],
      metadata: map['metadata'],
    );
  }

  @override
  List<Object?> get props => [
        id,
        saleId,
        saleNumber,
        method,
        status,
        amount,
        referenceNumber,
        reference,
        transactionId,
        cardLast4,
        cardType,
        mobileMoneyProvider,
        mobileMoneyNumber,
        bankName,
        bankAccountNumber,
        paymentDate,
        createdAt,
        updatedAt,
        notes,
        processedById,
        processedByName,
        metadata,
      ];
}

class SplitPaymentEntity extends Equatable {
  final List<PaymentEntity> payments;
  final int totalAmount;
  final int paidAmount;
  final int remainingAmount;

  const SplitPaymentEntity({
    required this.payments,
    required this.totalAmount,
    required this.paidAmount,
    required this.remainingAmount,
  });

  bool get isFullyPaid => remainingAmount <= 0;

  @override
  List<Object?> get props => [payments, totalAmount, paidAmount, remainingAmount];
}
