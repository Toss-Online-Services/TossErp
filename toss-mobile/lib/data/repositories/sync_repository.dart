import 'dart:convert';
import 'package:sqflite/sqflite.dart';

import '../../domain/entities/sync_entity.dart';
import '../database/database_helper.dart';

class SyncRepository {
  final DatabaseHelper _databaseHelper;

  SyncRepository(this._databaseHelper);

  // Queue Management
  Future<void> addToQueue(SyncQueueEntity queueItem) async {
    final db = await _databaseHelper.database;
    await db.insert(
      'sync_queue',
      queueItem.toJson(),
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
  }

  Future<void> updateQueueItem(SyncQueueEntity queueItem) async {
    final db = await _databaseHelper.database;
    await db.update(
      'sync_queue',
      queueItem.toJson(),
      where: 'id = ?',
      whereArgs: [queueItem.id],
    );
  }

  Future<void> removeFromQueue(String id) async {
    final db = await _databaseHelper.database;
    await db.delete(
      'sync_queue',
      where: 'id = ?',
      whereArgs: [id],
    );
  }

  Future<SyncQueueEntity?> getQueueItem(String id) async {
    final db = await _databaseHelper.database;
    final result = await db.query(
      'sync_queue',
      where: 'id = ?',
      whereArgs: [id],
      limit: 1,
    );

    if (result.isNotEmpty) {
      return SyncQueueEntity.fromJson(result.first);
    }
    return null;
  }

  Future<List<SyncQueueEntity>> getQueueItems({
    SyncStatus? status,
    SyncEntityType? entityType,
    int? limit,
    int? offset,
  }) async {
    final db = await _databaseHelper.database;
    
    String whereClause = '';
    List<dynamic> whereArgs = [];

    if (status != null) {
      whereClause = 'status = ?';
      whereArgs.add(status.name);
    }

    if (entityType != null) {
      if (whereClause.isNotEmpty) whereClause += ' AND ';
      whereClause += 'entityType = ?';
      whereArgs.add(entityType.name);
    }

    final result = await db.query(
      'sync_queue',
      where: whereClause.isNotEmpty ? whereClause : null,
      whereArgs: whereArgs.isNotEmpty ? whereArgs : null,
      orderBy: 'priority ASC, createdAt ASC',
      limit: limit,
      offset: offset,
    );

    return result.map((json) => SyncQueueEntity.fromJson(json)).toList();
  }

  Future<int> getQueueSize() async {
    final db = await _databaseHelper.database;
    final result = await db.rawQuery('SELECT COUNT(*) as count FROM sync_queue');
    return Sqflite.firstIntValue(result) ?? 0;
  }

  Future<void> clearQueue({bool onlyCompleted = true}) async {
    final db = await _databaseHelper.database;
    
    if (onlyCompleted) {
      await db.delete(
        'sync_queue',
        where: 'status = ?',
        whereArgs: [SyncStatus.completed.name],
      );
    } else {
      await db.delete('sync_queue');
    }
  }

  Future<void> cleanupOldItems(DateTime cutoffDate) async {
    final db = await _databaseHelper.database;
    await db.delete(
      'sync_queue',
      where: 'status = ? AND completedAt < ?',
      whereArgs: [SyncStatus.completed.name, cutoffDate.toIso8601String()],
    );
  }

  // Conflict Management
  Future<void> addConflict(SyncConflict conflict) async {
    final db = await _databaseHelper.database;
    await db.insert(
      'sync_conflicts',
      conflict.toJson(),
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
  }

  Future<void> removeConflict(String id) async {
    final db = await _databaseHelper.database;
    await db.delete(
      'sync_conflicts',
      where: 'id = ?',
      whereArgs: [id],
    );
  }

  Future<SyncConflict?> getConflict(String id) async {
    final db = await _databaseHelper.database;
    final result = await db.query(
      'sync_conflicts',
      where: 'id = ?',
      whereArgs: [id],
      limit: 1,
    );

    if (result.isNotEmpty) {
      return SyncConflict.fromJson(result.first);
    }
    return null;
  }

  Future<List<SyncConflict>> getPendingConflicts() async {
    final db = await _databaseHelper.database;
    final result = await db.query(
      'sync_conflicts',
      orderBy: 'detectedAt DESC',
    );

    return result.map((json) => SyncConflict.fromJson(json)).toList();
  }

  Future<void> clearResolvedConflicts() async {
    final db = await _databaseHelper.database;
    
    // Get conflicts that have been resolved (their queue items are completed)
    final resolvedConflicts = await db.rawQuery('''
      SELECT c.id 
      FROM sync_conflicts c
      JOIN sync_queue q ON c.queueItemId = q.id
      WHERE q.status = ?
    ''', [SyncStatus.completed.name]);

    for (final conflict in resolvedConflicts) {
      await removeConflict(conflict['id'] as String);
    }
  }

  // Statistics
  Future<Map<String, dynamic>> getSyncStatistics() async {
    final db = await _databaseHelper.database;
    
    // Queue statistics
    final queueStats = await db.rawQuery('''
      SELECT 
        status,
        COUNT(*) as count
      FROM sync_queue 
      GROUP BY status
    ''');

    // Entity type statistics
    final entityStats = await db.rawQuery('''
      SELECT 
        entityType,
        COUNT(*) as count
      FROM sync_queue 
      GROUP BY entityType
    ''');

    // Conflict statistics
    final conflictCount = await db.rawQuery('''
      SELECT COUNT(*) as count FROM sync_conflicts
    ''');

    // Recent activity (last 24 hours)
    final yesterday = DateTime.now().subtract(const Duration(days: 1));
    final recentActivity = await db.rawQuery('''
      SELECT COUNT(*) as count 
      FROM sync_queue 
      WHERE createdAt > ?
    ''', [yesterday.toIso8601String()]);

    return {
      'queueStats': Map.fromIterable(
        queueStats,
        key: (item) => item['status'],
        value: (item) => item['count'],
      ),
      'entityStats': Map.fromIterable(
        entityStats,
        key: (item) => item['entityType'],
        value: (item) => item['count'],
      ),
      'conflictCount': Sqflite.firstIntValue(conflictCount) ?? 0,
      'recentActivity': Sqflite.firstIntValue(recentActivity) ?? 0,
    };
  }

  // Dependency Management
  Future<List<SyncQueueEntity>> getDependentItems(String parentId) async {
    final db = await _databaseHelper.database;
    final result = await db.query(
      'sync_queue',
      where: 'dependsOnId = ?',
      whereArgs: [parentId],
      orderBy: 'priority ASC, createdAt ASC',
    );

    return result.map((json) => SyncQueueEntity.fromJson(json)).toList();
  }

  Future<bool> hasPendingDependencies(String itemId) async {
    final item = await getQueueItem(itemId);
    if (item?.dependsOnId == null) return false;

    final dependency = await getQueueItem(item!.dependsOnId!);
    return dependency?.status != SyncStatus.completed;
  }

  // Retry Management
  Future<List<SyncQueueEntity>> getRetryableItems() async {
    final db = await _databaseHelper.database;
    final now = DateTime.now().toIso8601String();
    
    final result = await db.query(
      'sync_queue',
      where: 'status = ? AND attemptCount < maxRetries AND (retryAfter IS NULL OR retryAfter <= ?)',
      whereArgs: [SyncStatus.failed.name, now],
      orderBy: 'priority ASC, createdAt ASC',
    );

    return result.map((json) => SyncQueueEntity.fromJson(json)).toList();
  }

  Future<void> resetFailedItems() async {
    final db = await _databaseHelper.database;
    await db.update(
      'sync_queue',
      {
        'status': SyncStatus.pending.name,
        'attemptCount': 0,
        'errorMessage': null,
        'retryAfter': null,
      },
      where: 'status = ?',
      whereArgs: [SyncStatus.failed.name],
    );
  }

  // Batch Operations
  Future<void> addBatchToQueue(List<SyncQueueEntity> items) async {
    final db = await _databaseHelper.database;
    final batch = db.batch();

    for (final item in items) {
      batch.insert(
        'sync_queue',
        item.toJson(),
        conflictAlgorithm: ConflictAlgorithm.replace,
      );
    }

    await batch.commit(noResult: true);
  }

  Future<void> updateBatchStatus(
    List<String> itemIds,
    SyncStatus status, {
    String? errorMessage,
    DateTime? completedAt,
  }) async {
    final db = await _databaseHelper.database;
    final batch = db.batch();

    final updateData = <String, dynamic>{
      'status': status.name,
      'lastAttemptAt': DateTime.now().toIso8601String(),
    };

    if (errorMessage != null) {
      updateData['errorMessage'] = errorMessage;
    }

    if (completedAt != null) {
      updateData['completedAt'] = completedAt.toIso8601String();
    }

    for (final itemId in itemIds) {
      batch.update(
        'sync_queue',
        updateData,
        where: 'id = ?',
        whereArgs: [itemId],
      );
    }

    await batch.commit(noResult: true);
  }

  // Priority Management
  Future<void> updateItemPriority(String itemId, int priority) async {
    final db = await _databaseHelper.database;
    await db.update(
      'sync_queue',
      {'priority': priority},
      where: 'id = ?',
      whereArgs: [itemId],
    );
  }

  Future<void> boostPriority(List<String> itemIds) async {
    final db = await _databaseHelper.database;
    final batch = db.batch();

    for (final itemId in itemIds) {
      batch.update(
        'sync_queue',
        {'priority': 1}, // Highest priority
        where: 'id = ?',
        whereArgs: [itemId],
      );
    }

    await batch.commit(noResult: true);
  }

  // Search and Filtering
  Future<List<SyncQueueEntity>> searchQueueItems({
    String? entityId,
    SyncEntityType? entityType,
    SyncOperationType? operationType,
    SyncStatus? status,
    DateTime? startDate,
    DateTime? endDate,
    int? limit,
    int? offset,
  }) async {
    final db = await _databaseHelper.database;
    
    final whereConditions = <String>[];
    final whereArgs = <dynamic>[];

    if (entityId != null) {
      whereConditions.add('entityId LIKE ?');
      whereArgs.add('%$entityId%');
    }

    if (entityType != null) {
      whereConditions.add('entityType = ?');
      whereArgs.add(entityType.name);
    }

    if (operationType != null) {
      whereConditions.add('operationType = ?');
      whereArgs.add(operationType.name);
    }

    if (status != null) {
      whereConditions.add('status = ?');
      whereArgs.add(status.name);
    }

    if (startDate != null) {
      whereConditions.add('createdAt >= ?');
      whereArgs.add(startDate.toIso8601String());
    }

    if (endDate != null) {
      whereConditions.add('createdAt <= ?');
      whereArgs.add(endDate.toIso8601String());
    }

    final result = await db.query(
      'sync_queue',
      where: whereConditions.isNotEmpty ? whereConditions.join(' AND ') : null,
      whereArgs: whereArgs.isNotEmpty ? whereArgs : null,
      orderBy: 'createdAt DESC',
      limit: limit,
      offset: offset,
    );

    return result.map((json) => SyncQueueEntity.fromJson(json)).toList();
  }

  // Export/Import
  Future<List<Map<String, dynamic>>> exportQueue() async {
    final db = await _databaseHelper.database;
    return await db.query('sync_queue');
  }

  Future<void> importQueue(List<Map<String, dynamic>> queueData) async {
    final db = await _databaseHelper.database;
    final batch = db.batch();

    // Clear existing queue
    batch.delete('sync_queue');

    // Import new data
    for (final item in queueData) {
      batch.insert('sync_queue', item);
    }

    await batch.commit(noResult: true);
  }
}
