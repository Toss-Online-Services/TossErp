import 'dart:convert';
import 'package:sqflite/sqflite.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

import '../../domain/entities/sync_entity.dart';
import '../database/database_helper.dart';

class LocalStorageRepository {
  final DatabaseHelper _databaseHelper;
  final FlutterSecureStorage _secureStorage = const FlutterSecureStorage();

  LocalStorageRepository(this._databaseHelper);

  // Sync Configuration
  Future<void> saveSyncConfiguration(SyncConfiguration configuration) async {
    await _secureStorage.write(
      key: 'sync_configuration',
      value: jsonEncode(configuration.toJson()),
    );
  }

  Future<SyncConfiguration?> getSyncConfiguration() async {
    final configJson = await _secureStorage.read(key: 'sync_configuration');
    if (configJson != null) {
      return SyncConfiguration.fromJson(jsonDecode(configJson));
    }
    return null;
  }

  // Entity Management
  Future<void> saveEntity(
    SyncEntityType entityType,
    String entityId,
    Map<String, dynamic> data,
  ) async {
    final db = await _databaseHelper.database;
    final tableName = _getTableName(entityType);
    
    // Add metadata
    data['entityId'] = entityId;
    data['entityType'] = entityType.name;
    data['lastSyncAt'] = DateTime.now().toIso8601String();
    
    await db.insert(
      tableName,
      data,
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
  }

  Future<Map<String, dynamic>?> getEntity(
    SyncEntityType entityType,
    String entityId,
  ) async {
    final db = await _databaseHelper.database;
    final tableName = _getTableName(entityType);
    
    final result = await db.query(
      tableName,
      where: 'id = ?',
      whereArgs: [entityId],
      limit: 1,
    );

    return result.isNotEmpty ? result.first : null;
  }

  Future<List<Map<String, dynamic>>> getAllEntities(
    SyncEntityType entityType, {
    DateTime? modifiedSince,
    int? limit,
    int? offset,
  }) async {
    final db = await _databaseHelper.database;
    final tableName = _getTableName(entityType);
    
    String? whereClause;
    List<dynamic>? whereArgs;
    
    if (modifiedSince != null) {
      whereClause = 'updatedAt > ?';
      whereArgs = [modifiedSince.toIso8601String()];
    }
    
    final result = await db.query(
      tableName,
      where: whereClause,
      whereArgs: whereArgs,
      orderBy: 'updatedAt DESC',
      limit: limit,
      offset: offset,
    );

    return result;
  }

  Future<void> deleteEntity(
    SyncEntityType entityType,
    String entityId,
  ) async {
    final db = await _databaseHelper.database;
    final tableName = _getTableName(entityType);
    
    await db.delete(
      tableName,
      where: 'id = ?',
      whereArgs: [entityId],
    );
  }

  Future<void> markEntityAsSynced(
    SyncEntityType entityType,
    String entityId,
  ) async {
    final db = await _databaseHelper.database;
    final tableName = _getTableName(entityType);
    
    await db.update(
      tableName,
      {'lastSyncAt': DateTime.now().toIso8601String()},
      where: 'id = ?',
      whereArgs: [entityId],
    );
  }

  // Offline Data Management
  Future<List<Map<String, dynamic>>> getUnsyncedEntities(
    SyncEntityType entityType,
  ) async {
    final db = await _databaseHelper.database;
    final tableName = _getTableName(entityType);
    
    // Get entities that have been modified since last sync
    final result = await db.query(
      tableName,
      where: 'updatedAt > lastSyncAt OR lastSyncAt IS NULL',
      orderBy: 'updatedAt ASC',
    );

    return result;
  }

  Future<int> getUnsyncedEntityCount(SyncEntityType entityType) async {
    final db = await _databaseHelper.database;
    final tableName = _getTableName(entityType);
    
    final result = await db.rawQuery(
      'SELECT COUNT(*) as count FROM $tableName WHERE updatedAt > lastSyncAt OR lastSyncAt IS NULL',
    );

    return Sqflite.firstIntValue(result) ?? 0;
  }

  // User and Location Management
  Future<String?> getCurrentUserId() async {
    return await _secureStorage.read(key: 'current_user_id');
  }

  Future<void> setCurrentUserId(String userId) async {
    await _secureStorage.write(key: 'current_user_id', value: userId);
  }

  Future<String?> getCurrentLocationId() async {
    return await _secureStorage.read(key: 'current_location_id');
  }

  Future<void> setCurrentLocationId(String locationId) async {
    await _secureStorage.write(key: 'current_location_id', value: locationId);
  }

  // Cache Management
  Future<void> cacheData(String key, Map<String, dynamic> data) async {
    final db = await _databaseHelper.database;
    
    await db.insert(
      'cache',
      {
        'key': key,
        'data': jsonEncode(data),
        'createdAt': DateTime.now().toIso8601String(),
        'expiresAt': DateTime.now().add(const Duration(hours: 24)).toIso8601String(),
      },
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
  }

  Future<Map<String, dynamic>?> getCachedData(String key) async {
    final db = await _databaseHelper.database;
    
    final result = await db.query(
      'cache',
      where: 'key = ? AND expiresAt > ?',
      whereArgs: [key, DateTime.now().toIso8601String()],
      limit: 1,
    );

    if (result.isNotEmpty) {
      return jsonDecode(result.first['data'] as String);
    }
    return null;
  }

  Future<void> clearExpiredCache() async {
    final db = await _databaseHelper.database;
    
    await db.delete(
      'cache',
      where: 'expiresAt <= ?',
      whereArgs: [DateTime.now().toIso8601String()],
    );
  }

  Future<void> clearAllCache() async {
    final db = await _databaseHelper.database;
    await db.delete('cache');
  }

  // Metadata Management
  Future<void> saveMetadata(String key, dynamic value) async {
    await _secureStorage.write(
      key: 'metadata_$key',
      value: jsonEncode(value),
    );
  }

  Future<T?> getMetadata<T>(String key) async {
    final value = await _secureStorage.read(key: 'metadata_$key');
    if (value != null) {
      return jsonDecode(value) as T;
    }
    return null;
  }

  Future<void> deleteMetadata(String key) async {
    await _secureStorage.delete(key: 'metadata_$key');
  }

  // Backup and Restore
  Future<Map<String, dynamic>> createBackup(List<SyncEntityType> entityTypes) async {
    final backup = <String, dynamic>{
      'version': '1.0',
      'createdAt': DateTime.now().toIso8601String(),
      'entities': <String, List<Map<String, dynamic>>>{},
    };

    for (final entityType in entityTypes) {
      final entities = await getAllEntities(entityType);
      backup['entities'][entityType.name] = entities;
    }

    return backup;
  }

  Future<void> restoreFromBackup(Map<String, dynamic> backup) async {
    final db = await _databaseHelper.database;
    final batch = db.batch();

    final entities = backup['entities'] as Map<String, dynamic>;
    
    for (final entry in entities.entries) {
      final entityTypeName = entry.key;
      final entityData = entry.value as List<dynamic>;
      
      try {
        final entityType = SyncEntityType.values.firstWhere(
          (e) => e.name == entityTypeName,
        );
        final tableName = _getTableName(entityType);
        
        // Clear existing data for this entity type
        batch.delete(tableName);
        
        // Insert backup data
        for (final entity in entityData) {
          batch.insert(tableName, entity as Map<String, dynamic>);
        }
      } catch (e) {
        // Skip unknown entity types
        continue;
      }
    }

    await batch.commit(noResult: true);
  }

  // Storage Statistics
  Future<Map<String, dynamic>> getStorageStatistics() async {
    final db = await _databaseHelper.database;
    final stats = <String, dynamic>{};

    for (final entityType in SyncEntityType.values) {
      try {
        final tableName = _getTableName(entityType);
        
        final countResult = await db.rawQuery(
          'SELECT COUNT(*) as count FROM $tableName',
        );
        
        final unsyncedCountResult = await db.rawQuery(
          'SELECT COUNT(*) as count FROM $tableName WHERE updatedAt > lastSyncAt OR lastSyncAt IS NULL',
        );
        
        stats[entityType.name] = {
          'total': Sqflite.firstIntValue(countResult) ?? 0,
          'unsynced': Sqflite.firstIntValue(unsyncedCountResult) ?? 0,
        };
      } catch (e) {
        // Table might not exist
        stats[entityType.name] = {
          'total': 0,
          'unsynced': 0,
        };
      }
    }

    // Cache statistics
    final cacheCountResult = await db.rawQuery(
      'SELECT COUNT(*) as count FROM cache',
    );
    
    final expiredCacheCountResult = await db.rawQuery(
      'SELECT COUNT(*) as count FROM cache WHERE expiresAt <= ?',
      [DateTime.now().toIso8601String()],
    );

    stats['cache'] = {
      'total': Sqflite.firstIntValue(cacheCountResult) ?? 0,
      'expired': Sqflite.firstIntValue(expiredCacheCountResult) ?? 0,
    };

    return stats;
  }

  // Data Validation
  Future<List<String>> validateDataIntegrity() async {
    final issues = <String>[];
    final db = await _databaseHelper.database;

    for (final entityType in SyncEntityType.values) {
      try {
        final tableName = _getTableName(entityType);
        
        // Check for duplicate IDs
        final duplicateResult = await db.rawQuery('''
          SELECT id, COUNT(*) as count 
          FROM $tableName 
          GROUP BY id 
          HAVING COUNT(*) > 1
        ''');
        
        if (duplicateResult.isNotEmpty) {
          issues.add('Duplicate IDs found in $tableName: ${duplicateResult.length} conflicts');
        }
        
        // Check for invalid dates
        final invalidDateResult = await db.rawQuery('''
          SELECT COUNT(*) as count 
          FROM $tableName 
          WHERE createdAt IS NULL OR updatedAt IS NULL
        ''');
        
        final invalidDateCount = Sqflite.firstIntValue(invalidDateResult) ?? 0;
        if (invalidDateCount > 0) {
          issues.add('Invalid dates found in $tableName: $invalidDateCount records');
        }
        
      } catch (e) {
        // Table might not exist
        continue;
      }
    }

    return issues;
  }

  Future<void> repairDataIntegrity() async {
    final db = await _databaseHelper.database;

    for (final entityType in SyncEntityType.values) {
      try {
        final tableName = _getTableName(entityType);
        
        // Remove duplicate records (keep the most recent)
        await db.execute('''
          DELETE FROM $tableName 
          WHERE rowid NOT IN (
            SELECT MIN(rowid) 
            FROM $tableName 
            GROUP BY id
          )
        ''');
        
        // Fix null dates
        final now = DateTime.now().toIso8601String();
        await db.update(
          tableName,
          {
            'createdAt': now,
            'updatedAt': now,
          },
          where: 'createdAt IS NULL OR updatedAt IS NULL',
        );
        
      } catch (e) {
        // Table might not exist
        continue;
      }
    }
  }

  // Helper Methods
  String _getTableName(SyncEntityType entityType) {
    switch (entityType) {
      case SyncEntityType.transaction:
        return 'transactions';
      case SyncEntityType.product:
        return 'products';
      case SyncEntityType.customer:
        return 'customers';
      case SyncEntityType.inventory:
        return 'inventory';
      case SyncEntityType.discount:
        return 'discounts';
      case SyncEntityType.employee:
        return 'employees';
      case SyncEntityType.location:
        return 'locations';
      case SyncEntityType.transfer:
        return 'transfers';
    }
  }

  // Bulk Operations
  Future<void> saveEntitiesBatch(
    SyncEntityType entityType,
    List<Map<String, dynamic>> entities,
  ) async {
    final db = await _databaseHelper.database;
    final tableName = _getTableName(entityType);
    final batch = db.batch();

    for (final entity in entities) {
      entity['entityType'] = entityType.name;
      entity['lastSyncAt'] = DateTime.now().toIso8601String();
      
      batch.insert(
        tableName,
        entity,
        conflictAlgorithm: ConflictAlgorithm.replace,
      );
    }

    await batch.commit(noResult: true);
  }

  Future<void> deleteEntitiesBatch(
    SyncEntityType entityType,
    List<String> entityIds,
  ) async {
    final db = await _databaseHelper.database;
    final tableName = _getTableName(entityType);
    final batch = db.batch();

    for (final entityId in entityIds) {
      batch.delete(
        tableName,
        where: 'id = ?',
        whereArgs: [entityId],
      );
    }

    await batch.commit(noResult: true);
  }

  // Search and Query
  Future<List<Map<String, dynamic>>> searchEntities(
    SyncEntityType entityType,
    String searchTerm, {
    List<String>? searchFields,
    int? limit,
  }) async {
    final db = await _databaseHelper.database;
    final tableName = _getTableName(entityType);
    
    final fields = searchFields ?? ['name', 'description'];
    final whereConditions = fields.map((field) => '$field LIKE ?').join(' OR ');
    final whereArgs = fields.map((field) => '%$searchTerm%').toList();

    final result = await db.query(
      tableName,
      where: whereConditions,
      whereArgs: whereArgs,
      orderBy: 'updatedAt DESC',
      limit: limit,
    );

    return result;
  }

  Future<List<Map<String, dynamic>>> queryEntities(
    SyncEntityType entityType, {
    String? where,
    List<dynamic>? whereArgs,
    String? orderBy,
    int? limit,
    int? offset,
  }) async {
    final db = await _databaseHelper.database;
    final tableName = _getTableName(entityType);

    final result = await db.query(
      tableName,
      where: where,
      whereArgs: whereArgs,
      orderBy: orderBy,
      limit: limit,
      offset: offset,
    );

    return result;
  }

  // Database Maintenance
  Future<void> vacuum() async {
    final db = await _databaseHelper.database;
    await db.execute('VACUUM');
  }

  Future<int> getDatabaseSize() async {
    final db = await _databaseHelper.database;
    final result = await db.rawQuery('PRAGMA page_count');
    final pageCount = Sqflite.firstIntValue(result) ?? 0;
    
    final pageSizeResult = await db.rawQuery('PRAGMA page_size');
    final pageSize = Sqflite.firstIntValue(pageSizeResult) ?? 0;
    
    return pageCount * pageSize;
  }

  Future<void> optimize() async {
    await clearExpiredCache();
    await vacuum();
  }
}
