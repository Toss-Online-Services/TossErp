import 'dart:async';
import 'package:connectivity_plus/connectivity_plus.dart';
import 'package:sqflite/sqflite.dart';
import '../../data/datasources/local/database_helper.dart';

/// Background sync service for offline-to-online synchronization
class SyncService {
  static final SyncService _instance = SyncService._internal();
  factory SyncService() => _instance;
  SyncService._internal();

  final Connectivity _connectivity = Connectivity();
  StreamSubscription<ConnectivityResult>? _connectivitySubscription;
  
  bool _isSyncing = false;
  DateTime? _lastSyncTime;
  final List<SyncOperation> _syncQueue = [];

  /// Initialize sync service and start monitoring connectivity
  Future<void> initialize() async {
    // Listen to connectivity changes
    _connectivitySubscription = _connectivity.onConnectivityChanged.listen(_onConnectivityChanged);
    
    // Check current connectivity
    final connectivityResult = await _connectivity.checkConnectivity();
    if (connectivityResult != ConnectivityResult.none) {
      await syncPending();
    }
  }

  /// Handle connectivity changes
  void _onConnectivityChanged(ConnectivityResult result) {
    if (result != ConnectivityResult.none && !_isSyncing) {
      syncPending();
    }
  }

  /// Queue an operation for sync
  Future<void> queueOperation(SyncOperation operation) async {
    _syncQueue.add(operation);
    
    // Save to local database
    final db = await DatabaseHelper().database;
    await db.insert('sync_queue', operation.toMap());
    
    // Attempt immediate sync if online
    final connectivityResult = await _connectivity.checkConnectivity();
    if (connectivityResult != ConnectivityResult.none) {
      await syncPending();
    }
  }

  /// Sync all pending operations
  Future<SyncResult> syncPending() async {
    if (_isSyncing) {
      return SyncResult(success: false, message: 'Sync already in progress');
    }

    _isSyncing = true;
    final result = SyncResult(success: true, message: 'Sync started');
    
    try {
      final db = await DatabaseHelper().database;
      final pendingOps = await db.query(
        'sync_queue',
        where: 'status = ?',
        whereArgs: ['pending'],
        orderBy: 'created_at ASC',
      );

      int successCount = 0;
      int failCount = 0;

      for (final opMap in pendingOps) {
        final operation = SyncOperation.fromMap(opMap);
        
        try {
          // Execute sync operation based on type
          final success = await _executeSyncOperation(operation);
          
          if (success) {
            successCount++;
            await db.update(
              'sync_queue',
              {'status': 'completed', 'completed_at': DateTime.now().toIso8601String()},
              where: 'id = ?',
              whereArgs: [operation.id],
            );
          } else {
            failCount++;
            await db.update(
              'sync_queue',
              {'status': 'failed', 'retry_count': operation.retryCount + 1},
              where: 'id = ?',
              whereArgs: [operation.id],
            );
          }
        } catch (e) {
          failCount++;
          print('Error syncing operation ${operation.id}: $e');
        }
      }

      _lastSyncTime = DateTime.now();
      result.message = 'Synced $successCount operations, $failCount failed';
      result.syncedCount = successCount;
      result.failedCount = failCount;
      
      return result;
    } catch (e) {
      result.success = false;
      result.message = 'Sync error: $e';
      return result;
    } finally {
      _isSyncing = false;
    }
  }

  /// Execute a single sync operation
  Future<bool> _executeSyncOperation(SyncOperation operation) async {
    // This would call the appropriate API endpoint based on operation type
    // For now, return success simulation
    
    switch (operation.type) {
      case 'create_sale':
        return await _syncCreateSale(operation);
      case 'update_sale':
        return await _syncUpdateSale(operation);
      case 'create_customer':
        return await _syncCreateCustomer(operation);
      case 'update_inventory':
        return await _syncUpdateInventory(operation);
      default:
        return false;
    }
  }

  Future<bool> _syncCreateSale(SyncOperation operation) async {
    // POST to /api/sales
    // For now, return true
    await Future.delayed(const Duration(milliseconds: 100));
    return true;
  }

  Future<bool> _syncUpdateSale(SyncOperation operation) async {
    // PUT to /api/sales/{id}
    await Future.delayed(const Duration(milliseconds: 100));
    return true;
  }

  Future<bool> _syncCreateCustomer(SyncOperation operation) async {
    // POST to /api/customers
    await Future.delayed(const Duration(milliseconds: 100));
    return true;
  }

  Future<bool> _syncUpdateInventory(SyncOperation operation) async {
    // PUT to /api/inventory/stock-levels
    await Future.delayed(const Duration(milliseconds: 100));
    return true;
  }

  /// Get sync status
  Future<SyncStatus> getStatus() async {
    final db = await DatabaseHelper().database;
    final pending = await db.query('sync_queue', where: 'status = ?', whereArgs: ['pending']);
    final failed = await db.query('sync_queue', where: 'status = ?', whereArgs: ['failed']);
    
    final connectivityResult = await _connectivity.checkConnectivity();
    
    return SyncStatus(
      isOnline: connectivityResult != ConnectivityResult.none,
      isSyncing: _isSyncing,
      pendingCount: pending.length,
      failedCount: failed.length,
      lastSyncTime: _lastSyncTime,
    );
  }

  /// Clear completed sync operations (older than 7 days)
  Future<void> cleanupOldOperations() async {
    final db = await DatabaseHelper().database;
    final cutoffDate = DateTime.now().subtract(const Duration(days: 7));
    
    await db.delete(
      'sync_queue',
      where: 'status = ? AND completed_at < ?',
      whereArgs: ['completed', cutoffDate.toIso8601String()],
    );
  }

  /// Dispose resources
  void dispose() {
    _connectivitySubscription?.cancel();
  }
}

/// Sync operation model
class SyncOperation {
  final String? id;
  final String type;
  final Map<String, dynamic> data;
  final String status;
  final int retryCount;
  final DateTime createdAt;
  final DateTime? completedAt;

  SyncOperation({
    this.id,
    required this.type,
    required this.data,
    this.status = 'pending',
    this.retryCount = 0,
    DateTime? createdAt,
    this.completedAt,
  }) : createdAt = createdAt ?? DateTime.now();

  Map<String, dynamic> toMap() {
    return {
      'id': id,
      'type': type,
      'data': data.toString(), // Would use JSON encoding
      'status': status,
      'retry_count': retryCount,
      'created_at': createdAt.toIso8601String(),
      'completed_at': completedAt?.toIso8601String(),
    };
  }

  factory SyncOperation.fromMap(Map<String, dynamic> map) {
    return SyncOperation(
      id: map['id']?.toString(),
      type: map['type'] as String,
      data: {}, // Would parse JSON
      status: map['status'] as String,
      retryCount: map['retry_count'] as int? ?? 0,
      createdAt: DateTime.parse(map['created_at'] as String),
      completedAt: map['completed_at'] != null 
        ? DateTime.parse(map['completed_at'] as String) 
        : null,
    );
  }
}

/// Sync result model
class SyncResult {
  bool success;
  String message;
  int syncedCount;
  int failedCount;

  SyncResult({
    required this.success,
    required this.message,
    this.syncedCount = 0,
    this.failedCount = 0,
  });
}

/// Sync status model
class SyncStatus {
  final bool isOnline;
  final bool isSyncing;
  final int pendingCount;
  final int failedCount;
  final DateTime? lastSyncTime;

  SyncStatus({
    required this.isOnline,
    required this.isSyncing,
    required this.pendingCount,
    required this.failedCount,
    this.lastSyncTime,
  });
}

