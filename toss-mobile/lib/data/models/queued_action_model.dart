import '../../domain/entities/queued_action_entity.dart';

class QueuedActionModel {
  int id;
  String repository;
  String method;
  String param;
  bool isCritical;
  String? status; // pending, processing, failed
  int? retryCount;
  String? lastError;
  String? nextRetryAt;
  String createdAt;

  QueuedActionModel({
    required this.id,
    required this.repository,
    required this.method,
    required this.param,
    required this.isCritical,
    this.status,
    this.retryCount,
    this.lastError,
    this.nextRetryAt,
    required this.createdAt,
  });

  factory QueuedActionModel.fromJson(Map<String, dynamic> json) {
    return QueuedActionModel(
      id: json['id'],
      repository: json['repository'],
      method: json['method'],
      param: json['param'],
      isCritical: json['isCritical'] == 1 ? true : false,
      status: json['status'],
      retryCount: json['retryCount'],
      lastError: json['lastError'],
      nextRetryAt: json['nextRetryAt'],
      createdAt: json['createdAt'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'repository': repository,
      'method': method,
      'param': param,
      'isCritical': isCritical ? 1 : 0,
      'status': status,
      'retryCount': retryCount,
      'lastError': lastError,
      'nextRetryAt': nextRetryAt,
      'createdAt': createdAt,
    };
  }

  factory QueuedActionModel.fromEntity(QueuedActionEntity entity) {
    return QueuedActionModel(
      id: entity.id ?? DateTime.now().millisecondsSinceEpoch,
      repository: entity.repository,
      method: entity.method,
      param: entity.param,
      isCritical: entity.isCritical,
      status: 'pending',
      retryCount: 0,
      createdAt: entity.createdAt ?? DateTime.now().toIso8601String(),
    );
  }

  QueuedActionEntity toEntity() {
    return QueuedActionEntity(
      id: id,
      repository: repository,
      method: method,
      param: param,
      isCritical: isCritical,
      createdAt: createdAt,
    );
  }
}
