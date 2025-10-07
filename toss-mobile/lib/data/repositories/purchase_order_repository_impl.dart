import 'dart:convert';
import '../datasources/local/purchase_order_local_datasource_impl.dart';
import '../datasources/remote/purchase_order_remote_datasource_impl.dart';
import '../repositories/connectivity_repository.dart';
import '../datasources/local/queued_action_local_datasource_impl.dart';
import '../models/queued_action_model.dart';
import '../../domain/entities/purchase_order_entity.dart';
import '../../domain/repositories/purchase_order_repository.dart';

class PurchaseOrderRepositoryImpl implements PurchaseOrderRepository {
  final PurchaseOrderLocalDatasourceImpl _localDatasource;
  final PurchaseOrderRemoteDatasourceImpl _remoteDatasource;
  final ConnectivityRepository _connectivityRepository;
  final QueuedActionLocalDatasourceImpl _queuedActionLocalDatasource;

  PurchaseOrderRepositoryImpl(
    this._localDatasource,
    this._remoteDatasource,
    this._connectivityRepository,
    this._queuedActionLocalDatasource,
  );

  @override
  Future<List<PurchaseOrderEntity>> getAllPurchaseOrders() async {
    try {
      return await _localDatasource.getAllPurchaseOrders();
    } catch (e) {
      throw Exception('Failed to get all purchase orders: $e');
    }
  }

  @override
  Future<PurchaseOrderEntity?> getPurchaseOrderById(int id) async {
    try {
      return await _localDatasource.getPurchaseOrderById(id);
    } catch (e) {
      throw Exception('Failed to get purchase order by ID: $e');
    }
  }

  @override
  Future<List<PurchaseOrderEntity>> getPurchaseOrdersBySupplier(int supplierId) async {
    try {
      return await _localDatasource.getPurchaseOrdersBySupplier(supplierId);
    } catch (e) {
      throw Exception('Failed to get purchase orders by supplier: $e');
    }
  }

  @override
  Future<List<PurchaseOrderEntity>> getPurchaseOrdersByStatus(PurchaseOrderStatus status) async {
    try {
      return await _localDatasource.getPurchaseOrdersByStatus(status);
    } catch (e) {
      throw Exception('Failed to get purchase orders by status: $e');
    }
  }

  @override
  Future<List<PurchaseOrderEntity>> getOverduePurchaseOrders() async {
    try {
      return await _localDatasource.getOverduePurchaseOrders();
    } catch (e) {
      throw Exception('Failed to get overdue purchase orders: $e');
    }
  }

  @override
  Future<int> createPurchaseOrder(PurchaseOrderEntity purchaseOrder) async {
    try {
      // Save to local database first
      final orderId = await _localDatasource.createPurchaseOrder(purchaseOrder);

      if (await _connectivityRepository.isConnected) {
        try {
          // Try to sync to remote
          await _remoteDatasource.createPurchaseOrder(purchaseOrder);
        } catch (e) {
          // If remote fails, queue the action for later
          await _queuedActionLocalDatasource.createQueuedAction(
            QueuedActionModel(
              id: DateTime.now().millisecondsSinceEpoch,
              repository: 'PurchaseOrderRepository',
              method: 'createPurchaseOrder',
              param: jsonEncode(purchaseOrder.toMap()),
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
            repository: 'PurchaseOrderRepository',
            method: 'createPurchaseOrder',
            param: jsonEncode(purchaseOrder.toMap()),
            isCritical: true,
            status: 'pending',
            retryCount: 0,
            createdAt: DateTime.now().toIso8601String(),
          ),
        );
      }

      return orderId;
    } catch (e) {
      throw Exception('Failed to create purchase order: $e');
    }
  }

  @override
  Future<void> updatePurchaseOrder(PurchaseOrderEntity purchaseOrder) async {
    try {
      // Update local database first
      await _localDatasource.updatePurchaseOrder(purchaseOrder);

      if (await _connectivityRepository.isConnected) {
        try {
          // Try to sync to remote
          await _remoteDatasource.updatePurchaseOrder(purchaseOrder);
        } catch (e) {
          // If remote fails, queue the action for later
          await _queuedActionLocalDatasource.createQueuedAction(
            QueuedActionModel(
              id: DateTime.now().millisecondsSinceEpoch,
              repository: 'PurchaseOrderRepository',
              method: 'updatePurchaseOrder',
              param: jsonEncode(purchaseOrder.toMap()),
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
            repository: 'PurchaseOrderRepository',
            method: 'updatePurchaseOrder',
            param: jsonEncode(purchaseOrder.toMap()),
            isCritical: true,
            status: 'pending',
            retryCount: 0,
            createdAt: DateTime.now().toIso8601String(),
          ),
        );
      }
    } catch (e) {
      throw Exception('Failed to update purchase order: $e');
    }
  }

  @override
  Future<void> deletePurchaseOrder(int id) async {
    try {
      // Delete from local database first
      await _localDatasource.deletePurchaseOrder(id);

      if (await _connectivityRepository.isConnected) {
        try {
          // Try to sync to remote
          await _remoteDatasource.deletePurchaseOrder(id);
        } catch (e) {
          // If remote fails, queue the action for later
          await _queuedActionLocalDatasource.createQueuedAction(
            QueuedActionModel(
              id: DateTime.now().millisecondsSinceEpoch,
              repository: 'PurchaseOrderRepository',
              method: 'deletePurchaseOrder',
              param: jsonEncode({'id': id}),
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
            repository: 'PurchaseOrderRepository',
            method: 'deletePurchaseOrder',
            param: jsonEncode({'id': id}),
            isCritical: true,
            status: 'pending',
            retryCount: 0,
            createdAt: DateTime.now().toIso8601String(),
          ),
        );
      }
    } catch (e) {
      throw Exception('Failed to delete purchase order: $e');
    }
  }

  @override
  Future<void> updatePurchaseOrderStatus(int id, PurchaseOrderStatus status) async {
    try {
      // Update local database first
      await _localDatasource.updatePurchaseOrderStatus(id, status);

      if (await _connectivityRepository.isConnected) {
        try {
          // Try to sync to remote
          await _remoteDatasource.updatePurchaseOrderStatus(id, status);
        } catch (e) {
          // If remote fails, queue the action for later
          await _queuedActionLocalDatasource.createQueuedAction(
            QueuedActionModel(
              id: DateTime.now().millisecondsSinceEpoch,
              repository: 'PurchaseOrderRepository',
              method: 'updatePurchaseOrderStatus',
              param: jsonEncode({'id': id, 'status': status.name}),
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
            repository: 'PurchaseOrderRepository',
            method: 'updatePurchaseOrderStatus',
            param: jsonEncode({'id': id, 'status': status.name}),
            isCritical: true,
            status: 'pending',
            retryCount: 0,
            createdAt: DateTime.now().toIso8601String(),
          ),
        );
      }
    } catch (e) {
      throw Exception('Failed to update purchase order status: $e');
    }
  }

  @override
  Future<void> seedSamplePurchaseOrders() async {
    try {
      await _localDatasource.seedSamplePurchaseOrders();
    } catch (e) {
      throw Exception('Failed to seed sample purchase orders: $e');
    }
  }
}
