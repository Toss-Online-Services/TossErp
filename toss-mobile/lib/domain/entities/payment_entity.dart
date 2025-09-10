import 'package:equatable/equatable.dart';

class PaymentEntity extends Equatable {
  final int? id;
  final int transactionId;
  final String method;
  final int amount;
  final String? reference;
  final String? createdAt;
  final String? updatedAt;

  const PaymentEntity({
    this.id,
    required this.transactionId,
    required this.method,
    required this.amount,
    this.reference,
    this.createdAt,
    this.updatedAt,
  });

  PaymentEntity copyWith({
    int? id,
    int? transactionId,
    String? method,
    int? amount,
    String? reference,
    String? createdAt,
    String? updatedAt,
  }) {
    return PaymentEntity(
      id: id ?? this.id,
      transactionId: transactionId ?? this.transactionId,
      method: method ?? this.method,
      amount: amount ?? this.amount,
      reference: reference ?? this.reference,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  @override
  List<Object?> get props => [id, transactionId, method, amount, reference, createdAt, updatedAt];
}


