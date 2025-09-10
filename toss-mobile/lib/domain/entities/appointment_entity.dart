import 'package:equatable/equatable.dart';

class AppointmentEntity extends Equatable {
  final int? id;
  final String? customerName;
  final String? customerPhone;
  final String serviceName;
  final String? staffName;
  final String scheduledAt;
  final String status;
  final String? note;
  final int? linkedTransactionId;
  final String? createdAt;
  final String? updatedAt;

  const AppointmentEntity({
    this.id,
    this.customerName,
    this.customerPhone,
    required this.serviceName,
    this.staffName,
    required this.scheduledAt,
    this.status = 'scheduled',
    this.note,
    this.linkedTransactionId,
    this.createdAt,
    this.updatedAt,
  });

  AppointmentEntity copyWith({
    int? id,
    String? customerName,
    String? customerPhone,
    String? serviceName,
    String? staffName,
    String? scheduledAt,
    String? status,
    String? note,
    int? linkedTransactionId,
    String? createdAt,
    String? updatedAt,
  }) {
    return AppointmentEntity(
      id: id ?? this.id,
      customerName: customerName ?? this.customerName,
      customerPhone: customerPhone ?? this.customerPhone,
      serviceName: serviceName ?? this.serviceName,
      staffName: staffName ?? this.staffName,
      scheduledAt: scheduledAt ?? this.scheduledAt,
      status: status ?? this.status,
      note: note ?? this.note,
      linkedTransactionId: linkedTransactionId ?? this.linkedTransactionId,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  @override
  List<Object?> get props => [id, customerName, customerPhone, serviceName, staffName, scheduledAt, status, note, linkedTransactionId, createdAt, updatedAt];
}


