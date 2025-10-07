import 'dart:async';
import 'dart:convert';
// import 'dart:io';
import 'package:connectivity_plus/connectivity_plus.dart';
import 'package:battery_plus/battery_plus.dart';
import 'package:crypto/crypto.dart';

import '../../domain/entities/sync_entity.dart';
import '../repositories/sync_repository.dart';
import '../repositories/connectivity_repository.dart';
import '../repositories/local_storage_repository.dart';
import '../repositories/cloud_storage_repository.dart';

class SyncService {
  final SyncRepository _syncRepository;
  final ConnectivityRepository _connectivityRepository;
  final LocalStorageRepository _localStorageRepository;
  final CloudStorageRepository _cloudStorageRepository;
  final Battery _battery = Battery();

  // Sync state
  bool _isSyncing = false;
  bool _isPaused = false;
  Timer? _syncTimer;
  SyncConfiguration _configuration = const SyncConfiguration();
  
  // Streams
  final _syncStatusController = StreamController<SyncStatus>.broadcast();
  final _syncProgressController = StreamController<double>.broadcast();
  final _conflictController = StreamController<SyncConflict>.broadcast();
  final _queueSizeController = StreamController<int>.broadcast();

  Stream<SyncStatus> get syncStatusStream => _syncStatusController.stream;
  Stream<double> get syncProgressStream => _syncProgressController.stream;
  Stream<SyncConflict> get conflictStream => _conflictController.stream;
  Stream<int> get queueSizeStream => _queueSizeController.stream;

  SyncService(
    this._syncRepository,
    this._connectivityRepository,
    this._localStorageRepository,
    this._cloudStorageRepository,
  );

  // Configuration Management
  Future<void> updateConfiguration(SyncConfiguration configuration) async {
    _configuration = configuration;
    await _localStorageRepository.saveSyncConfiguration(configuration);
    
    // Restart sync timer if needed
    if (_configuration.autoSync) {
      _startSyncTimer();
    } else {
      _stopSyncTimer();
    }
  }

  Future<SyncConfiguration> getConfiguration() async {
    try {
      _configuration = await _localStorageRepository.getSyncConfiguration() ?? _configuration;
      return _configuration;
    } catch (e) {
      return _configuration;
    }
  }

  // Queue Management
  Future<String> addToQueue({
    required SyncOperationType operationType,
    required SyncEntityType entityType,
    required String entityId,
    required Map<String, dynamic> data,
    Map<String, dynamic>? previousData,
    int priority = 5,
    String? dependsOnId,
    Map<String, dynamic> metadata = const {},
    bool requiresOnline = true,
  }) async {
    // Check if entity type is enabled
    if (!_configuration.enabledEntities.contains(entityType)) {
      throw Exception('Sync for entity type $entityType is disabled');
    }

    // Check queue size limit
    final currentQueueSize = await _syncRepository.getQueueSize();
    if (currentQueueSize >= _configuration.maxQueueSize) {
      // Remove oldest completed items to make room
      await _cleanupQueue();
      
      final newQueueSize = await _syncRepository.getQueueSize();
      if (newQueueSize >= _configuration.maxQueueSize) {
        throw Exception('Sync queue is full');
      }
    }

    final queueItem = SyncQueueEntity(
      id: _generateId(),
      operationType: operationType,
      entityType: entityType,
      entityId: entityId,
      data: data,
      previousData: previousData,
      status: SyncStatus.pending,
      createdAt: DateTime.now(),
      priority: priority,
      dependsOnId: dependsOnId,
      metadata: metadata,
      locationId: await _getCurrentLocationId(),
      userId: await _getCurrentUserId(),
      requiresOnline: requiresOnline,
      maxRetries: _configuration.maxRetries,
    );

    await _syncRepository.addToQueue(queueItem);
    _updateQueueSize();

    // Trigger immediate sync if auto-sync is enabled and we're online
    if (_configuration.autoSync && await _isOnline()) {
      // Start sync asynchronously
      // ignore: unawaited_futures
      startSync();
    }

    return queueItem.id;
  }

  Future<void> removeFromQueue(String id) async {
    await _syncRepository.removeFromQueue(id);
    _updateQueueSize();
  }

  Future<List<SyncQueueEntity>> getQueueItems({
    SyncStatus? status,
    SyncEntityType? entityType,
    int? limit,
    int? offset,
  }) async {
    return await _syncRepository.getQueueItems(
      status: status,
      entityType: entityType,
      limit: limit,
      offset: offset,
    );
  }

  Future<void> clearQueue({bool onlyCompleted = true}) async {
    await _syncRepository.clearQueue(onlyCompleted: onlyCompleted);
    _updateQueueSize();
  }

  // Sync Operations
  Future<void> startSync({bool force = false}) async {
    if (_isSyncing && !force) return;
    if (_isPaused && !force) return;

    _isSyncing = true;
    _syncStatusController.add(SyncStatus.inProgress);

    try {
      // Check connectivity and battery
      if (!await _canSync()) {
        _syncStatusController.add(SyncStatus.failed);
        return;
      }

      await _performSync();
      _syncStatusController.add(SyncStatus.completed);
    } catch (e) {
      _syncStatusController.add(SyncStatus.failed);
      rethrow;
    } finally {
      _isSyncing = false;
    }
  }

  Future<void> pauseSync() async {
    _isPaused = true;
    _stopSyncTimer();
  }

  Future<void> resumeSync() async {
    _isPaused = false;
    if (_configuration.autoSync) {
      _startSyncTimer();
    }
  }

  Future<void> _performSync() async {
    // Get pending queue items sorted by priority and dependencies
    final pendingItems = await _getOrderedPendingItems();
    
    if (pendingItems.isEmpty) {
      _syncProgressController.add(1.0);
      return;
    }

    int processedCount = 0;
    final totalCount = pendingItems.length;

    for (final item in pendingItems) {
      try {
        // Check if item is ready to process (dependencies resolved)
        if (item.dependsOnId != null) {
          final dependency = await _syncRepository.getQueueItem(item.dependsOnId!);
          if (dependency?.status != SyncStatus.completed) {
            continue; // Skip for now, will be processed in next cycle
          }
        }

        // Update item status to in-progress
        await _updateQueueItemStatus(item.id, SyncStatus.inProgress);

        // Process the sync operation
        await _processSyncItem(item);

        processedCount++;
        _syncProgressController.add(processedCount / totalCount);
      } catch (e) {
        await _handleSyncError(item, e.toString());
      }
    }
  }

  Future<List<SyncQueueEntity>> _getOrderedPendingItems() async {
    final pendingItems = await _syncRepository.getQueueItems(
      status: SyncStatus.pending,
    );

    // Add items ready for retry
    final retryItems = await _syncRepository.getQueueItems(
      status: SyncStatus.failed,
    );
    
    final readyForRetry = retryItems.where((item) => item.isReadyForRetry).toList();
    
    final allItems = [...pendingItems, ...readyForRetry];

    // Sort by priority (lower number = higher priority) and creation time
    allItems.sort((a, b) {
      final priorityCompare = a.priority.compareTo(b.priority);
      if (priorityCompare != 0) return priorityCompare;
      return a.createdAt.compareTo(b.createdAt);
    });

    return allItems;
  }

  Future<void> _processSyncItem(SyncQueueEntity item) async {
    try {
      switch (item.operationType) {
        case SyncOperationType.create:
          await _syncCreate(item);
          break;
        case SyncOperationType.update:
          await _syncUpdate(item);
          break;
        case SyncOperationType.delete:
          await _syncDelete(item);
          break;
        case SyncOperationType.transfer:
          await _syncTransfer(item);
          break;
        case SyncOperationType.payment:
          await _syncPayment(item);
          break;
      }

      // Mark as completed
      await _updateQueueItemStatus(
        item.id,
        SyncStatus.completed,
        completedAt: DateTime.now(),
      );
    } on ConflictException catch (e) {
      // Handle conflict
      await _handleConflict(item, e);
    } catch (e) {
      await _handleSyncError(item, e.toString());
      rethrow;
    }
  }

  Future<void> _syncCreate(SyncQueueEntity item) async {
    // Save entity remotely
    await _cloudStorageRepository.saveEntity(
      item.entityType,
      item.entityId,
      item.data,
    );
  }

  Future<void> _syncUpdate(SyncQueueEntity item) async {
    // Update entity remotely (save with merge)
    await _cloudStorageRepository.saveEntity(
      item.entityType,
      item.entityId,
      item.data,
    );
  }

  Future<void> _syncDelete(SyncQueueEntity item) async {
    await _cloudStorageRepository.deleteEntity(
      item.entityType,
      item.entityId,
    );
  }

  Future<void> _syncTransfer(SyncQueueEntity item) async {
    // Handle inventory transfers between locations
    await _cloudStorageRepository.saveEntity(
      SyncEntityType.transfer,
      item.entityId,
      item.data,
    );
  }

  Future<void> _syncPayment(SyncQueueEntity item) async {
    // Handle payment processing
    await _cloudStorageRepository.saveEntity(
      SyncEntityType.transaction,
      item.entityId,
      item.data,
    );
  }

  // Conflict Resolution
  Future<void> _handleConflict(SyncQueueEntity item, ConflictException e) async {
    final conflictId = _generateId();
    
    final conflict = SyncConflict(
      id: conflictId,
      queueItemId: item.id,
      entityType: item.entityType,
      entityId: item.entityId,
      localData: e.localData,
      remoteData: e.remoteData,
      conflictFields: _detectConflictFields(e.localData, e.remoteData),
      detectedAt: DateTime.now(),
      suggestedStrategy: _suggestResolutionStrategy(item.entityType, e.localData, e.remoteData),
    );

    // Save conflict for resolution
    await _syncRepository.addConflict(conflict);
    
    // Update queue item status
    await _updateQueueItemStatus(
      item.id,
      SyncStatus.conflict,
      conflictData: conflict.toJson(),
    );

    // Notify conflict stream
    _conflictController.add(conflict);

    // Auto-resolve if strategy is not manual
    if (!_configuration.requireUserApprovalForConflicts &&
        conflict.suggestedStrategy != null &&
        conflict.suggestedStrategy != ConflictResolutionStrategy.manual) {
      await resolveConflict(conflictId, conflict.suggestedStrategy!);
    }
  }

  Future<void> resolveConflict(
    String conflictId,
    ConflictResolutionStrategy strategy, {
    Map<String, dynamic>? customData,
  }) async {
    final conflict = await _syncRepository.getConflict(conflictId);
    if (conflict == null) return;

    final queueItem = await _syncRepository.getQueueItem(conflict.queueItemId);
    if (queueItem == null) return;

    Map<String, dynamic> resolvedData;

    switch (strategy) {
      case ConflictResolutionStrategy.localWins:
        resolvedData = conflict.localData;
        break;
      case ConflictResolutionStrategy.remoteWins:
        resolvedData = conflict.remoteData;
        break;
      case ConflictResolutionStrategy.merge:
        resolvedData = _mergeData(conflict.localData, conflict.remoteData);
        break;
      case ConflictResolutionStrategy.manual:
        if (customData == null) {
          throw Exception('Custom data required for manual resolution');
        }
        resolvedData = customData;
        break;
      case ConflictResolutionStrategy.keepBoth:
        // Create a new entity for the local version
        await _handleKeepBoth(conflict);
        resolvedData = conflict.remoteData;
        break;
    }

    // Update queue item with resolved data
    final updatedItem = queueItem.copyWith(
      data: resolvedData,
      status: SyncStatus.pending,
      resolutionStrategy: strategy,
    );

    await _syncRepository.updateQueueItem(updatedItem);
    await _syncRepository.removeConflict(conflictId);

    // Retry sync for this item
    await _processSyncItem(updatedItem);
  }

  Future<List<SyncConflict>> getPendingConflicts() async {
    return await _syncRepository.getPendingConflicts();
  }

  // Selective Sync
  Future<void> syncSpecificEntity(
    SyncEntityType entityType,
    String entityId, {
    bool force = false,
  }) async {
    if (!_configuration.enabledEntities.contains(entityType) && !force) {
      return;
    }

    // Get local data
    final localData = await _localStorageRepository.getEntity(entityType, entityId);
    if (localData == null) return;

    // Add to queue with high priority
    await addToQueue(
      operationType: SyncOperationType.update,
      entityType: entityType,
      entityId: entityId,
      data: localData,
      priority: 1, // High priority
    );

    // Trigger immediate sync
    await startSync();
  }

  Future<void> syncEntityType(SyncEntityType entityType) async {
    if (!_configuration.enabledEntities.contains(entityType)) {
      return;
    }

    final entities = await _localStorageRepository.getAllEntities(entityType);
    
    for (final entity in entities) {
      await addToQueue(
        operationType: SyncOperationType.update,
        entityType: entityType,
        entityId: entity['id'],
        data: entity,
        priority: 3, // Medium priority
      );
    }

    await startSync();
  }

  Future<void> pullRemoteChanges({
    List<SyncEntityType>? entityTypes,
    DateTime? since,
  }) async {
    final types = entityTypes ?? _configuration.enabledEntities;
    
    for (final entityType in types) {
      await _pullEntityTypeChanges(entityType, since);
    }
  }

  Future<void> _pullEntityTypeChanges(SyncEntityType entityType, DateTime? since) async {
    // Fetch all entities modified since given date
    final entities = await _cloudStorageRepository.getAllEntities(
      entityType,
      modifiedSince: since,
      locationId: await _getCurrentLocationId(),
    );

    for (final entity in entities) {
      await _applyRemoteChange(entityType, {
        'id': entity['id'],
        'data': entity,
      });
    }
  }

  Future<void> _applyRemoteChange(SyncEntityType entityType, Map<String, dynamic> change) async {
    final localEntity = await _localStorageRepository.getEntity(
      entityType,
      change['id'],
    );

    if (localEntity != null) {
      // Check for conflicts
      final conflicts = _detectConflictFields(localEntity, change['data']);
      if (conflicts.isNotEmpty) {
        // Handle conflict
        final conflict = SyncConflict(
          id: _generateId(),
          queueItemId: '',
          entityType: entityType,
          entityId: change['id'],
          localData: localEntity,
          remoteData: change['data'],
          conflictFields: conflicts,
          detectedAt: DateTime.now(),
          suggestedStrategy: _suggestResolutionStrategy(entityType, localEntity, change['data']),
        );

        await _syncRepository.addConflict(conflict);
        _conflictController.add(conflict);
        return;
      }
    }

    // Apply change locally
    await _localStorageRepository.saveEntity(
      entityType,
      change['id'],
      change['data'],
    );
  }

  // Helper Methods
  bool get isSyncing => _isSyncing;
  bool get isPaused => _isPaused;

  Future<bool> _canSync() async {
    // Check connectivity
    if (!await _isOnline()) return false;

    // Check if sync is only on WiFi
    if (_configuration.syncOnlyOnWifi) {
      final connectivityResult = await Connectivity().checkConnectivity();
      if (!connectivityResult.contains(ConnectivityResult.wifi)) {
        return false;
      }
    }

    // Check battery level
    if (!_configuration.syncOnLowBattery) {
      final batteryLevel = await _battery.batteryLevel;
      if (batteryLevel < 20) return false; // Below 20%
    }

    return true;
  }

  Future<bool> _isOnline() async {
    // ConnectivityRepository exposes a boolean getter; wrap in Future
    return _connectivityRepository.isConnected;
  }

  // Public APIs referenced by UI layers
  Future<Map<String, dynamic>> getSyncStatistics() async {
    return await _syncRepository.getSyncStatistics();
  }

  Future<SyncConfiguration> getSyncConfiguration() async {
    return await getConfiguration();
  }

  Future<void> syncAll() async {
    await startSync();
  }

  Future<void> forceSync() async {
    await startSync(force: true);
  }

  Future<Map<String, dynamic>> getSyncAnalytics(String range) async {
    // Basic placeholder analytics aggregated from repository statistics
    final stats = await _syncRepository.getSyncStatistics();
    return {
      'summary': {
        'totalSyncs': stats['totalSyncs'] ?? 0,
        'successRate': stats['successRate'] ?? 0.0,
        'failedSyncs': stats['failedSyncs'] ?? 0,
        'avgDuration': stats['avgDuration'] ?? 0.0,
      },
      'entityBreakdown': stats['entityBreakdown'] ?? <String, dynamic>{},
      'operationBreakdown': stats['operationBreakdown'] ?? <String, dynamic>{},
      'recentActivity': stats['recentActivity'] ?? <dynamic>[],
      'performance': stats['performance'] ?? <String, dynamic>{},
      'errors': stats['errors'] ?? <dynamic>[],
      'network': stats['network'] ?? <String, dynamic>{},
      'trends': stats['trends'] ?? <dynamic>[],
    };
  }

  Future<bool> testConnection() async {
    try {
      return await _isOnline();
    } catch (_) {
      return false;
    }
  }

  Future<Map<String, dynamic>> createBackup() async {
    // Basic backup: export queue and return as map
    final queue = await _syncRepository.exportQueue();
    return {
      'exportedAt': DateTime.now().toIso8601String(),
      'queue': queue,
    };
  }

  Future<void> retryQueueItem(String id) async {
    final item = await _syncRepository.getQueueItem(id);
    if (item == null) return;
    final updated = item.copyWith(
      status: SyncStatus.pending,
      attemptCount: 0,
      errorMessage: null,
      retryAfter: null,
    );
    await _syncRepository.updateQueueItem(updated);
  }

  void _startSyncTimer() {
    _stopSyncTimer();
    _syncTimer = Timer.periodic(_configuration.syncInterval, (timer) {
      if (!_isPaused) {
        startSync();
      }
    });
  }

  void _stopSyncTimer() {
    _syncTimer?.cancel();
    _syncTimer = null;
  }

  Future<void> _updateQueueItemStatus(
    String id,
    SyncStatus status, {
    String? errorMessage,
    DateTime? completedAt,
    Map<String, dynamic>? conflictData,
  }) async {
    final item = await _syncRepository.getQueueItem(id);
    if (item == null) return;

    final updatedItem = item.copyWith(
      status: status,
      lastAttemptAt: DateTime.now(),
      attemptCount: item.attemptCount + 1,
      errorMessage: errorMessage,
      completedAt: completedAt,
      conflictData: conflictData,
      retryAfter: status == SyncStatus.failed 
          ? DateTime.now().add(_configuration.retryBackoffMultiplier * item.attemptCount)
          : null,
    );

    await _syncRepository.updateQueueItem(updatedItem);
  }

  Future<void> _handleSyncError(SyncQueueEntity item, String error) async {
    if (item.canRetry) {
      await _updateQueueItemStatus(
        item.id,
        SyncStatus.failed,
        errorMessage: error,
      );
    } else {
      // Max retries reached, mark as failed permanently
      await _updateQueueItemStatus(
        item.id,
        SyncStatus.failed,
        errorMessage: 'Max retries exceeded: $error',
      );
    }
  }

  void _updateQueueSize() {
    _syncRepository.getQueueSize().then((size) {
      _queueSizeController.add(size);
    });
  }

  Future<void> _cleanupQueue() async {
    // Remove completed items older than 7 days
    final cutoffDate = DateTime.now().subtract(const Duration(days: 7));
    await _syncRepository.cleanupOldItems(cutoffDate);
  }

  // _getCollectionName removed; CloudStorageRepository handles collection paths internally.

  Map<String, dynamic> _detectConflictFields(
    Map<String, dynamic> local,
    Map<String, dynamic> remote,
  ) {
    final conflicts = <String, dynamic>{};
    
    for (final key in local.keys) {
      if (remote.containsKey(key) && local[key] != remote[key]) {
        conflicts[key] = {
          'local': local[key],
          'remote': remote[key],
        };
      }
    }
    
    return conflicts;
  }

  ConflictResolutionStrategy _suggestResolutionStrategy(
    SyncEntityType entityType,
    Map<String, dynamic> localData,
    Map<String, dynamic> remoteData,
  ) {
    // Simple strategy based on timestamps
    final localUpdated = DateTime.tryParse(localData['updatedAt'] ?? '');
    final remoteUpdated = DateTime.tryParse(remoteData['updatedAt'] ?? '');
    
    if (localUpdated != null && remoteUpdated != null) {
      return localUpdated.isAfter(remoteUpdated)
          ? ConflictResolutionStrategy.localWins
          : ConflictResolutionStrategy.remoteWins;
    }
    
    return _configuration.defaultConflictStrategy;
  }

  Map<String, dynamic> _mergeData(
    Map<String, dynamic> local,
    Map<String, dynamic> remote,
  ) {
    final merged = Map<String, dynamic>.from(remote);
    
    // Keep local changes for specific fields that should prioritize local
    final localPriorityFields = ['lastModifiedBy', 'localNotes'];
    
    for (final field in localPriorityFields) {
      if (local.containsKey(field)) {
        merged[field] = local[field];
      }
    }
    
    return merged;
  }

  Future<void> _handleKeepBoth(SyncConflict conflict) async {
    // Create a new entity ID for the local version
    final newEntityId = '${conflict.entityId}_local_${DateTime.now().millisecondsSinceEpoch}';
    
    // Save local version with new ID
    await _localStorageRepository.saveEntity(
      conflict.entityType,
      newEntityId,
      conflict.localData,
    );
  }

  String _generateId() {
    final timestamp = DateTime.now().millisecondsSinceEpoch;
    final bytes = utf8.encode('$timestamp${DateTime.now().microsecond}');
    final digest = sha256.convert(bytes);
    return digest.toString().substring(0, 16);
  }

  Future<String> _getCurrentLocationId() async {
    // Get current location from local storage or configuration
    return await _localStorageRepository.getCurrentLocationId() ?? 'default';
  }

  Future<String> _getCurrentUserId() async {
    // Get current user from local storage or configuration
    return await _localStorageRepository.getCurrentUserId() ?? 'unknown';
  }

  void dispose() {
    _stopSyncTimer();
    _syncStatusController.close();
    _syncProgressController.close();
    _conflictController.close();
    _queueSizeController.close();
  }
}

class ConflictException implements Exception {
  final String message;
  final Map<String, dynamic> localData;
  final Map<String, dynamic> remoteData;

  ConflictException(this.message, {
    required this.localData,
    required this.remoteData,
  });

  @override
  String toString() => 'ConflictException: $message';
}
