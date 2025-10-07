import 'package:equatable/equatable.dart';

class CashMovementEntity extends Equatable {
  final int? id;
  final int shiftId;
  final String type;
  final int amount;
  final String? note;
  final String? createdAt;

  const CashMovementEntity({
    this.id,
    required this.shiftId,
    required this.type,
    required this.amount,
    this.note,
    this.createdAt,
  });

  CashMovementEntity copyWith({
    int? id,
    int? shiftId,
    String? type,
    int? amount,
    String? note,
    String? createdAt,
  }) {
    return CashMovementEntity(
      id: id ?? this.id,
      shiftId: shiftId ?? this.shiftId,
      type: type ?? this.type,
      amount: amount ?? this.amount,
      note: note ?? this.note,
      createdAt: createdAt ?? this.createdAt,
    );
  }

  @override
  List<Object?> get props => [id, shiftId, type, amount, note, createdAt];
}


