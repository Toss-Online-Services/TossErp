import 'dart:async';

import 'package:sqflite/sqflite.dart';

import '../../../app/database/app_database.dart';
import '../../../domain/entities/inventory_movement_entity.dart';
import '../../datasources/interfaces/inventory_movement_datasource.dart';

class InventoryMovementLocalDatasourceImpl implements InventoryMovementDatasource {
  final AppDatabase _appDatabase;

  InventoryMovementLocalDatasourceImpl(this._appDatabase);

  @override
  Future<int> createInventoryMovement(InventoryMovementEntity movement) async {
    final id = await _appDatabase.database.insert(
      AppDatabaseConfig.inventoryMovementTableName,
      _movementToMap(movement),
      conflictAlgorithm: ConflictAlgorithm.replace,
    );
    return id;
  }

  @override
  Future<List<InventoryMovementEntity>> getAllInventoryMovements() async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.inventoryMovementTableName,
      orderBy: 'createdAt DESC',
    );
    return res.map((e) => _mapToMovement(e)).toList();
  }

  @override
  Future<List<InventoryMovementEntity>> getInventoryMovementsByProduct(String productId) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.inventoryMovementTableName,
      where: 'productId = ?',
      whereArgs: [productId],
      orderBy: 'createdAt DESC',
    );
    return res.map((e) => _mapToMovement(e)).toList();
  }

  @override
  Future<List<InventoryMovementEntity>> getInventoryMovementsByType(MovementType type) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.inventoryMovementTableName,
      where: 'type = ?',
      whereArgs: [type.name],
      orderBy: 'createdAt DESC',
    );
    return res.map((e) => _mapToMovement(e)).toList();
  }

  @override
  Future<List<InventoryMovementEntity>> getInventoryMovementsByDateRange(
    DateTime startDate,
    DateTime endDate,
  ) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.inventoryMovementTableName,
      where: 'createdAt BETWEEN ? AND ?',
      whereArgs: [
        startDate.toIso8601String(),
        endDate.toIso8601String(),
      ],
      orderBy: 'createdAt DESC',
    );
    return res.map((e) => _mapToMovement(e)).toList();
  }

  @override
  Future<List<InventoryMovementEntity>> getInventoryMovementsByLocation(String locationId) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.inventoryMovementTableName,
      where: 'fromLocationId = ? OR toLocationId = ?',
      whereArgs: [locationId, locationId],
      orderBy: 'createdAt DESC',
    );
    return res.map((e) => _mapToMovement(e)).toList();
  }

  @override
  Future<InventoryMovementEntity?> getInventoryMovementById(int id) async {
    final res = await _appDatabase.database.query(
      AppDatabaseConfig.inventoryMovementTableName,
      where: 'id = ?',
      whereArgs: [id],
    );

    if (res.isEmpty) return null;
    return _mapToMovement(res.first);
  }

  @override
  Future<void> updateInventoryMovement(InventoryMovementEntity movement) async {
    await _appDatabase.database.update(
      AppDatabaseConfig.inventoryMovementTableName,
      _movementToMap(movement),
      where: 'id = ?',
      whereArgs: [movement.id],
    );
  }

  @override
  Future<void> deleteInventoryMovement(int id) async {
    await _appDatabase.database.delete(
      AppDatabaseConfig.inventoryMovementTableName,
      where: 'id = ?',
      whereArgs: [id],
    );
  }

  @override
  Future<List<InventoryMovementEntity>> getLowStockMovements() async {
    // Get movements that resulted in low stock (quantity <= reorder point)
    final res = await _appDatabase.database.rawQuery('''
      SELECT im.* FROM ${AppDatabaseConfig.inventoryMovementTableName} im
      JOIN ${AppDatabaseConfig.productTableName} p ON im.productId = p.id
      WHERE im.type = 'sale' AND p.currentStock <= p.reorderPoint
      ORDER BY im.createdAt DESC
    ''');
    return res.map((e) => _mapToMovement(e)).toList();
  }

  @override
  Future<List<InventoryMovementEntity>> getExpiredProductMovements() async {
    final res = await _appDatabase.database.rawQuery('''
      SELECT im.* FROM ${AppDatabaseConfig.inventoryMovementTableName} im
      JOIN ${AppDatabaseConfig.productTableName} p ON im.productId = p.id
      WHERE im.type = 'adjustment' AND im.reason = 'expired'
      ORDER BY im.createdAt DESC
    ''');
    return res.map((e) => _mapToMovement(e)).toList();
  }

  @override
  Future<Map<String, double>> getInventoryValuation() async {
    final res = await _appDatabase.database.rawQuery('''
      SELECT 
        p.category,
        SUM(p.currentStock * p.costPrice) as totalValue
      FROM ${AppDatabaseConfig.productTableName} p
      WHERE p.currentStock > 0
      GROUP BY p.category
    ''');
    
    final Map<String, double> valuation = {};
    for (final row in res) {
      valuation[row['category'] as String] = (row['totalValue'] as num).toDouble();
    }
    return valuation;
  }

  @override
  Future<List<Map<String, dynamic>>> getInventoryTurnoverReport() async {
    final res = await _appDatabase.database.rawQuery('''
      SELECT 
        p.id,
        p.name,
        p.category,
        p.currentStock,
        p.costPrice,
        COALESCE(SUM(CASE WHEN im.type = 'sale' THEN im.quantity ELSE 0 END), 0) as totalSold,
        COALESCE(SUM(CASE WHEN im.type = 'purchase' THEN im.quantity ELSE 0 END), 0) as totalPurchased,
        CASE 
          WHEN p.currentStock > 0 THEN 
            COALESCE(SUM(CASE WHEN im.type = 'sale' THEN im.quantity ELSE 0 END), 0) / p.currentStock
          ELSE 0 
        END as turnoverRatio
      FROM ${AppDatabaseConfig.productTableName} p
      LEFT JOIN ${AppDatabaseConfig.inventoryMovementTableName} im ON p.id = im.productId
      WHERE im.createdAt >= datetime('now', '-30 days') OR im.createdAt IS NULL
      GROUP BY p.id, p.name, p.category, p.currentStock, p.costPrice
      ORDER BY turnoverRatio DESC
    ''');
    
    return res.map((row) => Map<String, dynamic>.from(row)).toList();
  }

  @override
  Future<void> seedSampleInventoryMovements() async {
    final sampleMovements = [
      InventoryMovementEntity(
        id: DateTime.now().millisecondsSinceEpoch,
        productId: 1,
        batchId: 1,
        type: MovementType.purchase,
        reason: MovementReason.purchase,
        quantity: 100,
        unitPrice: 1000, // 10.0 in cents
        totalValue: 100000, // 1000.0 in cents
        referenceId: 1,
        referenceType: 'purchase_order',
        notes: 'Initial stock purchase',
        fromLocationId: null,
        toLocationId: 1,
        createdById: 'user1',
        createdAt: DateTime.now().subtract(const Duration(days: 30)),
        updatedAt: DateTime.now().subtract(const Duration(days: 30)).toIso8601String(),
      ),
      InventoryMovementEntity(
        id: DateTime.now().millisecondsSinceEpoch + 1,
        productId: 1,
        batchId: 1,
        type: MovementType.sale,
        reason: MovementReason.sale,
        quantity: 5,
        unitPrice: 1500, // 15.0 in cents
        totalValue: 7500, // 75.0 in cents
        referenceId: 1,
        referenceType: 'transaction',
        notes: 'Regular sale',
        fromLocationId: 1,
        toLocationId: null,
        createdById: 'user1',
        createdAt: DateTime.now().subtract(const Duration(days: 1)),
        updatedAt: DateTime.now().subtract(const Duration(days: 1)).toIso8601String(),
      ),
      InventoryMovementEntity(
        id: DateTime.now().millisecondsSinceEpoch + 2,
        productId: 2,
        batchId: 2,
        type: MovementType.adjustment,
        reason: MovementReason.damaged,
        quantity: 2,
        unitPrice: 800, // 8.0 in cents
        totalValue: 1600, // 16.0 in cents
        referenceId: 1,
        referenceType: 'adjustment',
        notes: 'Damaged goods adjustment',
        fromLocationId: 1,
        toLocationId: null,
        createdById: 'user1',
        createdAt: DateTime.now().subtract(const Duration(hours: 2)),
        updatedAt: DateTime.now().subtract(const Duration(hours: 2)).toIso8601String(),
      ),
    ];

    for (final movement in sampleMovements) {
      await createInventoryMovement(movement);
    }
  }

  Map<String, dynamic> _movementToMap(InventoryMovementEntity movement) {
    return {
      'id': movement.id,
      'productId': movement.productId,
      'batchId': movement.batchId,
      'type': movement.type.name,
      'reason': movement.reason,
      'quantity': movement.quantity,
      'unitPrice': movement.unitPrice,
      'totalValue': movement.totalValue,
      'referenceId': movement.referenceId,
      'referenceType': movement.referenceType,
      'notes': movement.notes,
      'fromLocationId': movement.fromLocationId,
      'toLocationId': movement.toLocationId,
      'createdById': movement.createdById,
      'createdAt': movement.createdAt,
      'updatedAt': movement.updatedAt,
    };
  }

  InventoryMovementEntity _mapToMovement(Map<String, dynamic> map) {
    return InventoryMovementEntity(
      id: map['id'] as int,
      productId: map['productId'] as int,
      batchId: map['batchId'] as int?,
      type: MovementType.values.firstWhere(
        (e) => e.name == map['type'],
        orElse: () => MovementType.adjustment,
      ),
      reason: MovementReason.values.firstWhere(
        (e) => e.name == map['reason'],
        orElse: () => MovementReason.adjustment,
      ),
      quantity: map['quantity'] as int,
      unitPrice: map['unitPrice'] as int,
      totalValue: map['totalValue'] as int,
      referenceId: map['referenceId'] as int?,
      referenceType: map['referenceType'] as String?,
      notes: map['notes'] as String?,
      fromLocationId: map['fromLocationId'] as int?,
      toLocationId: map['toLocationId'] as int?,
      createdById: map['createdById'] as String,
      createdAt: DateTime.parse(map['createdAt'] as String),
      updatedAt: map['updatedAt'] as String?,
    );
  }
}
