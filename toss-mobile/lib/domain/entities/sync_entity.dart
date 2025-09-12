import 'package:equatable/equatable.dart';

enum SyncOperationType {
  create,
  update,
  delete,
  transfer,
  payment,
}

enum SyncEntityType {
  transaction,
  product,
  customer,
  inventory,
  discount,
  employee,
  location,
  transfer,
}

enum SyncStatus {
  pending,
  inProgress,
  completed,
  failed,
  conflict,
  retrying,
}

enum ConflictResolutionStrategy {
  localWins,
  remoteWins,
  merge,
  manual,
  keepBoth,
}

class SyncQueueEntity extends Equatable {
  final String id;
  final SyncOperationType operationType;
  final SyncEntityType entityType;
  final String entityId;
  final Map<String, dynamic> data;
  final Map<String, dynamic>? previousData;
  final SyncStatus status;
  final DateTime createdAt;
  final DateTime? lastAttemptAt;
  final DateTime? completedAt;
  final int attemptCount;
  final int maxRetries;
  final String? errorMessage;
  final Map<String, dynamic>? conflictData;
  final ConflictResolutionStrategy? resolutionStrategy;
  final int priority;
  final String? dependsOnId;
  final Map<String, dynamic> metadata;
  final String locationId;
  final String userId;
  final bool requiresOnline;
  final DateTime? retryAfter;

  const SyncQueueEntity({
    required this.id,
    required this.operationType,
    required this.entityType,
    required this.entityId,
    required this.data,
    this.previousData,
    required this.status,
    required this.createdAt,
    this.lastAttemptAt,
    this.completedAt,
    this.attemptCount = 0,
    this.maxRetries = 3,
    this.errorMessage,
    this.conflictData,
    this.resolutionStrategy,
    this.priority = 5,
    this.dependsOnId,
    this.metadata = const {},
    required this.locationId,
    required this.userId,
    this.requiresOnline = true,
    this.retryAfter,
  });

  SyncQueueEntity copyWith({
    String? id,
    SyncOperationType? operationType,
    SyncEntityType? entityType,
    String? entityId,
    Map<String, dynamic>? data,
    Map<String, dynamic>? previousData,
    SyncStatus? status,
    DateTime? createdAt,
    DateTime? lastAttemptAt,
    DateTime? completedAt,
    int? attemptCount,
    int? maxRetries,
    String? errorMessage,
    Map<String, dynamic>? conflictData,
    ConflictResolutionStrategy? resolutionStrategy,
    int? priority,
    String? dependsOnId,
    Map<String, dynamic>? metadata,
    String? locationId,
    String? userId,
    bool? requiresOnline,
    DateTime? retryAfter,
  }) {
    return SyncQueueEntity(
      id: id ?? this.id,
      operationType: operationType ?? this.operationType,
      entityType: entityType ?? this.entityType,
      entityId: entityId ?? this.entityId,
      data: data ?? this.data,
      previousData: previousData ?? this.previousData,
      status: status ?? this.status,
      createdAt: createdAt ?? this.createdAt,
      lastAttemptAt: lastAttemptAt ?? this.lastAttemptAt,
      completedAt: completedAt ?? this.completedAt,
      attemptCount: attemptCount ?? this.attemptCount,
      maxRetries: maxRetries ?? this.maxRetries,
      errorMessage: errorMessage ?? this.errorMessage,
      conflictData: conflictData ?? this.conflictData,
      resolutionStrategy: resolutionStrategy ?? this.resolutionStrategy,
      priority: priority ?? this.priority,
      dependsOnId: dependsOnId ?? this.dependsOnId,
      metadata: metadata ?? this.metadata,
      locationId: locationId ?? this.locationId,
      userId: userId ?? this.userId,
      requiresOnline: requiresOnline ?? this.requiresOnline,
      retryAfter: retryAfter ?? this.retryAfter,
    );
  }

  bool get canRetry => attemptCount < maxRetries && status != SyncStatus.completed;
  
  bool get isConflicted => status == SyncStatus.conflict;
  
  bool get isExpired => retryAfter != null && DateTime.now().isBefore(retryAfter!);
  
  bool get isPending => status == SyncStatus.pending;
  
  bool get isReadyForRetry => 
      canRetry && 
      (retryAfter == null || DateTime.now().isAfter(retryAfter!));

  Duration get timeToRetry {
    if (retryAfter == null) return Duration.zero;
    final now = DateTime.now();
    return retryAfter!.isAfter(now) ? retryAfter!.difference(now) : Duration.zero;
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'operationType': operationType.name,
      'entityType': entityType.name,
      'entityId': entityId,
      'data': data,
      'previousData': previousData,
      'status': status.name,
      'createdAt': createdAt.toIso8601String(),
      'lastAttemptAt': lastAttemptAt?.toIso8601String(),
      'completedAt': completedAt?.toIso8601String(),
      'attemptCount': attemptCount,
      'maxRetries': maxRetries,
      'errorMessage': errorMessage,
      'conflictData': conflictData,
      'resolutionStrategy': resolutionStrategy?.name,
      'priority': priority,
      'dependsOnId': dependsOnId,
      'metadata': metadata,
      'locationId': locationId,
      'userId': userId,
      'requiresOnline': requiresOnline,
      'retryAfter': retryAfter?.toIso8601String(),
    };
  }

  factory SyncQueueEntity.fromJson(Map<String, dynamic> json) {
    return SyncQueueEntity(
      id: json['id'] as String,
      operationType: SyncOperationType.values.firstWhere(
        (e) => e.name == json['operationType'],
      ),
      entityType: SyncEntityType.values.firstWhere(
        (e) => e.name == json['entityType'],
      ),
      entityId: json['entityId'] as String,
      data: Map<String, dynamic>.from(json['data'] as Map),
      previousData: json['previousData'] != null 
          ? Map<String, dynamic>.from(json['previousData'] as Map)
          : null,
      status: SyncStatus.values.firstWhere(
        (e) => e.name == json['status'],
      ),
      createdAt: DateTime.parse(json['createdAt'] as String),
      lastAttemptAt: json['lastAttemptAt'] != null 
          ? DateTime.parse(json['lastAttemptAt'] as String)
          : null,
      completedAt: json['completedAt'] != null 
          ? DateTime.parse(json['completedAt'] as String)
          : null,
      attemptCount: json['attemptCount'] as int? ?? 0,
      maxRetries: json['maxRetries'] as int? ?? 3,
      errorMessage: json['errorMessage'] as String?,
      conflictData: json['conflictData'] != null 
          ? Map<String, dynamic>.from(json['conflictData'] as Map)
          : null,
      resolutionStrategy: json['resolutionStrategy'] != null
          ? ConflictResolutionStrategy.values.firstWhere(
              (e) => e.name == json['resolutionStrategy'],
            )
          : null,
      priority: json['priority'] as int? ?? 5,
      dependsOnId: json['dependsOnId'] as String?,
      metadata: Map<String, dynamic>.from(json['metadata'] as Map? ?? {}),
      locationId: json['locationId'] as String,
      userId: json['userId'] as String,
      requiresOnline: json['requiresOnline'] as bool? ?? true,
      retryAfter: json['retryAfter'] != null 
          ? DateTime.parse(json['retryAfter'] as String)
          : null,
    );
  }

  @override
  List<Object?> get props => [
        id,
        operationType,
        entityType,
        entityId,
        data,
        previousData,
        status,
        createdAt,
        lastAttemptAt,
        completedAt,
        attemptCount,
        maxRetries,
        errorMessage,
        conflictData,
        resolutionStrategy,
        priority,
        dependsOnId,
        metadata,
        locationId,
        userId,
        requiresOnline,
        retryAfter,
      ];
}

class SyncConflict extends Equatable {
  final String id;
  final String queueItemId;
  final SyncEntityType entityType;
  final String entityId;
  final Map<String, dynamic> localData;
  final Map<String, dynamic> remoteData;
  final Map<String, dynamic> conflictFields;
  final DateTime detectedAt;
  final ConflictResolutionStrategy? suggestedStrategy;
  final Map<String, dynamic> metadata;

  const SyncConflict({
    required this.id,
    required this.queueItemId,
    required this.entityType,
    required this.entityId,
    required this.localData,
    required this.remoteData,
    required this.conflictFields,
    required this.detectedAt,
    this.suggestedStrategy,
    this.metadata = const {},
  });

  SyncConflict copyWith({
    String? id,
    String? queueItemId,
    SyncEntityType? entityType,
    String? entityId,
    Map<String, dynamic>? localData,
    Map<String, dynamic>? remoteData,
    Map<String, dynamic>? conflictFields,
    DateTime? detectedAt,
    ConflictResolutionStrategy? suggestedStrategy,
    Map<String, dynamic>? metadata,
  }) {
    return SyncConflict(
      id: id ?? this.id,
      queueItemId: queueItemId ?? this.queueItemId,
      entityType: entityType ?? this.entityType,
      entityId: entityId ?? this.entityId,
      localData: localData ?? this.localData,
      remoteData: remoteData ?? this.remoteData,
      conflictFields: conflictFields ?? this.conflictFields,
      detectedAt: detectedAt ?? this.detectedAt,
      suggestedStrategy: suggestedStrategy ?? this.suggestedStrategy,
      metadata: metadata ?? this.metadata,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'queueItemId': queueItemId,
      'entityType': entityType.name,
      'entityId': entityId,
      'localData': localData,
      'remoteData': remoteData,
      'conflictFields': conflictFields,
      'detectedAt': detectedAt.toIso8601String(),
      'suggestedStrategy': suggestedStrategy?.name,
      'metadata': metadata,
    };
  }

  factory SyncConflict.fromJson(Map<String, dynamic> json) {
    return SyncConflict(
      id: json['id'] as String,
      queueItemId: json['queueItemId'] as String,
      entityType: SyncEntityType.values.firstWhere(
        (e) => e.name == json['entityType'],
      ),
      entityId: json['entityId'] as String,
      localData: Map<String, dynamic>.from(json['localData'] as Map),
      remoteData: Map<String, dynamic>.from(json['remoteData'] as Map),
      conflictFields: Map<String, dynamic>.from(json['conflictFields'] as Map),
      detectedAt: DateTime.parse(json['detectedAt'] as String),
      suggestedStrategy: json['suggestedStrategy'] != null
          ? ConflictResolutionStrategy.values.firstWhere(
              (e) => e.name == json['suggestedStrategy'],
            )
          : null,
      metadata: Map<String, dynamic>.from(json['metadata'] as Map? ?? {}),
    );
  }

  @override
  List<Object?> get props => [
        id,
        queueItemId,
        entityType,
        entityId,
        localData,
        remoteData,
        conflictFields,
        detectedAt,
        suggestedStrategy,
        metadata,
      ];
}

class SyncConfiguration extends Equatable {
  final bool autoSync;
  final Duration syncInterval;
  final bool syncOnlyOnWifi;
  final bool syncOnLowBattery;
  final int maxRetries;
  final Duration retryBackoffMultiplier;
  final int maxQueueSize;
  final bool enableCompression;
  final bool enableEncryption;
  final List<SyncEntityType> enabledEntities;
  final Map<SyncEntityType, int> entityPriorities;
  final ConflictResolutionStrategy defaultConflictStrategy;
  final bool requireUserApprovalForConflicts;
  final Duration conflictResolutionTimeout;
  final bool enablePartialSync;
  final Map<String, dynamic> customSettings;

  const SyncConfiguration({
    this.autoSync = true,
    this.syncInterval = const Duration(minutes: 5),
    this.syncOnlyOnWifi = false,
    this.syncOnLowBattery = false,
    this.maxRetries = 3,
    this.retryBackoffMultiplier = const Duration(seconds: 30),
    this.maxQueueSize = 1000,
    this.enableCompression = true,
    this.enableEncryption = true,
    this.enabledEntities = SyncEntityType.values,
    this.entityPriorities = const {},
    this.defaultConflictStrategy = ConflictResolutionStrategy.manual,
    this.requireUserApprovalForConflicts = true,
    this.conflictResolutionTimeout = const Duration(hours: 24),
    this.enablePartialSync = true,
    this.customSettings = const {},
  });

  SyncConfiguration copyWith({
    bool? autoSync,
    Duration? syncInterval,
    bool? syncOnlyOnWifi,
    bool? syncOnLowBattery,
    int? maxRetries,
    Duration? retryBackoffMultiplier,
    int? maxQueueSize,
    bool? enableCompression,
    bool? enableEncryption,
    List<SyncEntityType>? enabledEntities,
    Map<SyncEntityType, int>? entityPriorities,
    ConflictResolutionStrategy? defaultConflictStrategy,
    bool? requireUserApprovalForConflicts,
    Duration? conflictResolutionTimeout,
    bool? enablePartialSync,
    Map<String, dynamic>? customSettings,
  }) {
    return SyncConfiguration(
      autoSync: autoSync ?? this.autoSync,
      syncInterval: syncInterval ?? this.syncInterval,
      syncOnlyOnWifi: syncOnlyOnWifi ?? this.syncOnlyOnWifi,
      syncOnLowBattery: syncOnLowBattery ?? this.syncOnLowBattery,
      maxRetries: maxRetries ?? this.maxRetries,
      retryBackoffMultiplier: retryBackoffMultiplier ?? this.retryBackoffMultiplier,
      maxQueueSize: maxQueueSize ?? this.maxQueueSize,
      enableCompression: enableCompression ?? this.enableCompression,
      enableEncryption: enableEncryption ?? this.enableEncryption,
      enabledEntities: enabledEntities ?? this.enabledEntities,
      entityPriorities: entityPriorities ?? this.entityPriorities,
      defaultConflictStrategy: defaultConflictStrategy ?? this.defaultConflictStrategy,
      requireUserApprovalForConflicts: requireUserApprovalForConflicts ?? this.requireUserApprovalForConflicts,
      conflictResolutionTimeout: conflictResolutionTimeout ?? this.conflictResolutionTimeout,
      enablePartialSync: enablePartialSync ?? this.enablePartialSync,
      customSettings: customSettings ?? this.customSettings,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'autoSync': autoSync,
      'syncInterval': syncInterval.inMilliseconds,
      'syncOnlyOnWifi': syncOnlyOnWifi,
      'syncOnLowBattery': syncOnLowBattery,
      'maxRetries': maxRetries,
      'retryBackoffMultiplier': retryBackoffMultiplier.inMilliseconds,
      'maxQueueSize': maxQueueSize,
      'enableCompression': enableCompression,
      'enableEncryption': enableEncryption,
      'enabledEntities': enabledEntities.map((e) => e.name).toList(),
      'entityPriorities': entityPriorities.map((k, v) => MapEntry(k.name, v)),
      'defaultConflictStrategy': defaultConflictStrategy.name,
      'requireUserApprovalForConflicts': requireUserApprovalForConflicts,
      'conflictResolutionTimeout': conflictResolutionTimeout.inMilliseconds,
      'enablePartialSync': enablePartialSync,
      'customSettings': customSettings,
    };
  }

  factory SyncConfiguration.fromJson(Map<String, dynamic> json) {
    return SyncConfiguration(
      autoSync: json['autoSync'] as bool? ?? true,
      syncInterval: Duration(milliseconds: json['syncInterval'] as int? ?? 300000),
      syncOnlyOnWifi: json['syncOnlyOnWifi'] as bool? ?? false,
      syncOnLowBattery: json['syncOnLowBattery'] as bool? ?? false,
      maxRetries: json['maxRetries'] as int? ?? 3,
      retryBackoffMultiplier: Duration(milliseconds: json['retryBackoffMultiplier'] as int? ?? 30000),
      maxQueueSize: json['maxQueueSize'] as int? ?? 1000,
      enableCompression: json['enableCompression'] as bool? ?? true,
      enableEncryption: json['enableEncryption'] as bool? ?? true,
      enabledEntities: (json['enabledEntities'] as List?)
          ?.map((e) => SyncEntityType.values.firstWhere((et) => et.name == e))
          .toList() ?? SyncEntityType.values,
      entityPriorities: (json['entityPriorities'] as Map?)
          ?.map((k, v) => MapEntry(
                SyncEntityType.values.firstWhere((et) => et.name == k),
                v as int,
              )) ?? const {},
      defaultConflictStrategy: ConflictResolutionStrategy.values.firstWhere(
        (e) => e.name == (json['defaultConflictStrategy'] ?? 'manual'),
      ),
      requireUserApprovalForConflicts: json['requireUserApprovalForConflicts'] as bool? ?? true,
      conflictResolutionTimeout: Duration(milliseconds: json['conflictResolutionTimeout'] as int? ?? 86400000),
      enablePartialSync: json['enablePartialSync'] as bool? ?? true,
      customSettings: Map<String, dynamic>.from(json['customSettings'] as Map? ?? {}),
    );
  }

  @override
  List<Object?> get props => [
        autoSync,
        syncInterval,
        syncOnlyOnWifi,
        syncOnLowBattery,
        maxRetries,
        retryBackoffMultiplier,
        maxQueueSize,
        enableCompression,
        enableEncryption,
        enabledEntities,
        entityPriorities,
        defaultConflictStrategy,
        requireUserApprovalForConflicts,
        conflictResolutionTimeout,
        enablePartialSync,
        customSettings,
      ];
}
