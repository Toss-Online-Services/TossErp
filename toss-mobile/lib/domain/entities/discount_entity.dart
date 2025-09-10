import 'package:equatable/equatable.dart';

class DiscountEntity extends Equatable {
  final int? id;
  final int? transactionId;
  final int? orderedProductId;
  final String scope; // line|cart
  final String type; // percentage|fixed
  final int value;
  final String? code;
  final String? reason;
  final String? createdAt;
  final String? updatedAt;

  const DiscountEntity({
    this.id,
    this.transactionId,
    this.orderedProductId,
    required this.scope,
    required this.type,
    required this.value,
    this.code,
    this.reason,
    this.createdAt,
    this.updatedAt,
  });

  DiscountEntity copyWith({
    int? id,
    int? transactionId,
    int? orderedProductId,
    String? scope,
    String? type,
    int? value,
    String? code,
    String? reason,
    String? createdAt,
    String? updatedAt,
  }) {
    return DiscountEntity(
      id: id ?? this.id,
      transactionId: transactionId ?? this.transactionId,
      orderedProductId: orderedProductId ?? this.orderedProductId,
      scope: scope ?? this.scope,
      type: type ?? this.type,
      value: value ?? this.value,
      code: code ?? this.code,
      reason: reason ?? this.reason,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  @override
  List<Object?> get props => [id, transactionId, orderedProductId, scope, type, value, code, reason, createdAt, updatedAt];
}


