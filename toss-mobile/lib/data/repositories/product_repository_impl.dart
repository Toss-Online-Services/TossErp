import 'dart:convert';

import 'package:flutter/foundation.dart';

import '../../app/const/const.dart';
import '../../app/services/connectivity/connectivity_service.dart';
import '../../core/errors/errors.dart';
import '../../core/usecase/usecase.dart';
import '../../domain/entities/product_entity.dart';
import '../../domain/repositories/product_repository.dart';
import '../datasources/local/product_local_datasource_impl.dart';
import '../datasources/local/queued_action_local_datasource_impl.dart';
import '../datasources/remote/product_remote_datasource_impl.dart';
import '../models/product_model.dart';
import '../models/queued_action_model.dart';

class ProductRepositoryImpl extends ProductRepository {
  final ProductLocalDatasourceImpl productLocalDatasource;
  final ProductRemoteDatasourceImpl productRemoteDatasource;
  final QueuedActionLocalDatasourceImpl queuedActionLocalDatasource;

  ProductRepositoryImpl({
    required this.productLocalDatasource,
    required this.productRemoteDatasource,
    required this.queuedActionLocalDatasource,
  });

  @override
  Future<Result<int>> syncAllUserProducts(String userId) async {
    try {
      if (ConnectivityService.isConnected) {
        var local = await productLocalDatasource.getAllUserProducts(userId);
        var remote = await productRemoteDatasource.getAllUserProducts(userId);

        var res = await syncProducts(local, remote);

        // Sum all local and remote sync counts
        int totalSyncedCount = res.$1 + res.$2;

        // Return synced data count
        return Result.success(totalSyncedCount);
      }

      return Result.success(0);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<List<ProductEntity>>> getUserProducts(
    String userId, {
    String orderBy = 'createdAt',
    String sortBy = 'DESC',
    int limit = 10,
    int? offset,
    String? contains,
  }) async {
    try {
      if (kDebugMode) {
        debugPrint('🏪 ProductRepository: Getting products for user: $userId');
        debugPrint('🏪 ProductRepository: Contains: $contains, Limit: $limit, Offset: $offset');
      }
      
      var local = await productLocalDatasource.getUserProducts(
        userId,
        orderBy: orderBy,
        sortBy: sortBy,
        limit: limit,
        offset: offset,
        contains: contains,
      );

      if (kDebugMode) {
        debugPrint('🏪 ProductRepository: Found ${local.length} local products');
        if (local.isNotEmpty) {
          debugPrint('🏪 First local product: ${local.first.name}');
        }
      }

      if (ConnectivityService.isConnected) {
        var remote = await productRemoteDatasource.getUserProducts(
          userId,
          orderBy: orderBy,
          sortBy: sortBy,
          limit: limit,
          offset: offset,
          contains: contains,
        );

        if (kDebugMode) {
          debugPrint('🏪 ProductRepository: Found ${remote.length} remote products');
        }

        var res = await syncProducts(local, remote);

        int syncedToLocalCount = res.$1;
        int syncedToRemoteCount = res.$2;

        // Prefer remote when we synced more down; otherwise local.
        List<ProductModel> base = syncedToLocalCount > syncedToRemoteCount ? remote : local;
        var list = base.map((e) => e.toEntity()).toList();
        // Fallback local filter for substring (case-insensitive) when 'contains' is set but remote
        // returned prefix-based matches only. This narrows results client-side.
        if ((contains != null) && contains.trim().isNotEmpty) {
          final term = contains.trim().toLowerCase();
          list = list
              .where((p) =>
                  (p.name.toLowerCase().contains(term)) ||
                  (p.description?.toLowerCase().contains(term) ?? false) ||
                  (p.id?.toString().contains(term) ?? false) ||
                  (p.price.toString().contains(term)) ||
                  (p.stock.toString().contains(term)))
              .toList();
        }
        
        if (kDebugMode) {
          debugPrint('✅ ProductRepository: Returning ${list.length} products after sync and filtering');
        }
        
        return Result.success(list);
      }

      var list = local.map((e) => e.toEntity()).toList();
      
      if (kDebugMode) {
        debugPrint('✅ ProductRepository: Returning ${list.length} local products (offline mode)');
      }
      
      return Result.success(list);
    } catch (e) {
      if (kDebugMode) {
        debugPrint('❌ ProductRepository: Error getting products: $e');
      }
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<ProductEntity>> getProduct(int productId) async {
    try {
      var local = await productLocalDatasource.getProduct(productId);

      if (ConnectivityService.isConnected) {
        var remote = await productRemoteDatasource.getProduct(productId);

        List<ProductModel> localToList = local != null ? [local] : [];
        List<ProductModel> remoteToList = remote != null ? [remote] : [];

        var res = await syncProducts(localToList, remoteToList);

        int syncedToLocalCount = res.$1;
        int syncedToRemoteCount = res.$2;

        // If more data was synced to the local, return the remote data
        if (syncedToLocalCount > syncedToRemoteCount) {
          // Return remote data
          return Result.success(remote?.toEntity());
        } else {
          // Return local data
          return Result.success(local?.toEntity());
        }
      }

      return Result.success(local?.toEntity());
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<int>> createProduct(ProductEntity product) async {
    try {
      var data = ProductModel.fromEntity(product);

      var productId = await productLocalDatasource.createProduct(data);

      if (ConnectivityService.isConnected) {
        await productRemoteDatasource.createProduct(data);
      } else {
        await queuedActionLocalDatasource.createQueuedAction(
          QueuedActionModel(
            id: DateTime.now().millisecond,
            repository: 'ProductRepositoryImpl',
            method: 'createProduct',
            param: jsonEncode((data).toJson()),
            isCritical: true,
            createdAt: DateTime.now().toIso8601String(),
          ),
        );
      }

      return Result.success(productId);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> deleteProduct(int productId) async {
    try {
      await productLocalDatasource.deleteProduct(productId);

      if (ConnectivityService.isConnected) {
        await productRemoteDatasource.deleteProduct(productId);
      } else {
        await queuedActionLocalDatasource.createQueuedAction(
          QueuedActionModel(
            id: DateTime.now().millisecond,
            repository: 'ProductRepositoryImpl',
            method: 'deleteProduct',
            param: productId.toString(),
            isCritical: true,
            createdAt: DateTime.now().toIso8601String(),
          ),
        );
      }

      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  @override
  Future<Result<void>> updateProduct(ProductEntity product) async {
    try {
      await productLocalDatasource.updateProduct(ProductModel.fromEntity(product));

      if (ConnectivityService.isConnected) {
        await productRemoteDatasource.updateProduct(ProductModel.fromEntity(product));
      } else {
        await queuedActionLocalDatasource.createQueuedAction(
          QueuedActionModel(
            id: DateTime.now().millisecond,
            repository: 'ProductRepositoryImpl',
            method: 'updateProduct',
            param: jsonEncode(ProductModel.fromEntity(product).toJson()),
            isCritical: true,
            createdAt: DateTime.now().toIso8601String(),
          ),
        );
      }

      return Result.success(null);
    } catch (e) {
      return Result.error(APIError(message: e.toString()));
    }
  }

  // Perform a sync between local and remote data
  Future<(int, int)> syncProducts(List<ProductModel> local, List<ProductModel> remote) async {
    int syncedToLocalCount = 0;
    int syncedToRemoteCount = 0;

    // Local
    // Local first
    for (var localData in local) {
      var matchRemoteData = remote.where((remoteData) => remoteData.id == localData.id).firstOrNull;

      if (matchRemoteData != null) {
        var updatedAtLocal = DateTime.tryParse(localData.updatedAt ?? DateTime.now().toIso8601String());
        var updatedAtRemote = DateTime.tryParse(matchRemoteData.updatedAt ?? DateTime.now().toIso8601String());
        var differenceInMinutes = updatedAtRemote?.difference(updatedAtLocal!).inMinutes ?? 0;
        // Check is the difference is above the minimum interval sync tolerance
        var isRemoteNewer = differenceInMinutes.abs() > MIN_SYNC_INTERVAL_TOLERANCE_FOR_CRITICAL_IN_MINUTES;
        var isLocalNewer = differenceInMinutes.abs() > MIN_SYNC_INTERVAL_TOLERANCE_FOR_CRITICAL_IN_MINUTES;

        if (isRemoteNewer) {
          syncedToLocalCount += 1;
          // Save remote data to local db
          await productLocalDatasource.updateProduct(matchRemoteData);
        }

        if (isLocalNewer) {
          syncedToRemoteCount += 1;
          // Update remote with local data
          await productRemoteDatasource.updateProduct(localData);
        }
      } else {
        syncedToRemoteCount += 1;
        // No matching remote product, create it
        await productRemoteDatasource.createProduct(localData);
      }
    }

    // Remote first
    for (var remoteData in remote) {
      var matchLocalData = local.where((localData) => localData.id == remoteData.id).firstOrNull;

      if (matchLocalData != null) {
        var updatedAtLocal = DateTime.tryParse(remoteData.updatedAt ?? DateTime.now().toIso8601String());
        var updatedAtRemote = DateTime.tryParse(matchLocalData.updatedAt ?? DateTime.now().toIso8601String());
        var differenceInMinutes = updatedAtRemote?.difference(updatedAtLocal!).inMinutes ?? 0;
        // Check is the difference is above the minimum interval sync tolerance
        var isRemoteNewer = differenceInMinutes.abs() > MIN_SYNC_INTERVAL_TOLERANCE_FOR_CRITICAL_IN_MINUTES;
        var isLocalNewer = differenceInMinutes.abs() > MIN_SYNC_INTERVAL_TOLERANCE_FOR_CRITICAL_IN_MINUTES;

        if (isRemoteNewer) {
          syncedToLocalCount += 1;
          // Save remote data to local db
          await productLocalDatasource.updateProduct(remoteData);
        }

        if (isLocalNewer) {
          syncedToRemoteCount += 1;
          // Update remote with local data
          await productRemoteDatasource.updateProduct(matchLocalData);
        }
      } else {
        syncedToLocalCount += 1;
        // No matching remote product, create it
        await productLocalDatasource.createProduct(remoteData);
      }
    }

    return (syncedToLocalCount, syncedToRemoteCount);
  }

  @override
  Future<Result<ProductEntity?>> getProductByBarcode(String barcode) async {
    try {
      // First try to find the product in local datasource
      final localProducts = await productLocalDatasource.getAllUserProducts('');
      final localResult = localProducts.firstWhere(
        (product) => product.barcode == barcode,
        orElse: () => ProductModel.empty(),
      );

      if (localResult.id != 0) {
        return Result.success(localResult.toEntity());
      }

      // If not found locally and we have connectivity, try remote
      if (ConnectivityService.isConnected) {
        try {
          final remoteProducts = await productRemoteDatasource.getAllUserProducts('');
          final remoteResult = remoteProducts.firstWhere(
            (product) => product.barcode == barcode,
            orElse: () => ProductModel.empty(),
          );

          if (remoteResult.id != 0) {
            // Save to local for future access
            await productLocalDatasource.createProduct(remoteResult);
            return Result.success(remoteResult.toEntity());
          }
        } catch (e) {
          // If remote fails, continue with null result
        }
      }

      // Product not found
      return Result.success(null);
    } catch (e) {
      return Result.error(ServiceError(message: 'Failed to search product by barcode: $e'));
    }
  }
}
