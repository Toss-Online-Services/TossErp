import 'package:flutter/foundation.dart';

import '../../../app/services/auth/auth_service.dart';
import '../../../domain/entities/product_entity.dart';
import '../../../domain/repositories/product_repository.dart';
import '../../../domain/usecases/params/base_params.dart';
import '../../../domain/usecases/product_usecases.dart';

class ProductsProvider extends ChangeNotifier {
  final ProductRepository productRepository;

  ProductsProvider({required this.productRepository});

  List<ProductEntity>? allProducts;

  // Low stock detection state
  final int lowStockThreshold = 5; // default threshold; could be made configurable
  List<ProductEntity> lowStockProducts = [];

  bool isLoadingMore = false;

  Future<void> getAllProducts({int? offset, String? contains}) async {
    if (offset != null) {
      isLoadingMore = true;
      notifyListeners();
    }

    // Check if user is authenticated
    final authData = AuthService().getAuthData();
    if (authData == null && kDebugMode) {
      debugPrint('⚠️ No authenticated user found, using sample data user ID');
    }

    var params = BaseParams(
      param: authData?.uid ?? 'e51VrUAK7WdXpa75V641428qX0u2', // Use same user ID as sample data
      offset: offset,
      contains: contains,
    );

    if (kDebugMode) {
      debugPrint('🛒 ProductsProvider: Loading products for user: ${params.param}');
      debugPrint('🛒 ProductsProvider: Contains filter: $contains');
      debugPrint('🛒 ProductsProvider: Offset: $offset');
    }

    var res = await GetUserProductsUsecase(productRepository).call(params);

    if (res.isSuccess) {
      if (kDebugMode) {
        debugPrint('✅ ProductsProvider: Successfully loaded ${res.data?.length ?? 0} products');
        if (res.data?.isNotEmpty == true) {
          debugPrint('🛒 First product: ${res.data!.first.name}');
        }
      }
      
      if (offset == null) {
        allProducts = res.data ?? [];
      } else {
        allProducts?.addAll(res.data ?? []);
      }

      // Update low stock list on refresh loads
      if (offset == null) {
        _computeLowStock();
      }

      isLoadingMore = false;
      notifyListeners();
    } else {
      if (kDebugMode) {
        debugPrint('❌ GetUserProductsUsecase failed: ${res.error?.message}');
      }
      throw res.error?.message ?? 'Failed to load data';
    }
  }

  void _computeLowStock() {
    final list = allProducts ?? [];
    lowStockProducts = list.where((p) => p.stock <= lowStockThreshold).toList();
  }

  // Bulk operations by product IDs (compatible with existing UI)
  Future<void> bulkUpdatePricesByIds(Set<int> productIds, double newPrice) async {
    try {
      final productsToUpdate = allProducts?.where((p) => productIds.contains(p.id)).toList() ?? [];
      
      for (final product in productsToUpdate) {
        final updatedProduct = ProductEntity(
          id: product.id,
          createdById: product.createdById,
          name: product.name,
          sku: product.sku,
          barcode: product.barcode,
          imageUrl: product.imageUrl,
          type: product.type,
          trackingMethod: product.trackingMethod,
          stock: product.stock,
          sold: product.sold,
          price: newPrice.toInt(), // Convert to int since ProductEntity expects int
          costPrice: product.costPrice,
          description: product.description,
          unit: product.unit,
          isActive: product.isActive,
          isFavorite: product.isFavorite,
          categoryId: product.categoryId,
          category: product.category,
          variants: product.variants,
          batches: product.batches,
          lowStockThreshold: product.lowStockThreshold,
          reorderPoint: product.reorderPoint,
          reorderQuantity: product.reorderQuantity,
          enableLowStockAlert: product.enableLowStockAlert,
          enableExpiryAlert: product.enableExpiryAlert,
          customAttributes: product.customAttributes,
          createdAt: product.createdAt,
          updatedAt: DateTime.now().toIso8601String(),
        );
        
        await productRepository.updateProduct(updatedProduct);
        
        // Update local state
        final index = allProducts?.indexWhere((p) => p.id == product.id);
        if (index != null && index >= 0 && allProducts != null) {
          allProducts![index] = updatedProduct;
        }
      }
      
      notifyListeners();
    } catch (e) {
      debugPrint('Error updating prices by IDs: $e');
      rethrow;
    }
  }

  Future<void> bulkUpdateStockByIds(Set<int> productIds, int newStock) async {
    try {
      final productsToUpdate = allProducts?.where((p) => productIds.contains(p.id)).toList() ?? [];
      
      for (final product in productsToUpdate) {
        final updatedProduct = ProductEntity(
          id: product.id,
          createdById: product.createdById,
          name: product.name,
          sku: product.sku,
          barcode: product.barcode,
          imageUrl: product.imageUrl,
          type: product.type,
          trackingMethod: product.trackingMethod,
          stock: newStock,
          sold: product.sold,
          price: product.price,
          costPrice: product.costPrice,
          description: product.description,
          unit: product.unit,
          isActive: product.isActive,
          isFavorite: product.isFavorite,
          categoryId: product.categoryId,
          category: product.category,
          variants: product.variants,
          batches: product.batches,
          lowStockThreshold: product.lowStockThreshold,
          reorderPoint: product.reorderPoint,
          reorderQuantity: product.reorderQuantity,
          enableLowStockAlert: product.enableLowStockAlert,
          enableExpiryAlert: product.enableExpiryAlert,
          customAttributes: product.customAttributes,
          createdAt: product.createdAt,
          updatedAt: DateTime.now().toIso8601String(),
        );
        
        await productRepository.updateProduct(updatedProduct);
        
        // Update local state
        final index = allProducts?.indexWhere((p) => p.id == product.id);
        if (index != null && index >= 0 && allProducts != null) {
          allProducts![index] = updatedProduct;
        }
      }
      
      // Recompute low stock list
      _computeLowStock();
      notifyListeners();
    } catch (e) {
      debugPrint('Error updating stock by IDs: $e');
      rethrow;
    }
  }

  Future<void> bulkDeleteProductsByIds(Set<int> productIds) async {
    try {
      final productsToDelete = allProducts?.where((p) => productIds.contains(p.id)).toList() ?? [];
      
      for (final product in productsToDelete) {
        if (product.id != null) {
          await productRepository.deleteProduct(product.id!);
          
          // Remove from local state
          allProducts?.removeWhere((p) => p.id == product.id);
        }
      }
      
      // Recompute low stock list
      _computeLowStock();
      notifyListeners();
    } catch (e) {
      debugPrint('Error deleting products by IDs: $e');
      rethrow;
    }
  }

  // Barcode scanning functionality
  Future<ProductEntity?> searchProductByBarcode(String barcode) async {
    try {
      final result = await productRepository.getProductByBarcode(barcode);
      if (result.isSuccess) {
        return result.data;
      } else {
        debugPrint('Product not found for barcode: $barcode');
        return null;
      }
    } catch (e) {
      debugPrint('Error searching product by barcode: $e');
      return null;
    }
  }
}
