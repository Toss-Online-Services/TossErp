import 'package:equatable/equatable.dart';

class ZReportEntity extends Equatable {
  final int? id;
  final int shiftId;
  final String summaryJson;
  final String? createdAt;

  const ZReportEntity({
    this.id,
    required this.shiftId,
    required this.summaryJson,
    this.createdAt,
  });

  ZReportEntity copyWith({
    int? id,
    int? shiftId,
    String? summaryJson,
    String? createdAt,
  }) {
    return ZReportEntity(
      id: id ?? this.id,
      shiftId: shiftId ?? this.shiftId,
      summaryJson: summaryJson ?? this.summaryJson,
      createdAt: createdAt ?? this.createdAt,
    );
  }

  @override
  List<Object?> get props => [id, shiftId, summaryJson, createdAt];
}


