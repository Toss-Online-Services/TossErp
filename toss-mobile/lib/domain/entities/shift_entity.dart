import 'package:equatable/equatable.dart';

class ShiftEntity extends Equatable {
  final int? id;
  final String userId;
  final String? startedAt;
  final int openingFloat;
  final String? endedAt;
  final int? closingCash;
  final int? expectedCash;
  final int? variance;
  final String status;
  final String? createdAt;
  final String? updatedAt;

  const ShiftEntity({
    this.id,
    required this.userId,
    this.startedAt,
    required this.openingFloat,
    this.endedAt,
    this.closingCash,
    this.expectedCash,
    this.variance,
    this.status = 'open',
    this.createdAt,
    this.updatedAt,
  });

  ShiftEntity copyWith({
    int? id,
    String? userId,
    String? startedAt,
    int? openingFloat,
    String? endedAt,
    int? closingCash,
    int? expectedCash,
    int? variance,
    String? status,
    String? createdAt,
    String? updatedAt,
  }) {
    return ShiftEntity(
      id: id ?? this.id,
      userId: userId ?? this.userId,
      startedAt: startedAt ?? this.startedAt,
      openingFloat: openingFloat ?? this.openingFloat,
      endedAt: endedAt ?? this.endedAt,
      closingCash: closingCash ?? this.closingCash,
      expectedCash: expectedCash ?? this.expectedCash,
      variance: variance ?? this.variance,
      status: status ?? this.status,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  @override
  List<Object?> get props => [id, userId, startedAt, openingFloat, endedAt, closingCash, expectedCash, variance, status, createdAt, updatedAt];
}


