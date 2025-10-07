import 'dart:async';
import 'dart:convert';

import '../../domain/entities/inventory_movement_entity.dart';
import '../../domain/repositories/inventory_movement_repository.dart';
import '../datasources/local/inventory_movement_local_datasource_impl.dart';
import '../datasources/remote/inventory_movement_remote_datasource_impl.dart';
import '../repositories/connectivity_repository.dart';
import '../datasources/local/queued_action_local_datasource_impl.dart';
import '../models/queued_action_model.dart';

class InventoryMovementRepositoryImpl implements InventoryMovementRepository {
  final InventoryMovementLocalDatasourceImpl _localDatasource;
  final InventoryMovementRemoteDatasourceImpl _remoteDatasource;
  final ConnectivityRepository _connectivityRepository;
  final QueuedActionLocalDatasourceImpl _queuedActionLocalDatasource;

  InventoryMovementRepositoryImpl(
    this._localDatasource,
    this._remoteDatasource,
    this._connectivityRepository,
    this._queuedActionLocalDatasource,
  );

  @override
  Future<List<InventoryMovementEntity>> getAllInventoryMovements() async {
    try {
      return await _localDatasource.getAllInventoryMovements();
    } catch (e) {
      throw Exception('Failed to get inventory movements: $e');
    }
  }

  @override
  Future<List<InventoryMovementEntity>> getInventoryMovementsByProduct(String productId) async {
    try {
      return await _localDatasource.getInventoryMovementsByProduct(productId);
    } catch (e) {
      throw Exception('Failed to get inventory movements by product: $e');
    }
  }

  @override
  Future<List<InventoryMovementEntity>> getInventoryMovementsByType(MovementType type) async {
    try {
      return await _localDatasource.getInventoryMovementsByType(type);
    } catch (e) {
      throw Exception('Failed to get inventory movements by type: $e');
    }
  }

  @override
  Future<List<InventoryMovementEntity>> getInventoryMovementsByDateRange(
    DateTime startDate,
    DateTime endDate,
  ) async {
    try {
      return await _localDatasource.getInventoryMovementsByDateRange(startDate, endDate);
    } catch (e) {
      throw Exception('Failed to get inventory movements by date range: $e');
    }
  }

  @override
  Future<List<InventoryMovementEntity>> getInventoryMovementsByLocation(String locationId) async {
    try {
      return await _localDatasource.getInventoryMovementsByLocation(locationId);
    } catch (e) {
      throw Exception('Failed to get inventory movements by location: $e');
    }
  }

  @override
  Future<InventoryMovementEntity?> getInventoryMovementById(int id) async {
    try {
      return await _localDatasource.getInventoryMovementById(id);
    } catch (e) {
      throw Exception('Failed to get inventory movement by ID: $e');
    }
  }

  @override
  Future<int> createInventoryMovement(InventoryMovementEntity movement) async {
    try {
      // Always create locally first
      final movementId = await _localDatasource.createInventoryMovement(movement);
      
      if (await _connectivityRepository.isConnected) {
        try {
          // Try to sync to remote
          await _remoteDatasource.createInventoryMovement(movement);
        } catch (e) {
          // If remote fails, queue the action for later
          await _queuedActionLocalDatasource.createQueuedAction(
            QueuedActionModel(
              id: DateTime.now().millisecondsSinceEpoch,
              repository: 'InventoryMovementRepository',
              method: 'createInventoryMovement',
              param: jsonEncode(movement.toMap()),
              isCritical: true,
              status: 'pending',
              retryCount: 0,
              createdAt: DateTime.now().toIso8601String(),
            ),
          );
        }
      } else {
        // Queue for later when online
        await _queuedActionLocalDatasource.createQueuedAction(
          QueuedActionModel(
            id: DateTime.now().millisecondsSinceEpoch,
            repository: 'InventoryMovementRepository',
            method: 'createInventoryMovement',
            param: jsonEncode(movement.toMap()),
            isCritical: true,
            status: 'pending',
            retryCount: 0,
            createdAt: DateTime.now().toIso8601String(),
          ),
        );
      }
      
      return movementId;
    } catch (e) {
      throw Exception('Failed to create inventory movement: $e');
    }
  }

  @override
  Future<void> updateInventoryMovement(InventoryMovementEntity movement) async {
    try {
      // Always update local first
      await _localDatasource.updateInventoryMovement(movement);
      
      if (await _connectivityRepository.isConnected) {
        try {
          // Try to sync to remote
          await _remoteDatasource.updateInventoryMovement(movement);
        } catch (e) {
          // If remote fails, queue the action for later
          await _queuedActionLocalDatasource.createQueuedAction(
            QueuedActionModel(
              id: DateTime.now().millisecondsSinceEpoch,
              repository: 'InventoryMovementRepository',
              method: 'updateInventoryMovement',
              param: jsonEncode(movement.toMap()),
              isCritical: true,
              status: 'pending',
              retryCount: 0,
              createdAt: DateTime.now().toIso8601String(),
            ),
          );
        }
      } else {
        // Queue for later when online
        await _queuedActionLocalDatasource.createQueuedAction(
          QueuedActionModel(
            id: DateTime.now().millisecondsSinceEpoch,
            repository: 'InventoryMovementRepository',
            method: 'updateInventoryMovement',
            param: jsonEncode(movement.toMap()),
            isCritical: true,
            status: 'pending',
            retryCount: 0,
            createdAt: DateTime.now().toIso8601String(),
          ),
        );
      }
    } catch (e) {
      throw Exception('Failed to update inventory movement: $e');
    }
  }

  @override
  Future<void> deleteInventoryMovement(int id) async {
    try {
      // Always delete local first
      await _localDatasource.deleteInventoryMovement(id);
      
      if (await _connectivityRepository.isConnected) {
        try {
          // Try to sync to remote
          await _remoteDatasource.deleteInventoryMovement(id);
        } catch (e) {
          // If remote fails, queue the action for later
          await _queuedActionLocalDatasource.createQueuedAction(
            QueuedActionModel(
              id: DateTime.now().millisecondsSinceEpoch,
              repository: 'InventoryMovementRepository',
              method: 'deleteInventoryMovement',
              param: id.toString(),
              isCritical: true,
              status: 'pending',
              retryCount: 0,
              createdAt: DateTime.now().toIso8601String(),
            ),
          );
        }
      } else {
        // Queue for later when online
        await _queuedActionLocalDatasource.createQueuedAction(
          QueuedActionModel(
            id: DateTime.now().millisecondsSinceEpoch,
            repository: 'InventoryMovementRepository',
            method: 'deleteInventoryMovement',
            param: id.toString(),
            isCritical: true,
            status: 'pending',
            retryCount: 0,
            createdAt: DateTime.now().toIso8601String(),
          ),
        );
      }
    } catch (e) {
      throw Exception('Failed to delete inventory movement: $e');
    }
  }

  @override
  Future<List<InventoryMovementEntity>> getLowStockMovements() async {
    try {
      return await _localDatasource.getLowStockMovements();
    } catch (e) {
      throw Exception('Failed to get low stock movements: $e');
    }
  }

  @override
  Future<List<InventoryMovementEntity>> getExpiredProductMovements() async {
    try {
      return await _localDatasource.getExpiredProductMovements();
    } catch (e) {
      throw Exception('Failed to get expired product movements: $e');
    }
  }

  @override
  Future<Map<String, double>> getInventoryValuation() async {
    try {
      return await _localDatasource.getInventoryValuation();
    } catch (e) {
      throw Exception('Failed to get inventory valuation: $e');
    }
  }

  @override
  Future<List<Map<String, dynamic>>> getInventoryTurnoverReport() async {
    try {
      return await _localDatasource.getInventoryTurnoverReport();
    } catch (e) {
      throw Exception('Failed to get inventory turnover report: $e');
    }
  }

  @override
  Future<int> createStockAdjustment(
    String productId,
    String reason,
    double quantity,
    String? notes,
    String? locationId,
  ) async {
    try {
      final movement = InventoryMovementEntity(
        id: DateTime.now().millisecondsSinceEpoch,
        productId: int.parse(productId),
        type: MovementType.adjustment,
        reason: MovementReason.values.firstWhere(
          (e) => e.name == reason,
          orElse: () => MovementReason.adjustment,
        ),
        quantity: quantity.toInt(),
        unitPrice: 0, // Will be updated based on product cost
        totalValue: 0, // Will be calculated
        notes: notes,
        fromLocationId: locationId != null ? int.parse(locationId) : null,
        toLocationId: null,
        createdById: 'current_user', // TODO: Get from auth
        createdAt: DateTime.now(),
        updatedAt: DateTime.now().toIso8601String(),
      );

      return await createInventoryMovement(movement);
    } catch (e) {
      throw Exception('Failed to create stock adjustment: $e');
    }
  }

  @override
  Future<int> createInventoryTransfer(
    String productId,
    double quantity,
    String fromLocationId,
    String toLocationId,
    String? notes,
  ) async {
    try {
      final movement = InventoryMovementEntity(
        id: DateTime.now().millisecondsSinceEpoch,
        productId: int.parse(productId),
        type: MovementType.transfer,
        reason: MovementReason.transfer,
        quantity: quantity.toInt(),
        unitPrice: 0, // Will be updated based on product cost
        totalValue: 0, // Will be calculated
        notes: notes,
        fromLocationId: int.parse(fromLocationId),
        toLocationId: int.parse(toLocationId),
        createdById: 'current_user', // TODO: Get from auth
        createdAt: DateTime.now(),
        updatedAt: DateTime.now().toIso8601String(),
      );

      return await createInventoryMovement(movement);
    } catch (e) {
      throw Exception('Failed to create inventory transfer: $e');
    }
  }

  @override
  Future<void> seedSampleInventoryMovements() async {
    try {
      await _localDatasource.seedSampleInventoryMovements();
    } catch (e) {
      throw Exception('Failed to seed sample inventory movements: $e');
    }
  }
}
