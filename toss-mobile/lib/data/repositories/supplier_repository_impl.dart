import 'dart:convert';
import '../datasources/local/supplier_local_datasource_impl.dart';
import '../datasources/remote/supplier_remote_datasource_impl.dart';
import '../repositories/connectivity_repository.dart';
import '../datasources/local/queued_action_local_datasource_impl.dart';
import '../models/queued_action_model.dart';
import '../../domain/entities/supplier_entity.dart';
import '../../domain/repositories/supplier_repository.dart';

class SupplierRepositoryImpl implements SupplierRepository {
  final SupplierLocalDatasourceImpl _localDatasource;
  final SupplierRemoteDatasourceImpl _remoteDatasource;
  final ConnectivityRepository _connectivityRepository;
  final QueuedActionLocalDatasourceImpl _queuedActionLocalDatasource;

  SupplierRepositoryImpl(
    this._localDatasource,
    this._remoteDatasource,
    this._connectivityRepository,
    this._queuedActionLocalDatasource,
  );

  @override
  Future<List<SupplierEntity>> getAllSuppliers() async {
    try {
      return await _localDatasource.getAllSuppliers();
    } catch (e) {
      throw Exception('Failed to get all suppliers: $e');
    }
  }

  @override
  Future<SupplierEntity?> getSupplierById(int id) async {
    try {
      return await _localDatasource.getSupplierById(id);
    } catch (e) {
      throw Exception('Failed to get supplier by ID: $e');
    }
  }

  @override
  Future<List<SupplierEntity>> searchSuppliers(String query) async {
    try {
      return await _localDatasource.searchSuppliers(query);
    } catch (e) {
      throw Exception('Failed to search suppliers: $e');
    }
  }

  @override
  Future<List<SupplierEntity>> getActiveSuppliers() async {
    try {
      return await _localDatasource.getActiveSuppliers();
    } catch (e) {
      throw Exception('Failed to get active suppliers: $e');
    }
  }

  @override
  Future<int> createSupplier(SupplierEntity supplier) async {
    try {
      // Save to local database first
      final supplierId = await _localDatasource.createSupplier(supplier);

      if (await _connectivityRepository.isConnected) {
        try {
          // Try to sync to remote
          await _remoteDatasource.createSupplier(supplier);
        } catch (e) {
          // If remote fails, queue the action for later
          await _queuedActionLocalDatasource.createQueuedAction(
            QueuedActionModel(
              id: DateTime.now().millisecondsSinceEpoch,
              repository: 'SupplierRepository',
              method: 'createSupplier',
              param: jsonEncode(supplier.toMap()),
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
            repository: 'SupplierRepository',
            method: 'createSupplier',
            param: jsonEncode(supplier.toMap()),
            isCritical: true,
            status: 'pending',
            retryCount: 0,
            createdAt: DateTime.now().toIso8601String(),
          ),
        );
      }

      return supplierId;
    } catch (e) {
      throw Exception('Failed to create supplier: $e');
    }
  }

  @override
  Future<void> updateSupplier(SupplierEntity supplier) async {
    try {
      // Update local database first
      await _localDatasource.updateSupplier(supplier);

      if (await _connectivityRepository.isConnected) {
        try {
          // Try to sync to remote
          await _remoteDatasource.updateSupplier(supplier);
        } catch (e) {
          // If remote fails, queue the action for later
          await _queuedActionLocalDatasource.createQueuedAction(
            QueuedActionModel(
              id: DateTime.now().millisecondsSinceEpoch,
              repository: 'SupplierRepository',
              method: 'updateSupplier',
              param: jsonEncode(supplier.toMap()),
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
            repository: 'SupplierRepository',
            method: 'updateSupplier',
            param: jsonEncode(supplier.toMap()),
            isCritical: true,
            status: 'pending',
            retryCount: 0,
            createdAt: DateTime.now().toIso8601String(),
          ),
        );
      }
    } catch (e) {
      throw Exception('Failed to update supplier: $e');
    }
  }

  @override
  Future<void> deleteSupplier(int id) async {
    try {
      // Delete from local database first
      await _localDatasource.deleteSupplier(id);

      if (await _connectivityRepository.isConnected) {
        try {
          // Try to sync to remote
          await _remoteDatasource.deleteSupplier(id);
        } catch (e) {
          // If remote fails, queue the action for later
          await _queuedActionLocalDatasource.createQueuedAction(
            QueuedActionModel(
              id: DateTime.now().millisecondsSinceEpoch,
              repository: 'SupplierRepository',
              method: 'deleteSupplier',
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
            repository: 'SupplierRepository',
            method: 'deleteSupplier',
            param: jsonEncode({'id': id}),
            isCritical: true,
            status: 'pending',
            retryCount: 0,
            createdAt: DateTime.now().toIso8601String(),
          ),
        );
      }
    } catch (e) {
      throw Exception('Failed to delete supplier: $e');
    }
  }

  @override
  Future<void> seedSampleSuppliers() async {
    try {
      await _localDatasource.seedSampleSuppliers();
    } catch (e) {
      throw Exception('Failed to seed sample suppliers: $e');
    }
  }
}
