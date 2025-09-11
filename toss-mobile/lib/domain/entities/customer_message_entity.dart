import 'package:equatable/equatable.dart';

enum MessageType { receipt, promotion, loyalty, birthday, anniversary, reminder, alert }
enum MessageStatus { pending, sent, delivered, failed }
enum MessageChannel { sms, whatsapp, voice, print }

class CustomerMessageEntity extends Equatable {
  final int? id;
  final String customerId;
  final MessageType type;
  final MessageChannel channel;
  final String content;
  final MessageStatus status;
  final DateTime scheduledAt;
  final DateTime? sentAt;
  final DateTime? deliveredAt;
  final String? errorMessage;
  final Map<String, dynamic>? metadata;
  final int? retryCount;
  final String? createdById;
  final String? createdAt;
  final String? updatedAt;

  const CustomerMessageEntity({
    this.id,
    required this.customerId,
    required this.type,
    required this.channel,
    required this.content,
    this.status = MessageStatus.pending,
    required this.scheduledAt,
    this.sentAt,
    this.deliveredAt,
    this.errorMessage,
    this.metadata,
    this.retryCount = 0,
    this.createdById,
    this.createdAt,
    this.updatedAt,
  });

  CustomerMessageEntity copyWith({
    int? id,
    String? customerId,
    MessageType? type,
    MessageChannel? channel,
    String? content,
    MessageStatus? status,
    DateTime? scheduledAt,
    DateTime? sentAt,
    DateTime? deliveredAt,
    String? errorMessage,
    Map<String, dynamic>? metadata,
    int? retryCount,
    String? createdById,
    String? createdAt,
    String? updatedAt,
  }) {
    return CustomerMessageEntity(
      id: id ?? this.id,
      customerId: customerId ?? this.customerId,
      type: type ?? this.type,
      channel: channel ?? this.channel,
      content: content ?? this.content,
      status: status ?? this.status,
      scheduledAt: scheduledAt ?? this.scheduledAt,
      sentAt: sentAt ?? this.sentAt,
      deliveredAt: deliveredAt ?? this.deliveredAt,
      errorMessage: errorMessage ?? this.errorMessage,
      metadata: metadata ?? this.metadata,
      retryCount: retryCount ?? this.retryCount,
      createdById: createdById ?? this.createdById,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  bool get canRetry => retryCount < 3 && status == MessageStatus.failed;
  bool get isOverdue => DateTime.now().isAfter(scheduledAt.add(const Duration(hours: 1))) && status == MessageStatus.pending;

  @override
  List<Object?> get props => [
    id,
    customerId,
    type,
    channel,
    content,
    status,
    scheduledAt,
    sentAt,
    deliveredAt,
    errorMessage,
    metadata,
    retryCount,
    createdById,
    createdAt,
    updatedAt,
  ];
}
