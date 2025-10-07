import 'package:cloud_firestore/cloud_firestore.dart';

import '../../../domain/entities/inventory_movement_entity.dart';
import '../../datasources/interfaces/inventory_movement_datasource.dart';

class InventoryMovementRemoteDatasourceImpl implements InventoryMovementDatasource {
  final FirebaseFirestore _firestore = FirebaseFirestore.instance;
  static const String _collection = 'inventoryMovements';

  @override
  Future<int> createInventoryMovement(InventoryMovementEntity movement) async {
    final docRef = await _firestore.collection(_collection).add(_movementToMap(movement));
    return int.parse(docRef.id);
  }

  @override
  Future<List<InventoryMovementEntity>> getAllInventoryMovements() async {
    final snapshot = await _firestore
        .collection(_collection)
        .orderBy('createdAt', descending: true)
        .get();
    
    return snapshot.docs.map((doc) => _mapToMovement(doc.data(), doc.id)).toList();
  }

  @override
  Future<List<InventoryMovementEntity>> getInventoryMovementsByProduct(String productId) async {
    final snapshot = await _firestore
        .collection(_collection)
        .where('productId', isEqualTo: productId)
        .orderBy('createdAt', descending: true)
        .get();
    
    return snapshot.docs.map((doc) => _mapToMovement(doc.data(), doc.id)).toList();
  }

  @override
  Future<List<InventoryMovementEntity>> getInventoryMovementsByType(MovementType type) async {
    final snapshot = await _firestore
        .collection(_collection)
        .where('type', isEqualTo: type.name)
        .orderBy('createdAt', descending: true)
        .get();
    
    return snapshot.docs.map((doc) => _mapToMovement(doc.data(), doc.id)).toList();
  }

  @override
  Future<List<InventoryMovementEntity>> getInventoryMovementsByDateRange(
    DateTime startDate,
    DateTime endDate,
  ) async {
    final snapshot = await _firestore
        .collection(_collection)
        .where('createdAt', isGreaterThanOrEqualTo: startDate.toIso8601String())
        .where('createdAt', isLessThanOrEqualTo: endDate.toIso8601String())
        .orderBy('createdAt', descending: true)
        .get();
    
    return snapshot.docs.map((doc) => _mapToMovement(doc.data(), doc.id)).toList();
  }

  @override
  Future<List<InventoryMovementEntity>> getInventoryMovementsByLocation(String locationId) async {
    final snapshot = await _firestore
        .collection(_collection)
        .where('fromLocationId', isEqualTo: locationId)
        .get();
    
    final snapshot2 = await _firestore
        .collection(_collection)
        .where('toLocationId', isEqualTo: locationId)
        .get();
    
    final allDocs = [...snapshot.docs, ...snapshot2.docs];
    allDocs.sort((a, b) => b.data()['createdAt'].compareTo(a.data()['createdAt']));
    
    return allDocs.map((doc) => _mapToMovement(doc.data(), doc.id)).toList();
  }

  @override
  Future<InventoryMovementEntity?> getInventoryMovementById(int id) async {
    final doc = await _firestore.collection(_collection).doc(id.toString()).get();
    
    if (!doc.exists) return null;
    return _mapToMovement(doc.data()!, doc.id);
  }

  @override
  Future<void> updateInventoryMovement(InventoryMovementEntity movement) async {
    await _firestore
        .collection(_collection)
        .doc(movement.id.toString())
        .update(_movementToMap(movement));
  }

  @override
  Future<void> deleteInventoryMovement(int id) async {
    await _firestore.collection(_collection).doc(id.toString()).delete();
  }

  @override
  Future<List<InventoryMovementEntity>> getLowStockMovements() async {
    // This would require a more complex query in Firestore
    // For now, return empty list as this is better handled locally
    return [];
  }

  @override
  Future<List<InventoryMovementEntity>> getExpiredProductMovements() async {
    final snapshot = await _firestore
        .collection(_collection)
        .where('type', isEqualTo: 'adjustment')
        .where('reason', isEqualTo: 'expired')
        .orderBy('createdAt', descending: true)
        .get();
    
    return snapshot.docs.map((doc) => _mapToMovement(doc.data(), doc.id)).toList();
  }

  @override
  Future<Map<String, double>> getInventoryValuation() async {
    // This would require aggregation queries in Firestore
    // For now, return empty map as this is better handled locally
    return {};
  }

  @override
  Future<List<Map<String, dynamic>>> getInventoryTurnoverReport() async {
    // This would require complex aggregation queries in Firestore
    // For now, return empty list as this is better handled locally
    return [];
  }

  @override
  Future<void> seedSampleInventoryMovements() async {
    // Remote seeding is not typically done
    // This would be handled by the local datasource
  }

  Map<String, dynamic> _movementToMap(InventoryMovementEntity movement) {
    return {
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

  InventoryMovementEntity _mapToMovement(Map<String, dynamic> map, String id) {
    return InventoryMovementEntity(
      id: int.parse(id),
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
